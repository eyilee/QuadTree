namespace ProjectNothing
{
    public class NgComponent
    {
        protected NgGameObject2D m_GameObject;

        public NgGameObject2D GameObject => m_GameObject;

        public void OnConstruct (NgGameObject2D gameObject)
        {
            m_GameObject = gameObject;
        }
    }
}
