using UnityEngine;

namespace ProjectNothing
{
    public class NgCircle2D
    {
        Vector2 m_Center;
        float m_Radius;

        public Vector2 Center
        {
            get => m_Center;
            set => m_Center = value;
        }

        public float Radius
        {
            get => m_Radius;
            set => m_Radius = value;
        }

        public NgCircle2D (Vector2 center, float radius)
        {
            m_Center = center;
            m_Radius = radius;
        }
    }
}