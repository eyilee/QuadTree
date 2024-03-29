﻿using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class NgQuadTree2D
    {
        int m_Depth;
        int m_Capacity;
        NgBoundingBox2D m_BoundingBox;

        readonly List<NgCollider2D> m_Colliders = new ();
        readonly List<NgQuadTree2D> m_QuadTrees = new ();
        readonly Queue<NgQuadTree2D> m_Queue = new ();

        public NgBoundingBox2D BoundingBox => m_BoundingBox;
        public Matrix4x4 ObjectToWorld => Matrix4x4.TRS (m_BoundingBox.Center, Quaternion.identity, m_BoundingBox.Size);
        public List<NgQuadTree2D> QuadTrees => m_QuadTrees;

        public NgQuadTree2D (int depth, int capacity, NgBoundingBox2D bound, Queue<NgQuadTree2D> queue = null)
        {
            m_Depth = depth;
            m_Capacity = capacity;
            m_BoundingBox = bound;
            m_Queue = queue ?? new Queue<NgQuadTree2D> ();
        }

        public void Reset ()
        {
            m_Colliders.Clear ();

            foreach (NgQuadTree2D quadTree in m_QuadTrees)
            {
                quadTree.Reset ();
                m_Queue.Enqueue (quadTree);
            }

            m_QuadTrees.Clear ();
        }

        public NgQuadTree2D Create (int depth, int capacity, NgBoundingBox2D bound)
        {
            if (m_Queue.TryDequeue (out NgQuadTree2D quadTree))
            {
                quadTree.m_Depth = depth;
                quadTree.m_Capacity = capacity;
                quadTree.m_BoundingBox = bound;
                return quadTree;
            }

            return new NgQuadTree2D (depth, capacity, bound, m_Queue);
        }

        public bool TryInsert (NgCollider2D collider)
        {
            if (!m_BoundingBox.Contains (collider.BoundingBox))
            {
                return false;
            }

            if (m_Colliders.Count < m_Capacity || m_Depth == 0)
            {
                m_Colliders.Add (collider);
                return true;
            }

            if (m_QuadTrees.Count == 0)
            {
                SubDivide ();
            }

            foreach (NgQuadTree2D quadTree in m_QuadTrees)
            {
                if (quadTree.TryInsert (collider))
                {
                    return true;
                }
            }

            m_Colliders.Add (collider);
            return true;
        }

        public void Insert (NgCollider2D collider)
        {
            m_Colliders.Add (collider);
        }

        void SubDivide ()
        {
            Vector2 extents = m_BoundingBox.Extents * 0.5f;
            Vector2 size = m_BoundingBox.Size * 0.5f;

            m_QuadTrees.Capacity = m_Capacity;
            m_QuadTrees.Add (Create (m_Depth - 1, m_Capacity, new NgBoundingBox2D (m_BoundingBox.Center + new Vector2 (extents.x, extents.y), size)));
            m_QuadTrees.Add (Create (m_Depth - 1, m_Capacity, new NgBoundingBox2D (m_BoundingBox.Center + new Vector2 (-extents.x, extents.y), size)));
            m_QuadTrees.Add (Create (m_Depth - 1, m_Capacity, new NgBoundingBox2D (m_BoundingBox.Center + new Vector2 (-extents.x, -extents.y), size)));
            m_QuadTrees.Add (Create (m_Depth - 1, m_Capacity, new NgBoundingBox2D (m_BoundingBox.Center + new Vector2 (extents.x, -extents.y), size)));
        }

        public void Query (NgCollider2D target, List<NgCollider2D> colliders)
        {
            if (target.BoundingBox.Intersects (m_BoundingBox))
            {
                foreach (NgCollider2D collider in m_Colliders)
                {
                    if ((target.LayerMask & collider.LayerMask) == 0)
                    {
                        continue;
                    }

                    if (!target.Equals (collider) && target.BoundingBox.Intersects (collider.BoundingBox))
                    {
                        colliders.Add (collider);
                    }
                }

                foreach (NgQuadTree2D quadTree in m_QuadTrees)
                {
                    quadTree.Query (target, colliders);
                }
            }
        }
    }
}
