using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public readonly struct NgLine2D
    {
        readonly Vector2[] m_Vertices;

        public readonly Vector2 this[int index] => m_Vertices[index];
        public readonly Vector2 Start => m_Vertices[0];
        public readonly Vector2 End => m_Vertices[1];
        public readonly Vector2 Vector => m_Vertices[1] - m_Vertices[0];

        public NgLine2D (Vector2 v1, Vector2 v2)
        {
            m_Vertices = new Vector2[2] { v1, v2 };
        }
    }

    public readonly struct NgCircle2D
    {
        readonly Vector2 m_Center;
        readonly float m_Radius;

        public readonly Vector2 Center => m_Center;
        public readonly float Radius => m_Radius;

        public NgCircle2D (Vector2 center, float radius)
        {
            m_Center = center;
            m_Radius = radius;
        }
    }

    public readonly struct NgCapsule2D
    {
        readonly Vector2[] m_Vertices;
        readonly float m_Radius;

        public readonly Vector2 this[int index] => m_Vertices[index];
        public readonly Vector2 Start => m_Vertices[0];
        public readonly Vector2 End => m_Vertices[1];
        public readonly Vector2 Vector => m_Vertices[1] - m_Vertices[0];
        public readonly float Radius => m_Radius;

        public NgCapsule2D (Vector2 v1, Vector2 v2, float radius)
        {
            m_Vertices = new Vector2[2] { v1, v2 };
            m_Radius = radius;
        }
    }

    public readonly struct NgTriangle2D
    {
        readonly Vector2[] m_Vertices;

        public readonly Vector2 this[int index] => m_Vertices[index];

        public readonly NgLine2D Line (int index)
        {
            return new NgLine2D (m_Vertices[index % 3], m_Vertices[(index + 1) % 3]);
        }

        public NgTriangle2D (Vector2 v1, Vector2 v2, Vector2 v3)
        {
            m_Vertices = new Vector2[3] { v1, v2, v3 };
        }
    }

    public class NgPhysics2D
    {
        /*  Shapes
         *
         *  Point, Circle, Line, Capsule, Triangle, Rectangle, Polygon, Ray
         */

        /*  Functions
         *
         *  Vector2 Closest (A, B)
         *      Returns closest point on shape A to shape B
         *  
         *  bool Contains (A, B)
         *      Returns whether shape A completely contains shape B including boundaries
         *  
         *  bool Overlaps (A, B)
         *      Returns whether shape A overlaps shape B including boundaries
         *  
         *  List<Vector2> Intersects (A, B)
         *      Returns a list of points where shape A boundary intersects with shape B boundary
         *
         */

        /*
         *  Common
         */
        public static bool Overlaps<T1, T2> (T1 lhs, T2 rhs)
        {
            return Overlaps (lhs, rhs);
        }

        /*
         *  Point
         */
        public static bool Overlaps (Vector2 lhs, Vector2 rhs)
        {
            return (lhs.x - rhs.x) < float.Epsilon && (lhs.y - rhs.y) < float.Epsilon;
        }

        public static bool Overlaps (Vector2 point, NgCircle2D circle)
        {
            return Contains (circle, point);
        }

        /*
         *  Circle
         */
        public static Vector2 Closest (NgCircle2D circle, Vector2 point)
        {
            Vector2 vector = point - circle.Center;
            return circle.Center + vector * (vector.magnitude / circle.Radius);
        }

        public static bool Contains (NgCircle2D circle, Vector2 point)
        {
            return (point - circle.Center).magnitude <= circle.Radius;
        }

        public static bool Overlaps (NgCircle2D circle, Vector2 point)
        {
            return Contains (circle, point);
        }

        public static bool Overlaps (NgCircle2D lhs, NgCircle2D rhs)
        {
            return (lhs.Center - rhs.Center).magnitude <= lhs.Radius + rhs.Radius;
        }

        public static List<Vector2> Intersects (NgCircle2D circle, Vector2 point)
        {
            List<Vector2> list = new ();

            if (Contains (circle, point))
            {
                list.Add (point);
            }

            return list;
        }

        /*
         *  Line
         */
        public static Vector2 Closest (NgLine2D line, Vector2 point)
        {
            return line.Start + line.Vector * Mathf.Clamp01 (Vector2.Dot (line.Vector, point) / line.Vector.magnitude);
        }

        public static bool Contains (NgLine2D line, Vector2 point)
        {
            float z = Vector3.Cross (line.Start - point, line.End - point).z;
            if (z < float.Epsilon)
            {
                Vector2 vector = line.Vector;
                float factor = Mathf.Clamp01 (Vector2.Dot (vector, point) / vector.magnitude);
                return factor >= 0 && factor <= 1;
            }
            return false;
        }

        public static bool Overlaps (NgLine2D line, Vector2 point)
        {
            return Contains (line, point);
        }

        public static List<Vector2> Intersects (NgLine2D line, Vector2 point)
        {
            List<Vector2> list = new ();

            if (Contains (line, point))
            {
                list.Add (point);
            }

            return list;
        }

        /*
         *  Triangle
         */
        public static Vector2 Closest (NgTriangle2D triangle, Vector2 point)
        {
            Vector2 v1 = Closest (new NgLine2D (triangle[0], triangle[1]), point);
            float d1 = Vector2.Distance (v1, point);

            Vector2 v2 = Closest (new NgLine2D (triangle[1], triangle[2]), point);
            float d2 = Vector2.Distance (v2, point);

            Vector2 v3 = Closest (new NgLine2D (triangle[2], triangle[0]), point);
            float d3 = Vector2.Distance (v3, point);

            if (d1 <= d2 && d1 <= d3)
            {
                return v1;
            }
            else if (d2 <= d1 && d2 <= d3)
            {
                return v2;
            }
            else
            {
                return v3;
            }
        }

        public static bool Contains (NgTriangle2D triangle, Vector2 point)
        {
            bool z1 = Vector3.Cross (triangle[0] - point, triangle[1] - point).z >= 0;
            bool z2 = Vector3.Cross (triangle[1] - point, triangle[2] - point).z >= 0;
            bool z3 = Vector3.Cross (triangle[2] - point, triangle[0] - point).z >= 0;
            return z1 == z2 && z2 == z3;
        }

        public static bool Overlaps (NgTriangle2D triangle, Vector2 point)
        {
            return Contains (triangle, point);
        }

        public static List<Vector2> Intersects (NgTriangle2D triangle, Vector2 point)
        {
            List<Vector2> list = new ();

            for (int i = 0; i < 3; i++)
            {
                if (Contains (triangle.Line (i), point))
                {
                    list.Add (point);
                    break;
                }
            }

            return list;
        }

        /*
         *  ConvexHull
         */
        public static List<Vector2> GenerateConvexHull (List<Vector2> points)
        {
            if (points == null || points.Count == 0)
            {
                return new List<Vector2> ();
            }

            // Graham scan
            List<Vector2> list = new (points);

            list.Sort ((lhs, rhs) => lhs.y != rhs.y ? lhs.y.CompareTo (rhs.y) : lhs.x.CompareTo (rhs.x));

            if (points.Count > 2)
            {
                Vector2 origin = list[0];
                list.Sort (1, list.Count - 1, Comparer<Vector2>.Create ((lhs, rhs) =>
                {
                    Vector2 v1 = lhs - origin;
                    Vector2 v2 = rhs - origin;
                    float crossProduct = (v1.x * v2.y) - (v1.y * v2.x);
                    return crossProduct != 0.0f ? -crossProduct.CompareTo (0.0f) : -v1.magnitude.CompareTo (v2.magnitude);
                }));
            }

            List<Vector2> vertices = new ();
            for (int i = 0; i <= list.Count; i++)
            {
                Vector2 nextVertex = i == list.Count ? list[0] : list[i];

                if (vertices.Count < 2)
                {
                    vertices.Add (nextVertex);
                    continue;
                }

                while (vertices.Count >= 2)
                {
                    Vector2 v1 = vertices[^1] - vertices[^2];
                    Vector2 v2 = nextVertex - vertices[^1];

                    float crossProduct = (v1.x * v2.y) - (v1.y * v2.x);
                    if (crossProduct < 0.0f)
                    {
                        vertices.RemoveAt (vertices.Count - 1);
                        continue;
                    }

                    vertices.Add (nextVertex);

                    if (crossProduct == 0.0f && Vector2.Dot (v1, v2) < 0.0f)
                    {
                        vertices.Reverse (vertices.Count - 2, 2);
                    }

                    break;
                }
            }

            // Remove repeated origin point
            vertices.RemoveAt (vertices.Count - 1);

            return vertices;
        }
    }
}
