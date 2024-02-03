using UnityEngine;

namespace ProjectNothing
{
    public class NgSelection : NgGameObject
    {
        readonly NgCollider2D m_Collider = null;
        Vector2 m_BeginPosition;
        Vector2 m_EndPosition;

        public NgCollider2D Collider => m_Collider;
        public Matrix4x4 ObjectToWorld => Matrix4x4.TRS (Transform.Position, Quaternion.AngleAxis (Transform.Rotation, Vector3.forward), Transform.Scale);

        bool m_IsActive = false;

        public bool IsActive => m_IsActive;

        public NgSelection ()
        {
            m_Collider = AddComponent<NgCollider2D> ();
            m_Collider.LayerMask = (int)NgLayerMask.Selection;
        }

        public void OnBeginDrag (Vector2 position)
        {
            m_BeginPosition = position;
            m_EndPosition = position;

            m_IsActive = true;

            Update ();
        }

        public void OnDrag (Vector2 position)
        {
            m_EndPosition = position;

            Update ();
        }

        public void OnEndDrag ()
        {
            m_IsActive = false;
        }

        public void Update ()
        {
            if (!m_IsActive)
            {
                return;
            }

            Transform.Position = (m_BeginPosition + m_EndPosition) * 0.5f;

            Vector2 diff = m_EndPosition - m_BeginPosition;
            Transform.Scale = new Vector2 (Mathf.Abs (diff.x), Mathf.Abs (diff.y));

            m_Collider.BoundingBox = new NgBoundingBox2D (Transform.Position, Transform.Scale);
        }
    }
}
