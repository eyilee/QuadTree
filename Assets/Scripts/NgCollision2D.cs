using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectNothing
{
    public class NgCollision2D
    {
        NgLayerMask m_LayerMask;

        public NgLayerMask LayerMask
        {
            get => m_LayerMask;
            set => m_LayerMask = value;
        }
    }
}
