using UnityEngine;

namespace ProjectNothing
{
    public class NgLine2D
    {
        readonly Vector2[] m_Vertices = new Vector2[2];

        public Vector2 this[int index]
        {
            get { return m_Vertices[index]; }
            set { m_Vertices[index] = value; }
        }

        public Vector2 Start => m_Vertices[0];
        public Vector2 End => m_Vertices[1];
        public Vector2 Vector => m_Vertices[1] - m_Vertices[0];

        public NgLine2D (Vector2 v1, Vector2 v2)
        {
            m_Vertices[0] = v1;
            m_Vertices[1] = v2;
        }
    }
}