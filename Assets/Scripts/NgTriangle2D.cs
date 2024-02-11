using UnityEngine;

namespace ProjectNothing
{
    public class NgTriangle2D
    {
        readonly Vector2[] m_Vertices = new Vector2[3];

        public Vector2 this[int index]
        {
            get { return m_Vertices[index]; }
            set { m_Vertices[index] = value; }
        }

        public NgLine2D Line (int index)
        {
            return new NgLine2D (m_Vertices[index % 3], m_Vertices[(index + 1) % 3]);
        }

        public NgTriangle2D (Vector2 v1, Vector2 v2, Vector2 v3)
        {
            m_Vertices[0] = v1;
            m_Vertices[1] = v2;
            m_Vertices[2] = v3;
        }
    }
}