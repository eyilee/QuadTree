using UnityEngine;

namespace ProjectNothing
{
    public class SelectionSystem : MonoBehaviour
    {
        public delegate void OnClickCallback (Vector2 mousePosition);
        public delegate void OnBeginDragCallback (Vector2 mousePosition);
        public delegate void OnDragCallback (Vector2 mousePosition);
        public delegate void OnEndDragCallback (Vector2 mousePosition);

        [SerializeField]
        Camera m_Camera;

        float m_Delay = 0.2f;

        public float Delay
        {
            get => m_Delay;
            set => m_Delay = value;
        }

        float m_PressTime = 0f;
        bool m_IsDragging = false;

        public event OnClickCallback OnClick;
        public event OnBeginDragCallback OnBeginDrag;
        public event OnDragCallback OnDrag;
        public event OnEndDragCallback OnEndDrag;

        public void Update ()
        {
            if (Input.GetMouseButtonDown (0))
            {
                m_PressTime = Time.time;
            }

            if (Input.GetMouseButtonUp (0))
            {
                Vector2 mousePosition = m_Camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, m_Camera.nearClipPlane));

                if (Time.time - m_PressTime <= m_Delay)
                {
                    OnClick?.Invoke (mousePosition);
                }

                if (m_IsDragging)
                {
                    m_IsDragging = false;

                    OnEndDrag?.Invoke (mousePosition);
                }
            }

            if (Input.GetMouseButton (0))
            {
                Vector2 mousePosition = m_Camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, m_Camera.nearClipPlane));

                if (!m_IsDragging)
                {
                    m_IsDragging = true;

                    OnBeginDrag?.Invoke (mousePosition);
                }
                else
                {
                    OnDrag?.Invoke (mousePosition);
                }
            }
        }
    }
}
