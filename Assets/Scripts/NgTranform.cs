using UnityEngine;

namespace ProjectNothing
{
    public class NgTransform : NgComponent
    {
        Vector2 m_Position;
        float m_Rotation;
        Vector2 m_Scale;

        public Vector2 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public float Rotation
        {
            get { return m_Rotation; }
            set { m_Rotation = value; }
        }

        public Vector2 Scale
        {
            get { return m_Scale; }
            set { m_Scale = value; }
        }

        public NgTransform (NgGameObject gameObject) : base (gameObject)
        {
        }
    }
}
