namespace ProjectNothing
{
    public class NgCollider2D : NgComponent
    {
        int m_LayerMask;
        NgBoundingBox2D m_BoundingBox;

        public int LayerMask
        {
            get => m_LayerMask;
            set => m_LayerMask = value;
        }

        public NgBoundingBox2D BoundingBox
        {
            get => m_BoundingBox;
            set => m_BoundingBox = value;
        }

        public NgCollider2D (NgGameObject gameObject) : base (gameObject)
        {
        }
    }
}
