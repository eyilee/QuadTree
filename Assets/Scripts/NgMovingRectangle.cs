using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class NgMovingRectangle
    {
        readonly NgCollider2D m_Collider = null;
        bool m_IsDynamic = false;
        float m_Velocity = 0f;
        Vector2 m_Forward = Vector2.zero;

        public NgCollider2D Collider => m_Collider;
        public Matrix4x4 ObjectToWorld => Matrix4x4.TRS (m_Collider.Bound.Center, Quaternion.identity, m_Collider.Bound.Size);

        bool m_InCollision = false;
        bool m_IsSelected = false;

        public bool IsSelected => m_IsSelected;

        public float Velocity
        {
            get => m_Velocity;
            set => m_Velocity = value;
        }

        public Vector2 Forward
        {
            get => m_Forward;
            set => m_Forward = value;
        }

        public NgMovingRectangle ()
        {
            m_Collider = new NgCollider2D
            {
                LayerMask = (int)(NgLayerMask.Collision | NgLayerMask.Selection),
                IsDynamic = true
            };
        }

        public void SetPosition (Vector2 position)
        {
            m_Collider.Bound = new NgBound2D (position, m_Collider.Bound.Size);
        }

        public void SetSize (Vector2 size)
        {
            m_Collider.Bound = new NgBound2D (m_Collider.Bound.Center, size);
        }

        public void SetDynamic (bool isDynamic)
        {
            m_IsDynamic = isDynamic;

            m_Collider.IsDynamic = isDynamic;
        }

        public void Predict (float deltaTime)
        {
            if (m_IsDynamic && m_Velocity > float.Epsilon)
            {
                Vector2 step = m_Velocity * deltaTime * m_Forward;

                NgBound2D sweepBound = new (m_Collider.Bound.Center + step, m_Collider.Bound.Size);
                sweepBound.Encapsulate (m_Collider.Bound);
                m_Collider.SweepBound = sweepBound;
            }
        }

        public void OnCollision (List<NgCollider2D> colliders)
        {
            bool inCollision = false;
            bool isSelected = false;

            foreach (NgCollider2D collider in colliders)
            {
                if (collider.LayerMask == (int)NgLayerMask.Selection)
                {
                    isSelected = true;
                }
            }

            if (inCollision)
            {
                m_InCollision = true;
            }
            else
            {
                if (m_InCollision)
                {
                    m_InCollision = false;

                    // OnCollisionExit ();
                }
            }

            if (isSelected != m_IsSelected)
            {
                m_IsSelected = isSelected;
            }
        }

        public void Update (float deltaTime)
        {
            if (m_IsDynamic && m_Velocity > float.Epsilon)
            {
                Vector2 step = m_Velocity * deltaTime * m_Forward;

                m_Collider.Bound = new NgBound2D (m_Collider.Bound.Center + step, m_Collider.Bound.Size);
                m_Collider.SweepBound = m_Collider.Bound; // TODO: remove after Predict finished

                if (m_Collider.Bound.Center.x > 5f && m_Forward.x > 0)
                {
                    m_Forward = new Vector2 (-m_Forward.x, m_Forward.y);
                }
                else if (m_Collider.Bound.Center.x < -5f && m_Forward.x < 0)
                {
                    m_Forward = new Vector2 (-m_Forward.x, m_Forward.y);
                }

                if (m_Collider.Bound.Center.y > 5f && m_Forward.y > 0)
                {
                    m_Forward = new Vector2 (m_Forward.x, -m_Forward.y);
                }
                else if (m_Collider.Bound.Center.y < -5f && m_Forward.y < 0)
                {
                    m_Forward = new Vector2 (m_Forward.x, -m_Forward.y);
                }
            }
        }
    }
}
