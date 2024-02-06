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

        static readonly int ms_ColorID = Shader.PropertyToID ("_Color");
        static MaterialPropertyBlock ms_VertexMaterialPropertyBlock = null;

        readonly List<Vector2> m_Vertices = new ();

        SelectionSystem m_SelectionSystem;

        public void Awake ()
        {
            ms_VertexMaterialPropertyBlock = new MaterialPropertyBlock ();

            m_SelectionSystem = gameObject.GetComponent<SelectionSystem> ();
            m_SelectionSystem.OnLeftClick += (Vector2 mousePosition) => AddVertex (mousePosition);
            m_SelectionSystem.OnRightClick += (Vector2 mousePosition) => Complete ();
        }

        public void Update ()
        {
            ms_VertexMaterialPropertyBlock.SetColor (ms_ColorID, Color.white);

            RenderParams rp = new ()
            {
                material = m_VertexMaterial,
                matProps = ms_VertexMaterialPropertyBlock
            };

            List<Matrix4x4> objectToWorlds = new ();
            foreach (Vector2 vertex in m_Vertices)
            {
                objectToWorlds.Add (Matrix4x4.TRS (vertex, Quaternion.identity, Vector2.one * 0.1f));
            }

            if (objectToWorlds.Count > 0)
            {
                Graphics.RenderMeshInstanced (rp, m_VertexMesh, 0, objectToWorlds);
            }
        }

        void AddVertex (Vector2 mousePosition)
        {
            m_Vertices.Add (mousePosition);

            if (m_Vertices.Count >= 8)
            {
                Complete ();
            }
        }

        void Complete ()
        {
            // TODO: Graham scan
            m_Vertices.Sort ((lhs, rhs) =>
            {
                if (lhs.y == rhs.y)
                {
                    return lhs.x.CompareTo (rhs.x);
                }
                return lhs.y.CompareTo (rhs.y);
            });

            m_Vertices.Clear ();
        }
    }
}
