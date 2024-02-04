using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class NgCollider2D : NgComponent
    {
        readonly NgPhysicsShape2D m_PhysicsShape2D = new ();
        NgLayerMask m_LayerMask;
        NgBoundingBox2D m_BoundingBox;

        public NgPhysicsShape2D PhysicsShape2D => m_PhysicsShape2D;

        public NgLayerMask LayerMask
        {
            get => m_LayerMask;
            set => m_LayerMask = value;
        }

        public NgBoundingBox2D BoundingBox => m_BoundingBox;

        public NgCollider2D (NgGameObject gameObject) : base (gameObject)
        {
        }

        public void SetRectangle (Vector2 center, Vector2 size, float angle)
        {
            float sin = Mathf.Sin (angle * Mathf.Deg2Rad);
            float cos = Mathf.Cos (angle * Mathf.Deg2Rad);
            float sinX = sin * size.x;
            float cosX = cos * size.x;
            float sinY = sin * size.y;
            float cosY = cos * size.y;

            Vector2 v1 = new Vector2 (cosX - sinY, sinX + cosY) * 0.5f;
            Vector2 v2 = new Vector2 (cosX + sinY, sinX - cosY) * 0.5f;

            List<Vector2> vertices = new ()
            {
                center + v1,
                center - v2,
                center - v1,
                center + v2
            };

            m_PhysicsShape2D.SetPolygon (vertices);

            float x = Mathf.Max (Mathf.Abs (cosX - sinY), Mathf.Abs (cosX + sinY));
            float y = Mathf.Max (Mathf.Abs (sinX + cosY), Mathf.Abs (sinX - cosY));

            m_BoundingBox = new NgBoundingBox2D (center, new Vector2 (x, y));
        }
    }
}
