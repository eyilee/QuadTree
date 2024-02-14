using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        Material m_UnlitMaterial;
        [SerializeField]
        GameObject m_SelectionPrefab;

        [SerializeField]
        float m_BoardWidth;
        [SerializeField]
        float m_BoardHeight;

        [SerializeField]
        float m_RectangleWidth;
        [SerializeField]
        float m_RectangleHeight;

        static readonly int ms_ColorID = Shader.PropertyToID ("_Color");
        static MaterialPropertyBlock ms_UnlitMaterialPropertyBlock = null;

        NgQuadTree2D m_Root = null;

        Mesh m_Mesh;
        readonly List<NgRectangle> m_Rectangles = new ();

        NgSelection m_Selection = null;
        SelectionSystem m_SelectionSystem;

        public void Awake ()
        {
            ms_UnlitMaterialPropertyBlock = new MaterialPropertyBlock ();

            m_Root = new (5, 4, new NgBoundingBox2D (Vector2.zero, new Vector2 (m_BoardWidth, m_BoardHeight)));

            m_Mesh = MeshFactory.CreateRectangle (Vector2.one);

            m_Selection = new NgSelection (Instantiate (m_SelectionPrefab, Vector3.zero, Quaternion.identity, transform));
            m_SelectionSystem = gameObject.GetComponent<SelectionSystem> ();
            m_SelectionSystem.OnLeftClick += (Vector2 mousePosition) => AddRectangle (mousePosition);
            m_SelectionSystem.OnLeftBeginDrag += (Vector2 mousePosition) => m_Selection.OnBeginDrag (mousePosition);
            m_SelectionSystem.OnLeftDrag += (Vector2 mousePosition) => m_Selection.OnDrag (mousePosition);
            m_SelectionSystem.OnLeftEndDrag += (Vector2 mousePosition) => m_Selection.OnEndDrag ();
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

            foreach (NgRectangle rectangle in m_Rectangles)
            {
                rectangle.Update (deltaTime);
                m_Root.TryInsert (rectangle.Collider);
            }

            List<NgCollider2D> colliders = new ();
            List<NgCollision2D> collisions = new ();
            foreach (NgRectangle rectangle in m_Rectangles)
            {
                colliders.Clear ();
                collisions.Clear ();

                // Broad phase
                m_Root.Query (rectangle.Collider, colliders);

                // Narrow phase
                rectangle.Collider.GetCollisions (colliders, collisions);

                foreach (NgCollision2D collision in collisions)
                {
                    rectangle.OnCollision (collision);
                }
            }

            DrawRectangles ();
        }

        void DrawRectangles ()
        {
            List<Matrix4x4> rectangles = new ();
            List<Matrix4x4> selects = new ();

            foreach (NgRectangle rectangle in m_Rectangles)
            {
                if (!rectangle.IsSelected)
                {
                    rectangles.Add (rectangle.ObjectToWorld);
                }
                else
                {
                    selects.Add (rectangle.ObjectToWorld);
                }
            }

            if (rectangles.Count > 0)
            {
                ms_UnlitMaterialPropertyBlock.SetColor (ms_ColorID, Color.white);

                RenderParams rp = new ()
                {
                    worldBounds = new Bounds (Vector3.zero, new Vector3 (m_BoardWidth, m_BoardHeight, 0f)),
                    material = m_UnlitMaterial,
                    matProps = ms_UnlitMaterialPropertyBlock
                };

                Graphics.RenderMeshInstanced (rp, m_Mesh, 0, rectangles);
            }

            if (selects.Count > 0)
            {
                ms_UnlitMaterialPropertyBlock.SetColor (ms_ColorID, Color.red);

                RenderParams rp = new ()
                {
                    worldBounds = new Bounds (Vector3.zero, new Vector3 (m_BoardWidth, m_BoardHeight, 0f)),
                    material = m_UnlitMaterial,
                    matProps = ms_UnlitMaterialPropertyBlock
                };

                Graphics.RenderMeshInstanced (rp, m_Mesh, 0, selects);
            }
        }

        public void AddRectangle (Vector2 position)
        {
            int times = 100;
            while (times-- > 0)
            {
                float width = Random.Range (1f, 3f) * m_RectangleWidth;
                float height = Random.Range (1f, 3f) * m_RectangleHeight;
                NgRectangle rectangle = new (position, 0f, new Vector2 (width, height));

                Vector2 velocity = Random.insideUnitCircle;
                velocity.Normalize ();
                rectangle.Velocity = velocity * Random.Range (1f, 3f);

                rectangle.AngularVelocity = 180f * (Random.Range (0f, 1f) > 0.5f ? 1f : -1f);

                m_Rectangles.Add (rectangle);
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

                Gizmos.DrawWireCube (quadTree.BoundingBox.Center, quadTree.BoundingBox.Size);
            }

            Gizmos.color = Color.green;

            foreach (NgRectangle rectangle in m_Rectangles)
            {
                Gizmos.DrawWireCube (rectangle.Collider.BoundingBox.Center, rectangle.Collider.BoundingBox.Size);
            }
        }
    }
}
