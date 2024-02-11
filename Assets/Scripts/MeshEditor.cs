using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class MeshEditor : MonoBehaviour
    {
        [SerializeField]
        Mesh m_VertexMesh;
        [SerializeField]
        Material m_VertexMaterial;
        [SerializeField]
        bool m_IsConvexHull;

        static readonly int ms_ColorID = Shader.PropertyToID ("_Color");
        static MaterialPropertyBlock ms_VertexMaterialPropertyBlock = null;

        readonly List<Vector2> m_Vertices = new ();

        SelectionSystem m_SelectionSystem;
        Mesh m_NewMesh;

        public void Awake ()
        {
            ms_VertexMaterialPropertyBlock = new MaterialPropertyBlock ();

            m_SelectionSystem = gameObject.GetComponent<SelectionSystem> ();
            m_SelectionSystem.OnLeftClick += (Vector2 mousePosition) => AddVertex (mousePosition);
            m_SelectionSystem.OnRightClick += (Vector2 mousePosition) => Complete ();
        }

        public void Start ()
        {
            // for test
            AddVertex (new Vector2 (0f, 0f));
            AddVertex (new Vector2 (1f, 1f));
            AddVertex (new Vector2 (2f, 2f));
            AddVertex (new Vector2 (0f, 3f));
            AddVertex (new Vector2 (-2f, 2f));
            AddVertex (new Vector2 (-1f, 1f));
        }

        public void Update ()
        {
            int index = 1;

            foreach (Vector2 vertex in m_Vertices)
            {
                float factor = Mathf.Lerp (0.5f, 1f, index / (float)m_Vertices.Count);

                ms_VertexMaterialPropertyBlock.SetColor (ms_ColorID, Color.white * factor);

                RenderParams rp = new ()
                {
                    material = m_VertexMaterial,
                    matProps = ms_VertexMaterialPropertyBlock
                };

                Graphics.RenderMesh (rp, m_VertexMesh, 0, Matrix4x4.TRS (vertex, Quaternion.identity, Vector2.one * 0.1f));

                index++;
            }

            if (m_NewMesh != null)
            {
                RenderParams rp = new ()
                {
                    material = m_VertexMaterial,
                    matProps = ms_VertexMaterialPropertyBlock
                };

                Graphics.RenderMesh (rp, m_NewMesh, 0, Matrix4x4.TRS (Vector3.zero, Quaternion.identity, Vector3.one));
            }
        }

        void AddVertex (Vector2 mousePosition)
        {
            m_Vertices.Add (mousePosition);
        }

        void Complete ()
        {
            if (m_Vertices.Count >= 3)
            {
                m_NewMesh = m_IsConvexHull
                    ? MeshFactory.CreatePolygon (NgPhysics2D.GenerateConvexHull (m_Vertices))
                    : MeshFactory.CreatePolygon (m_Vertices);
            }

            m_Vertices.Clear ();
        }
    }
}
