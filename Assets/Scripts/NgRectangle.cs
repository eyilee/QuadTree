using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class NgRectangle
    {
        readonly NgCollider2D m_Collider = null;
        Vector2 m_Position;
        Vector2 m_Size;
        Vector2 m_Velocity;
        float m_Angle = 0f;
        float m_AngularVelocity = 0f;

        public NgCollider2D Collider => m_Collider;
        public Matrix4x4 ObjectToWorld => Matrix4x4.TRS (m_Position, Quaternion.AngleAxis (m_Angle, Vector3.forward), m_Size);

        public Vector2 Position
        {
            get => m_Position;
            set => m_Position = value;
        }

        public Vector2 Size
        {
            get => m_Size;
            set => m_Size = value;
        }

        public Vector2 Velocity
        {
            get => m_Velocity;
            set => m_Velocity = value;
        }

        public float Angle
        {
            get => m_Angle;
            set => m_Angle = value;
        }

        public float AngularVelocity
        {
            get => m_AngularVelocity;
            set => m_AngularVelocity = value;
        }

        bool m_IsSelected = false;

        public bool IsSelected => m_IsSelected;

        public NgRectangle (Vector2 position, Vector2 size, float angle = 0f)
        {
            m_Position = position;
            m_Size = size;
            m_Angle = angle;

            m_Collider = new NgCollider2D
            {
                LayerMask = (int)(NgLayerMask.Collision | NgLayerMask.Selection),
            };

            Vector2 rotated = (m_Size * 0.5f).Rotate (m_Angle);
            float max = Mathf.Max (Mathf.Abs (rotated.x), Mathf.Abs (rotated.y)) * 2f;
            m_Collider.Bound = new NgBound2D (m_Position, new Vector2 (max, max));
        }

        public void OnCollision (List<NgCollider2D> colliders)
        {
            m_IsSelected = false;

            foreach (NgCollider2D collider in colliders)
            {
                if (collider.LayerMask == (int)NgLayerMask.Selection)
                {
                    m_IsSelected = true;
                }
            }
        }

        public void Update (float deltaTime)
        {
            if (Mathf.Abs (m_Velocity.x) > float.Epsilon || Mathf.Abs (m_Velocity.y) > float.Epsilon)
            {
                m_Position += m_Velocity * deltaTime;
                m_Angle += m_AngularVelocity * deltaTime;

                Vector2 rotated = (m_Size * 0.5f).Rotate (m_Angle);
                float max = Mathf.Max (Mathf.Abs (rotated.x), Mathf.Abs (rotated.y)) * 2f;
                m_Collider.Bound = new NgBound2D (m_Position, new Vector2 (max, max));

                if (m_Position.x > 5f && m_Velocity.x > 0)
                {
                    m_Velocity = new Vector2 (-m_Velocity.x, m_Velocity.y);
                }
                else if (m_Position.x < -5f && m_Velocity.x < 0)
                {
                    m_Velocity = new Vector2 (-m_Velocity.x, m_Velocity.y);
                }

                if (m_Position.y > 5f && m_Velocity.y > 0)
                {
                    m_Velocity = new Vector2 (m_Velocity.x, -m_Velocity.y);
                }
                else if (m_Position.y < -5f && m_Velocity.y < 0)
                {
                    m_Velocity = new Vector2 (m_Velocity.x, -m_Velocity.y);
                }
            }
        }
    }
}
