using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class NgGameObject2D
    {
        protected GameObject m_GameObject = null;
        protected Transform m_Transform = null;
        protected Dictionary<Type, NgComponent> m_Components = new ();

        public NgGameObject2D (GameObject gameObject)
        {
            m_GameObject = gameObject;
            m_Transform = m_GameObject.transform;
        }

        public TComponent AddComponent<TComponent> () where TComponent : NgComponent, new()
        {
            Type type = typeof (TComponent);
            if (!m_Components.ContainsKey (type))
            {
                TComponent component = new ();
                component.OnConstruct (this);
                if (m_Components.TryAdd (type, component))
                {
                    return component;
                }
            }

            return null;
        }

        public void RemoveComponent (Type type)
        {
            m_Components.Remove (type);
        }
    }
}
