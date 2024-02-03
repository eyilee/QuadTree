namespace ProjectNothing
{
    public abstract class NgComponent
    {
        readonly NgGameObject m_GameObject;
        readonly NgTransform m_Transform;

        public NgGameObject GameObject => m_GameObject;
        public NgTransform Transform => m_Transform;

        public NgComponent (NgGameObject gameObject)
        {
            m_GameObject = gameObject;
            m_Transform = m_GameObject.Transform;
        }
    }
}
