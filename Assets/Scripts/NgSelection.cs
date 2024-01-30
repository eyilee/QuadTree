using UnityEngine;

namespace ProjectNothing
{
    public class NgSelection
    {
        readonly NgCollider2D m_Collider = null;
        Vector2 m_BeginPosition;
        Vector2 m_EndPosition;

        public NgCollider2D Collider => m_Collider;
        public Matrix4x4 ObjectToWorld => Matrix4x4.TRS (m_Collider.Bound.Center, Quaternion.identity, m_Collider.Bound.Size);

        bool m_IsActive = false;

        public bool IsActive => m_IsActive;

        public NgSelection ()
        {
            m_Collider = new NgCollider2D
            {
                LayerMask = (int)NgLayerMask.Selection,
            };
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

            Vector2 position = (m_BeginPosition + m_EndPosition) * 0.5f;
            Vector2 size = m_EndPosition - m_BeginPosition;

            m_Collider.Bound = new NgBound2D (position, new Vector2 (Mathf.Abs (size.x), Mathf.Abs (size.y)));
        }
    }
}
