using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        Mesh m_Mesh;
        [SerializeField]
        Material m_OutlineMaterial;
        [SerializeField]
        Material m_SquareMaterial;

        [SerializeField]
        float m_BoardWidth;
        [SerializeField]
        float m_BoardHeight;

        [SerializeField]
        float m_RectangleWidth;
        [SerializeField]
        float m_RectangleHeight;

        static readonly int ms_ColorID = Shader.PropertyToID ("_Color");
        static readonly int ms_BoundID = Shader.PropertyToID ("_Bound");
        static MaterialPropertyBlock ms_OutlineMaterialPropertyBlock = null;
        static MaterialPropertyBlock ms_SquareMaterialPropertyBlock = null;

        NgQuadTree2D m_Root = null;
        NgSelection m_Selection = null;
        readonly List<NgMovingRectangle> m_MovingRectangles = new ();

        SelectionSystem m_SelectionSystem;

        public void Awake ()
        {
            ms_OutlineMaterialPropertyBlock = new MaterialPropertyBlock ();
            ms_OutlineMaterialPropertyBlock.SetColor (ms_ColorID, Color.green);

            ms_SquareMaterialPropertyBlock = new MaterialPropertyBlock ();

            m_Root = new (5, 4, new NgBound2D (Vector2.zero, new Vector2 (m_BoardWidth, m_BoardHeight)));

            m_Selection = new NgSelection ();

            m_SelectionSystem = gameObject.GetComponent<SelectionSystem> ();
            m_SelectionSystem.OnClick += (Vector2 mousePosition) => AddRectangle (mousePosition);
            m_SelectionSystem.OnBeginDrag += (Vector2 mousePosition) => m_Selection.OnBeginDrag (mousePosition);
            m_SelectionSystem.OnDrag += (Vector2 mousePosition) => m_Selection.OnDrag (mousePosition);
            m_SelectionSystem.OnEndDrag += (Vector2 mousePosition) => m_Selection.OnEndDrag ();
        }

        void Update ()
        {
            float deltaTime = Time.deltaTime;

            m_Root.Reset ();

            if (m_Selection.IsActive)
            {
                m_Selection.Update ();
                m_Root.Insert (m_Selection.Collider);
            }

            foreach (NgMovingRectangle rectangle in m_MovingRectangles)
            {
                //rectangle.Predict (deltaTime);
                rectangle.Update (deltaTime);
                m_Root.TryInsert (rectangle.Collider);
            }

            List<NgCollider2D> colliders = new ();
            foreach (NgMovingRectangle rectangle in m_MovingRectangles)
            {
                colliders.Clear ();

                m_Root.Query (rectangle.Collider, colliders);

                rectangle.OnCollision (colliders);
            }

            DrawSelection ();
            DrawRectangles ();
        }

        void DrawSelection ()
        {
            if (!m_Selection.IsActive)
            {
                return;
            }

            NgBound2D bound = m_Selection.Collider.Bound;
            ms_OutlineMaterialPropertyBlock.SetVector (ms_BoundID, new Vector4 (bound.Center.x, bound.Center.y, bound.Size.x, bound.Size.y));

            RenderParams rp = new ()
            {
                worldBounds = new Bounds (Vector3.zero, new Vector3 (m_BoardWidth, m_BoardHeight, 0f)),
                material = m_OutlineMaterial,
                matProps = ms_OutlineMaterialPropertyBlock
            };

            Graphics.RenderMesh (rp, m_Mesh, 0, m_Selection.ObjectToWorld);
        }

        void DrawRectangles ()
        {
            List<Matrix4x4> rectangles = new ();
            List<Matrix4x4> selects = new ();

            foreach (NgMovingRectangle movingRectangle in m_MovingRectangles)
            {
                if (!movingRectangle.IsSelected)
                {
                    rectangles.Add (movingRectangle.ObjectToWorld);
                }
                else
                {
                    selects.Add (movingRectangle.ObjectToWorld);
                }
            }

            if (rectangles.Count > 0)
            {
                ms_SquareMaterialPropertyBlock.SetColor (ms_ColorID, Color.white);

                RenderParams rp = new ()
                {
                    worldBounds = new Bounds (Vector3.zero, new Vector3 (m_BoardWidth, m_BoardHeight, 0f)),
                    material = m_SquareMaterial,
                    matProps = ms_SquareMaterialPropertyBlock
                };

                Graphics.RenderMeshInstanced (rp, m_Mesh, 0, rectangles);
            }

            if (selects.Count > 0)
            {
                ms_SquareMaterialPropertyBlock.SetColor (ms_ColorID, Color.red);

                RenderParams rp = new ()
                {
                    worldBounds = new Bounds (Vector3.zero, new Vector3 (m_BoardWidth, m_BoardHeight, 0f)),
                    material = m_SquareMaterial,
                    matProps = ms_SquareMaterialPropertyBlock
                };

                Graphics.RenderMeshInstanced (rp, m_Mesh, 0, selects);
            }
        }

        public void AddRectangle (Vector2 position)
        {
            int times = 100;
            while (times-- > 0)
            {
                NgMovingRectangle rectangle = new ();
                rectangle.SetPosition (position);
                rectangle.SetSize (new Vector2 (m_RectangleWidth, m_RectangleHeight));
                rectangle.SetDynamic (true);
                rectangle.Velocity = Random.Range (1f, 2f);

                Vector2 forward = Random.insideUnitCircle;
                forward.Normalize ();
                rectangle.Forward = forward;

                m_MovingRectangles.Add (rectangle);
            }
        }

        public void OnDrawGizmos ()
        {
            if (m_Root == null)
            {
                return;
            }

            Gizmos.color = Color.white;

            Queue<NgQuadTree2D> queue = new ();
            queue.Enqueue (m_Root);

            while (queue.TryDequeue (out NgQuadTree2D quadTree))
            {
                foreach (NgQuadTree2D subQuadTree in quadTree.QuadTrees)
                {
                    queue.Enqueue (subQuadTree);
                }

                Gizmos.DrawWireCube (quadTree.Bound.Center, quadTree.Bound.Size);
            }
        }
    }
}
