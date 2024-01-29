using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        GameObject m_SelectionPrefab;
        [SerializeField]
        GameObject m_SquarePrefab;
        [SerializeField]
        GameObject m_RectanglePrefab;

        [SerializeField]
        Mesh m_Mesh;
        [SerializeField]
        Material m_OutlineMaterial;
        [SerializeField]
        Material m_SquareMaterial;
        [SerializeField]
        Texture2D m_OutlineTexture;
        [SerializeField]
        Texture2D m_SquareTexture;

        [SerializeField]
        float m_BoardWidth;
        [SerializeField]
        float m_BoardHeight;

        [SerializeField]
        float m_RectangleWidth;
        [SerializeField]
        float m_RectangleHeight;

        static readonly int ms_MainTexID = Shader.PropertyToID ("_MainTex");
        static readonly int ms_ColorID = Shader.PropertyToID ("_Color");
        static MaterialPropertyBlock ms_OutlineMaterialPropertyBlock = null;
        static MaterialPropertyBlock ms_SquareMaterialPropertyBlock = null;

        NgQuadTree2D m_Root;
        NgSelection m_Selection = null;
        readonly List<NgMovingRectangle> m_MovingRectangles = new ();

        SelectionSystem m_SelectionSystem;

        public void Awake ()
        {
            ms_OutlineMaterialPropertyBlock = new MaterialPropertyBlock ();
            ms_OutlineMaterialPropertyBlock.SetTexture (ms_MainTexID, m_OutlineTexture);
            ms_OutlineMaterialPropertyBlock.SetColor (ms_ColorID, Color.blue);

            ms_SquareMaterialPropertyBlock = new MaterialPropertyBlock ();

            m_Root = new (5, 4, new NgBound2D (Vector2.zero, new Vector2 (m_BoardWidth, m_BoardHeight)));

            m_Selection = new NgSelection (Instantiate (m_SelectionPrefab, Vector3.zero, Quaternion.identity, transform));
            m_Selection.Disable ();

            m_SelectionSystem = gameObject.GetComponent<SelectionSystem> ();
            m_SelectionSystem.OnClick += (Vector2 mousePosition) => AddRectangle (mousePosition);
            m_SelectionSystem.OnBeginDrag += (Vector2 mousePosition) => m_Selection.OnBeginDrag (mousePosition);
            m_SelectionSystem.OnDrag += (Vector2 mousePosition) => m_Selection.OnDrag (mousePosition);
            m_SelectionSystem.OnEndDrag += (Vector2 mousePosition) => m_Selection.OnEndDrag ();
        }

        public void Start ()
        {
            float x = (m_BoardWidth - m_RectangleWidth) * 0.5f;
            float y = (m_BoardHeight - m_RectangleHeight) * 0.5f;

            NgMovingRectangle right = new ();
            right.SetPosition (new Vector2 (x, 0f));
            right.SetSize (new Vector2 (m_RectangleWidth, m_BoardHeight));
            right.SetDynamic (false);
            m_MovingRectangles.Add (right);

            NgMovingRectangle up = new ();
            up.SetPosition (new Vector2 (0f, y));
            up.SetSize (new Vector2 (m_BoardWidth, m_RectangleHeight));
            up.SetDynamic (false);
            m_MovingRectangles.Add (up);

            NgMovingRectangle left = new ();
            left.SetPosition (new Vector2 (-x, 0f));
            left.SetSize (new Vector2 (m_RectangleWidth, m_BoardHeight));
            left.SetDynamic (false);
            m_MovingRectangles.Add (left);

            NgMovingRectangle down = new ();
            down.SetPosition (new Vector2 (0f, -y));
            down.SetSize (new Vector2 (m_BoardWidth, m_RectangleHeight));
            down.SetDynamic (false);
            m_MovingRectangles.Add (down);
        }

        void Update ()
        {
            float deltaTime = Time.deltaTime;

            m_Root.Reset ();

            if (m_Selection.Enabled)
            {
                m_Root.TryInsert (m_Selection.Collider);
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

            DrawRectangles ();
        }

        void DrawRectangles ()
        {
            foreach (NgMovingRectangle movingRectangle in m_MovingRectangles)
            {
                //ms_SquareMaterialPropertyBlock.SetColor (ms_ColorID, Color.red);

                RenderParams rp = new ()
                {
                    worldBounds = new Bounds (Vector3.zero, new Vector3 (m_BoardWidth, m_BoardHeight, 0f)),
                    material = m_SquareMaterial,
                    matProps = ms_SquareMaterialPropertyBlock
                };

                Graphics.RenderMesh (rp, m_Mesh, 0, movingRectangle.ObjectToWorld);
            }
        }

        public void AddRectangle (Vector2 position)
        {
            int times = 300;
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
