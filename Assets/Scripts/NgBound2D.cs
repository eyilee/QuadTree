using UnityEngine;

namespace ProjectNothing
{
    public struct NgBound2D
    {
        Vector2 m_Center;
        Vector2 m_Extents;

        public Vector2 Center
        {
            readonly get => m_Center;
            set => m_Center = value;
        }

        public Vector2 Extents
        {
            readonly get => m_Extents;
            set => m_Extents = value;
        }

        public Vector2 Size
        {
            readonly get => m_Extents * 2f;
            set => m_Extents = value * 0.5f;
        }

        public readonly Vector2 Min => m_Center - m_Extents;
        public readonly Vector2 Max => m_Center + m_Extents;

        public NgBound2D (Vector2 center, Vector2 size)
        {
            m_Center = center;
            m_Extents = size * 0.5f;
        }

        public void SetMinMax (Vector2 min, Vector2 max)
        {
            m_Extents = (max - min) * 0.5f;
            m_Center = min + m_Extents;
        }

        public void Encapsulate (NgBound2D bound)
        {
            Encapsulate (bound.Min);
            Encapsulate (bound.Max);
        }

        public void Encapsulate (Vector2 point)
        {
            SetMinMax (Vector2.Min (Min, point), Vector2.Max (Max, point));
        }

        public readonly bool Contains (NgBound2D bound) => Contains (bound.Min) && Contains (bound.Max);

        public readonly bool Contains (Vector2 point) => Min.x <= point.x && Max.x >= point.x && Min.y <= point.y && Max.y >= point.y;

        public readonly bool Intersects (NgBound2D bound) => Min.x <= bound.Max.x && Max.x >= bound.Min.x && Min.y <= bound.Max.y && Max.y >= bound.Min.y;
    }
}
