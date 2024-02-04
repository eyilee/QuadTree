using System;
using System.Collections.Generic;

namespace ProjectNothing
{
    public class NgGameObject
    {
        readonly Dictionary<Type, NgComponent> m_Components = new ();
        readonly NgGameObject m_GameObject;
        readonly NgTransform m_Transform;

        public NgGameObject GameObject => m_GameObject;
        public NgTransform Transform => m_Transform;

        public NgGameObject ()
        {
            m_GameObject = this;
            m_Transform = AddComponent<NgTransform> ();
        }

        public TComponent AddComponent<TComponent> () where TComponent : NgComponent
        {
            Type type = typeof (TComponent);
            if (!m_Components.ContainsKey (type))
            {
                TComponent component = Activator.CreateInstance (type, new object[] { this }) as TComponent;
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
