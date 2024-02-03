﻿using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class NgRectangle : NgGameObject
    {
        readonly NgCollider2D m_Collider = null;
        Vector2 m_Velocity;
        float m_AngularVelocity;

        public NgCollider2D Collider => m_Collider;
        public Matrix4x4 ObjectToWorld => Matrix4x4.TRS (Transform.Position, Quaternion.AngleAxis (Transform.Rotation, Vector3.forward), Transform.Scale);

        public Vector2 Velocity
        {
            get => m_Velocity;
            set => m_Velocity = value;
        }

        public float AngularVelocity
        {
            get => m_AngularVelocity;
            set => m_AngularVelocity = value;
        }

        bool m_IsSelected = false;

        public bool IsSelected => m_IsSelected;

        public NgRectangle (Vector2 position, float roration, Vector2 scale)
        {
            Transform.Position = position;
            Transform.Rotation = roration;
            Transform.Scale = scale;

            m_Collider = AddComponent<NgCollider2D> ();
            m_Collider.LayerMask = (int)(NgLayerMask.Collision | NgLayerMask.Selection);

            Vector2 rotated = (scale * 0.5f).Rotate (roration);
            Vector2 size = 2f * Mathf.Max (Mathf.Abs (rotated.x), Mathf.Abs (rotated.y)) * Vector2.one;
            m_Collider.BoundingBox = new NgBoundingBox2D (position, size);
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
                Transform.Position += m_Velocity * deltaTime;
                Transform.Rotation += m_AngularVelocity * deltaTime;

                Vector2 rotated = (Transform.Scale * 0.5f).Rotate (Transform.Rotation);
                Vector2 size = 2f * Mathf.Max (Mathf.Abs (rotated.x), Mathf.Abs (rotated.y)) * Vector2.one;
                m_Collider.BoundingBox = new NgBoundingBox2D (Transform.Position, size);

                if (Transform.Position.x > 5f && m_Velocity.x > 0)
                {
                    m_Velocity = new Vector2 (-m_Velocity.x, m_Velocity.y);
                }
                else if (Transform.Position.x < -5f && m_Velocity.x < 0)
                {
                    m_Velocity = new Vector2 (-m_Velocity.x, m_Velocity.y);
                }

                if (Transform.Position.y > 5f && m_Velocity.y > 0)
                {
                    m_Velocity = new Vector2 (m_Velocity.x, -m_Velocity.y);
                }
                else if (Transform.Position.y < -5f && m_Velocity.y < 0)
                {
                    m_Velocity = new Vector2 (m_Velocity.x, -m_Velocity.y);
                }
            }
        }
    }
}
