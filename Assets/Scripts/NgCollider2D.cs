namespace ProjectNothing
{
    public class NgCollider2D : NgComponent
    {
        static int ms_ID = 0;

        readonly int m_ID;
        int m_LayerMask;
        NgBound2D m_Bound;
        bool m_IsDynamic;
        NgBound2D m_SweepBound;

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

        public bool IsDynamic
        {
            get => m_IsDynamic;
            set => m_IsDynamic = value;
        }

        public NgBound2D SweepBound
        {
            get => m_IsDynamic ? m_SweepBound : m_Bound;
            set
            {
                if (m_IsDynamic)
                {
                    m_SweepBound = value;
                }
                else
                {
                    m_Bound = value;
                }
            }
        }

        public NgCollider2D ()
        {
            m_ID = ms_ID++;
            m_LayerMask = 0;
            m_IsDynamic = false;
            m_Bound = new NgBound2D ();
            m_SweepBound = new NgBound2D ();
        }
    }
}
