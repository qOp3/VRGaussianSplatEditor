using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GaussianSplatting.Runtime
{
    public class VRSelector : MonoBehaviour
    {
        public float radius = 0.3f;
        public GaussianSplatRenderer gs;
        public bool selectionMode = false;
        public InputActionProperty rightTriggerPress;
        public Slider slider;

        private static Mesh sphereMesh; // a sphere to indicate the selection range
        private Material lineMaterial;
        private bool subtract;     // add or subtract gs splats

        void Start()
        {
            if (sphereMesh == null)
            {
                GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphereMesh = temp.GetComponent<MeshFilter>().sharedMesh;
                Destroy(temp);
            }

            lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_ZWrite", 1);
            lineMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);

            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        void OnRenderObject()
        {
            if (selectionMode)
            {
                if (sphereMesh == null || lineMaterial == null) return;

                lineMaterial.SetPass(0);
                GL.wireframe = true;

                Matrix4x4 matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one * radius * 2);
                Graphics.DrawMeshNow(sphereMesh, matrix);

                GL.wireframe = false;
            }
        }

        void Update()
        {
            if (selectionMode)
            {
                Vector3 sphereCenter = transform.position;
                float right = rightTriggerPress.action.ReadValue<float>();
                subtract = (right > 0.5f) ? true : false; // press trigger to substract, release to add 
                gs.SelectSplatsInSphere(sphereCenter, radius, subtract); // make selection in GaussianSplatRender script
            }


        }

        public void SelectionModeSwitch()
        {
            selectionMode = !selectionMode;
        }

        public void DeleteSelectedSplats()
        {
            gs.EditDeleteSelected(); // Delete splats
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        void OnSliderValueChanged(float value)
        {
            gs.m_SplatScale = value;
        }
    }
}
