using UnityEngine;

namespace ProjectNothing
{
    public class NgSelection : NgGameObject2D
    {
        readonly SpriteRenderer m_SpriteRenderer = null;

        readonly NgCollider2D m_Collider = null;
        Vector2 m_BeginPosition;
        Vector2 m_EndPosition;

        public NgCollider2D Collider => m_Collider;
        public bool Enabled => m_SpriteRenderer != null && m_SpriteRenderer.enabled;

        public NgSelection (GameObject gameObject) : base (gameObject)
        {
            m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
            m_SpriteRenderer.color = Color.green;

            m_Collider = AddComponent<NgCollider2D> ();
            m_Collider.LayerMask = (int)NgLayerMask.Selection;
            m_Collider.IsDynamic = false;
        }

        public void Enable ()
        {
            if (!m_SpriteRenderer.enabled)
            {
                m_SpriteRenderer.enabled = true;
            }
        }

        public void Disable ()
        {
            if (m_SpriteRenderer.enabled)
            {
                m_SpriteRenderer.enabled = false;
            }
        }

        public void OnBeginDrag (Vector2 position)
        {
            m_BeginPosition = position;
            m_EndPosition = position;

            Enable ();

            Update ();
        }

        public void OnDrag (Vector2 position)
        {
            m_EndPosition = position;

            Update ();
        }

        public void OnEndDrag ()
        {
            Disable ();
        }

        public void Update ()
        {
            if (!m_SpriteRenderer.enabled)
            {
                return;
            }

            Vector2 position = (m_BeginPosition + m_EndPosition) * 0.5f;
            Vector2 size = m_EndPosition - m_BeginPosition;

            m_Transform.position = position;

            if (m_SpriteRenderer.drawMode == SpriteDrawMode.Simple)
            {
                m_Transform.localScale = size;
            }
            else
            {
                m_SpriteRenderer.size = size;
            }

            m_Collider.Bound = new NgBound2D (position, new Vector2 (Mathf.Abs (size.x), Mathf.Abs (size.y)));
        }
    }
}
