﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
//using GaussianSplatting.Editor;

namespace GaussianSplatting.Runtime
{
    public class GaussianVRController : MonoBehaviour
    {
        public GaussianSplatRenderer gs;
        public Transform t;
        public Transform Camera;
        public GameObject cutoutPrefab;
        private int c = 0;

        void Start()
        {

        }

        // splat format 
        public struct InputSplatData
        {
            public Vector3 pos;
            public Vector3 nor;
            public Vector3 dc0;
            public Vector3 sh1, sh2, sh3, sh4, sh5, sh6, sh7, sh8, sh9, shA, shB, shC, shD, shE, shF;
            public float opacity;
            public Vector3 scale;
            public Quaternion rot;
        }

        // add cutout box
        public void AddCutout()
        {
            Vector3 spawnPosition = Camera.position + Camera.forward * 3f;
            Quaternion spawnRotation = Quaternion.identity;
            GaussianCutout cutout = Instantiate(cutoutPrefab, spawnPosition, spawnRotation).GetComponent<GaussianCutout>();
            Transform cutoutTr = cutout.transform;
            cutoutTr.SetParent(gs.transform, true);
            gs.m_Cutouts ??= Array.Empty<GaussianCutout>();
            ArrayUtility.Add(ref gs.m_Cutouts, cutout);
            gs.UpdateEditCountsAndBounds();
        }

        public void DeleteCutout()
        {
            // If there are no cutouts, exit
            if (gs.m_Cutouts == null || gs.m_Cutouts.Length == 0)
                return;

            int lastIndex = gs.m_Cutouts.Length - 1;
            GaussianCutout lastCutout = gs.m_Cutouts[lastIndex];

            ArrayUtility.RemoveAt(ref gs.m_Cutouts, lastIndex);

            // Destroy the cutout GameObject
            if (lastCutout != null)
                Destroy(lastCutout.gameObject);

            gs.UpdateEditCountsAndBounds();
        }


        public unsafe void ExportData()
        {
            // Export edited gs data

            //export path .\VR - URP\
            bool bakeTransform = true;
            string fileName = t.name + ".ply";
            string rootPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string path = Path.Combine(rootPath, fileName);

            if (string.IsNullOrWhiteSpace(path))
                return;

            int kSplatSize = UnsafeUtility.SizeOf<InputSplatData>();
            Debug.Log($"Struct splatData size : {kSplatSize}");
            using var gpuData = new GraphicsBuffer(GraphicsBuffer.Target.Structured, gs.splatCount, kSplatSize);

            if (!gs.EditExportData(gpuData, bakeTransform))
                return;

            InputSplatData[] data = new InputSplatData[gpuData.count];
            gpuData.GetData(data);

            var gpuDeleted = gs.GpuEditDeleted;
            uint[] deleted = new uint[gpuDeleted.count];
            gpuDeleted.GetData(deleted);

            // count non-deleted splats
            int aliveCount = 0;
            for (int i = 0; i < data.Length; ++i)
            {
                int wordIdx = i >> 5;
                int bitIdx = i & 31;
                bool isDeleted = (deleted[wordIdx] & (1u << bitIdx)) != 0;
                bool isCutout = data[i].nor.sqrMagnitude > 0;
                if (!isDeleted && !isCutout)
                    ++aliveCount;
            }

            using FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            // note: this is a long string! but we don't use multiline literal because we want guaranteed LF line ending
            var header = $"ply\nformat binary_little_endian 1.0\nelement vertex {aliveCount}\nproperty float x\nproperty float y\nproperty float z\nproperty float nx\nproperty float ny\nproperty float nz\nproperty float f_dc_0\nproperty float f_dc_1\nproperty float f_dc_2\nproperty float f_rest_0\nproperty float f_rest_1\nproperty float f_rest_2\nproperty float f_rest_3\nproperty float f_rest_4\nproperty float f_rest_5\nproperty float f_rest_6\nproperty float f_rest_7\nproperty float f_rest_8\nproperty float f_rest_9\nproperty float f_rest_10\nproperty float f_rest_11\nproperty float f_rest_12\nproperty float f_rest_13\nproperty float f_rest_14\nproperty float f_rest_15\nproperty float f_rest_16\nproperty float f_rest_17\nproperty float f_rest_18\nproperty float f_rest_19\nproperty float f_rest_20\nproperty float f_rest_21\nproperty float f_rest_22\nproperty float f_rest_23\nproperty float f_rest_24\nproperty float f_rest_25\nproperty float f_rest_26\nproperty float f_rest_27\nproperty float f_rest_28\nproperty float f_rest_29\nproperty float f_rest_30\nproperty float f_rest_31\nproperty float f_rest_32\nproperty float f_rest_33\nproperty float f_rest_34\nproperty float f_rest_35\nproperty float f_rest_36\nproperty float f_rest_37\nproperty float f_rest_38\nproperty float f_rest_39\nproperty float f_rest_40\nproperty float f_rest_41\nproperty float f_rest_42\nproperty float f_rest_43\nproperty float f_rest_44\nproperty float opacity\nproperty float scale_0\nproperty float scale_1\nproperty float scale_2\nproperty float rot_0\nproperty float rot_1\nproperty float rot_2\nproperty float rot_3\nend_header\n";
            fs.Write(Encoding.UTF8.GetBytes(header));
            Debug.Log($"Total data length {data.Length}");
            for (int i = 0; i < data.Length; ++i)
            {
                int wordIdx = i >> 5;
                int bitIdx = i & 31;
                bool isDeleted = (deleted[wordIdx] & (1u << bitIdx)) != 0;
                bool isCutout = data[i].nor.sqrMagnitude > 0;
                if (!isDeleted && !isCutout)
                {
                    var splat = data[i];
                    byte* ptr = (byte*)&splat;
                    fs.Write(new ReadOnlySpan<byte>(ptr, kSplatSize));
                }
            }

            Debug.Log($"Exported PLY {path} with {aliveCount:N0} splats");
        }
    }
}
