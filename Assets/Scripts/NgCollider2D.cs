namespace ProjectNothing
{
    public class NgCollider2D
    {
        static int ms_ID = 0;

        readonly int m_ID;
        int m_LayerMask;
        NgBound2D m_Bound;

        public int ID => m_ID;

        public int LayerMask
        {
            get => m_LayerMask;
            set => m_LayerMask = value;
        }

        public NgBound2D Bound
        {
            get => m_Bound;
            set => m_Bound = value;
        }

        public NgCollider2D ()
        {
            m_ID = ms_ID++;
            m_LayerMask = 0;
        }
    }
}
