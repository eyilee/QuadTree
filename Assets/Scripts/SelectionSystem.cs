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

        float m_LeftPressTime = 0f;
        bool m_IsLeftDragging = false;

        public event OnClickCallback OnLeftClick;
        public event OnBeginDragCallback OnLeftBeginDrag;
        public event OnDragCallback OnLeftDrag;
        public event OnEndDragCallback OnLeftEndDrag;

        float m_RightPressTime = 0f;
        bool m_IsRightDragging = false;

        public event OnClickCallback OnRightClick;
        public event OnBeginDragCallback OnRightBeginDrag;
        public event OnDragCallback OnRightDrag;
        public event OnEndDragCallback OnRightEndDrag;

        public void Update ()
        {
            HandleMouseDown ();
            HandleMouseUp ();
            HandleMousePressed ();
        }

        public void HandleMouseDown ()
        {
            if (Input.GetMouseButtonDown (0))
            {
                m_LeftPressTime = Time.time;
            }

            if (Input.GetMouseButtonDown (1))
            {
                m_RightPressTime = Time.time;
            }
        }

        public void HandleMouseUp ()
        {
            if (Input.GetMouseButtonUp (0))
            {
                Vector2 mousePosition = m_Camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, m_Camera.nearClipPlane));

                if (Time.time - m_LeftPressTime <= m_Delay)
                {
                    OnLeftClick?.Invoke (mousePosition);
                }

                if (m_IsLeftDragging)
                {
                    m_IsLeftDragging = false;

                    OnLeftEndDrag?.Invoke (mousePosition);
                }
            }

            if (Input.GetMouseButtonUp (1))
            {
                Vector2 mousePosition = m_Camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, m_Camera.nearClipPlane));

                if (Time.time - m_RightPressTime <= m_Delay)
                {
                    OnRightClick?.Invoke (mousePosition);
                }

                if (m_IsRightDragging)
                {
                    m_IsRightDragging = false;

                    OnRightEndDrag?.Invoke (mousePosition);
                }
            }
        }

        public void HandleMousePressed ()
        {
            if (Input.GetMouseButton (0))
            {
                Vector2 mousePosition = m_Camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, m_Camera.nearClipPlane));

                if (!m_IsLeftDragging)
                {
                    m_IsLeftDragging = true;

                    OnLeftBeginDrag?.Invoke (mousePosition);
                }
                else
                {
                    OnLeftDrag?.Invoke (mousePosition);
                }
            }

            if (Input.GetMouseButton (1))
            {
                Vector2 mousePosition = m_Camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, m_Camera.nearClipPlane));

                if (!m_IsRightDragging)
                {
                    m_IsRightDragging = true;

                    OnRightBeginDrag?.Invoke (mousePosition);
                }
                else
                {
                    OnRightDrag?.Invoke (mousePosition);
                }
            }
        }
    }
}
