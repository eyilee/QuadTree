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

        public void GetCollisions (List<NgCollider2D> colliders, List<NgCollision2D> collisions)
        {
            switch (m_PhysicsShape2D.ShapeType)
            {
                case NgPhysicsShapeType2D.Point:
                    GetPointCollisions (colliders, collisions);
                    break;
                case NgPhysicsShapeType2D.Circle:
                    GetCircleCollisions (colliders, collisions);
                    break;
                case NgPhysicsShapeType2D.Line:
                    break;
                case NgPhysicsShapeType2D.Capsule:
                    break;
                case NgPhysicsShapeType2D.Triangle:
                    break;
                case NgPhysicsShapeType2D.Rectangle:
                    break;
                case NgPhysicsShapeType2D.Polygon:
                    break;
                default:
                    break;
            }
        }

        void GetPointCollisions (List<NgCollider2D> colliders, List<NgCollision2D> collisions)
        {
            Vector2 point = m_PhysicsShape2D.Vertices[0];

            colliders.ForEach (collider =>
            {
                switch (collider.m_PhysicsShape2D.ShapeType)
                {
                    case NgPhysicsShapeType2D.Point:
                        GetCollision (point, collider.m_PhysicsShape2D.Vertices[0], collisions);
                        break;
                    case NgPhysicsShapeType2D.Circle:
                        GetCollision (point, new NgCircle2D (collider.m_PhysicsShape2D.Vertices[0], collider.m_PhysicsShape2D.Radius), collisions);
                        break;
                    case NgPhysicsShapeType2D.Line:
                        break;
                    case NgPhysicsShapeType2D.Capsule:
                        break;
                    case NgPhysicsShapeType2D.Triangle:
                        break;
                    case NgPhysicsShapeType2D.Rectangle:
                        break;
                    case NgPhysicsShapeType2D.Polygon:
                        break;
                    default:
                        break;
                }
            });
        }

        void GetCircleCollisions (List<NgCollider2D> colliders, List<NgCollision2D> collisions)
        {
            NgCircle2D circle = new (m_PhysicsShape2D.Vertices[0], m_PhysicsShape2D.Radius);

            colliders.ForEach (collider =>
            {
                switch (collider.m_PhysicsShape2D.ShapeType)
                {
                    case NgPhysicsShapeType2D.Point:
                        GetCollision (circle, collider.m_PhysicsShape2D.Vertices[0], collisions);
                        break;
                    case NgPhysicsShapeType2D.Circle:
                        GetCollision (circle, new NgCircle2D (collider.m_PhysicsShape2D.Vertices[0], collider.m_PhysicsShape2D.Radius), collisions);
                        break;
                    case NgPhysicsShapeType2D.Line:
                        break;
                    case NgPhysicsShapeType2D.Capsule:
                        break;
                    case NgPhysicsShapeType2D.Triangle:
                        break;
                    case NgPhysicsShapeType2D.Rectangle:
                        break;
                    case NgPhysicsShapeType2D.Polygon:
                        break;
                    default:
                        break;
                }
            });
        }

        void GetCollision (Vector2 lhs, Vector2 rhs, List<NgCollision2D> collisions)
        {
            if (NgPhysics2D.Overlaps (lhs, rhs))
            {
                collisions.Add (new NgCollision2D ());
            }
        }

        void GetCollision (Vector2 point, NgCircle2D circle, List<NgCollision2D> collisions)
        {
            if (NgPhysics2D.Overlaps (point, circle))
            {
                collisions.Add (new NgCollision2D ());
            }
        }

        void GetCollision (NgCircle2D circle, Vector2 point, List<NgCollision2D> collisions)
        {
            if (NgPhysics2D.Overlaps (circle, point))
            {
                collisions.Add (new NgCollision2D ());
            }
        }

        void GetCollision (NgCircle2D lhs, NgCircle2D rhs, List<NgCollision2D> collisions)
        {
            if (NgPhysics2D.Overlaps (lhs, rhs))
            {
                collisions.Add (new NgCollision2D ());
            }
        }
    }
}
