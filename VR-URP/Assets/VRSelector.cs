using UnityEngine;

namespace GaussianSplatting.Runtime
{
    public class VRSelector : MonoBehaviour
    {
        public float radius = 20f;
        public GaussianSplatRenderer gs;
        public Color lineColor = Color.green;

        private static Mesh sphereMesh;
        private Material lineMaterial;

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
        }

        void OnRenderObject()
        {
            if (sphereMesh == null || lineMaterial == null) return;

            lineMaterial.SetPass(0);
            GL.wireframe = true;

            Matrix4x4 matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one * radius * 2);
            Graphics.DrawMeshNow(sphereMesh, matrix);

            GL.wireframe = false;
        }

        void Update()
        {
            Vector3 sphereCenter = transform.position;
            gs.SelectSplatsInSphere(sphereCenter, radius);

        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
