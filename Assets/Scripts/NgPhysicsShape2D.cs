using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public enum NgPhysicsShapeType2D
    {
        Circle,
        Capsule,
        Polygon,
        Edges
    }

    public class NgPhysicsShape2D
    {
        NgPhysicsShapeType2D m_ShapeType;
        readonly List<Vector2> m_Vertices = new ();
        float m_Radius;
        Matrix4x4 m_ObjectToWorld;

        public NgPhysicsShapeType2D ShapeType
        {
            get { return m_ShapeType; }
            set { m_ShapeType = value; }
        }

        public float Radius
        {
            get { return m_Radius; }
            set { m_Radius = value; }
        }

        public List<Vector2> Vertices => m_Vertices;

        public Matrix4x4 ObjectToWorld
        {
            get { return m_ObjectToWorld; }
            set { m_ObjectToWorld = value; }
        }

        public NgPhysicsShape2D ()
        {
            m_Vertices = new List<Vector2> ();
        }

        public void SetRectangle (Vector2 center, Vector2 size, float angle, float radius = 0f)
        {
            m_ShapeType = NgPhysicsShapeType2D.Polygon;

            Vector2 rotated = (size * 0.5f).Rotate (angle);
            m_Vertices.Clear ();
            m_Vertices.Add (new Vector2 (center.x + rotated.x, center.y + rotated.y));
            m_Vertices.Add (new Vector2 (center.x - rotated.y, center.y + rotated.x));
            m_Vertices.Add (new Vector2 (center.x - rotated.x, center.y - rotated.y));
            m_Vertices.Add (new Vector2 (center.x + rotated.y, center.y - rotated.x));

            m_Radius = radius;
        }
    }
}
