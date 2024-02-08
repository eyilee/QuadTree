using System.Collections.Generic;
using UnityEngine;

namespace ProjectNothing
{
    public class NgPhysics2D
    {
        public static bool Contains (NgTriangle2D triangle, Vector2 point)
        {
            bool z1 = Vector3.Cross (triangle[0] - point, triangle[1] - point).z > 0;
            bool z2 = Vector3.Cross (triangle[1] - point, triangle[2] - point).z > 0;
            bool z3 = Vector3.Cross (triangle[2] - point, triangle[0] - point).z > 0;
            return z1 == z2 && z2 == z3;
        }

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
