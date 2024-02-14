using UnityEngine;

namespace ProjectNothing
{
    public class NgSelection : NgGameObject
    {
        readonly SpriteRenderer m_SpriteRenderer = null;

        readonly NgCollider2D m_Collider = null;
        Vector2 m_BeginPosition;
        Vector2 m_EndPosition;

        public NgCollider2D Collider => m_Collider;
        public Matrix4x4 ObjectToWorld => Matrix4x4.TRS (Transform.Position, Quaternion.AngleAxis (Transform.Rotation, Vector3.forward), Transform.Scale);

        public bool IsActive => m_SpriteRenderer.enabled;

        public NgSelection (GameObject gameObject)
        {
            m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
            m_SpriteRenderer.color = Color.green;

            m_Collider = AddComponent<NgCollider2D> ();
            m_Collider.LayerMask = NgLayerMask.Selection;
        }

        public void OnBeginDrag (Vector2 position)
        {
            m_BeginPosition = position;
            m_EndPosition = position;

            if (!m_SpriteRenderer.enabled)
            {
                m_SpriteRenderer.enabled = true;
            }

            Update ();
        }

        public void OnDrag (Vector2 position)
        {
            m_EndPosition = position;

            Update ();
        }

        public void OnEndDrag ()
        {
            if (m_SpriteRenderer.enabled)
            {
                m_SpriteRenderer.enabled = false;
            }
        }

        public void Update ()
        {
            if (!m_SpriteRenderer.enabled)
            {
                return;
            }

            Transform.Position = (m_BeginPosition + m_EndPosition) * 0.5f;

            Vector2 diff = m_EndPosition - m_BeginPosition;
            Transform.Scale = new Vector2 (Mathf.Abs (diff.x), Mathf.Abs (diff.y));

            m_SpriteRenderer.transform.position = Transform.Position;
            m_SpriteRenderer.size = Transform.Scale;

            m_Collider.SetRectangle (Transform.Position, Transform.Scale, Transform.Rotation);
        }
    }
}
