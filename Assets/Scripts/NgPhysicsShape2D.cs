using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public enum NgPhysicsShapeType2D
    {
        Point,
        Circle,
        Line,
        Capsule,
        Triangle,
        Rectangle,
        Polygon
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

        public void SetPolygon (List<Vector2> vertices)
        {
            m_ShapeType = NgPhysicsShapeType2D.Polygon;

            m_Vertices.Clear ();
            m_Vertices.AddRange (vertices);

            m_Radius = 0f;
        }

        public void SetRectangle (Vector2 center, Vector2 size, float angle)
        {
            m_ShapeType = NgPhysicsShapeType2D.Polygon;

            float sin = Mathf.Sin (angle * Mathf.Deg2Rad);
            float cos = Mathf.Cos (angle * Mathf.Deg2Rad);
            float sinX = sin * size.x;
            float cosX = cos * size.x;
            float sinY = sin * size.y;
            float cosY = cos * size.y;
            Vector2 v1 = new Vector2 (cosX - sinY, sinX + cosY) * 0.5f;
            Vector2 v2 = new Vector2 (cosX + sinY, sinX - cosY) * 0.5f;

            m_Vertices.Clear ();
            m_Vertices.Add (center + v1);
            m_Vertices.Add (center - v2);
            m_Vertices.Add (center - v1);
            m_Vertices.Add (center + v2);

            m_Radius = 0f;
        }
    }
}
