using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectNothing
{
    public class MeshFactory
    {
        public static Mesh CreateTriangle (List<Vector2> points)
        {
            Mesh mesh = new ();

            // Set vertices
            List<Vector3> vertices = points.ConvertAll (v => (Vector3)v);
            mesh.SetVertices (vertices);

            // Set triangles
            List<int> triangles = ReorderIndexes (vertices, 0, 1, 2).ToList ();
            mesh.SetTriangles (triangles, 0);

            mesh.RecalculateBounds ();
            mesh.RecalculateNormals ();
            mesh.RecalculateTangents ();

            // Set uvs
            List<Vector2> uvs = CalculateUVs (vertices, mesh.bounds);
            mesh.SetUVs (0, uvs);

            return mesh;
        }

        public static Mesh CreateRectangle (Vector2 size)
        {
            float x = size.x * 0.5f;
            float y = size.y * 0.5f;

            Mesh mesh = new ();

            // Set vertices
            Vector3[] vertices = new Vector3[]
            {
                new (x, y, 0f),
                new (-x, y, 0f),
                new (-x, -y, 0f),
                new (x, -y, 0f)
            };
            mesh.SetVertices (vertices);

            // Set triangles
            int[] triangles = new int[]
            {
                0, 3, 1,
                1, 3, 2
            };
            mesh.SetTriangles (triangles, 0);

            mesh.RecalculateBounds ();
            mesh.RecalculateNormals ();
            mesh.RecalculateTangents ();

            // Set uvs
            Vector2[] uvs = new Vector2[]
            {
                new (1f, 1f),
                new (0f, 1f),
                new (0f, 0f),
                new (1f, 0f)
            };
            mesh.SetUVs (0, uvs);

            return mesh;
        }

        public static Mesh CreatePolygon (List<Vector2> points)
        {
            Mesh mesh = new ();

            // Set vertices
            List<Vector3> vertices = points.ConvertAll (v => (Vector3)v);
            mesh.SetVertices (vertices);

            // Set triangles
            List<int> triangles = new ();

            List<int> indexes = Enumerable.Range (0, vertices.Count).ToList ();
            while (indexes.Count > 3)
            {
                int earIndex = FindEarIndex (vertices, indexes);
                if (earIndex == -1)
                {
                    break;
                }

                int i0 = indexes[(earIndex - 1 + indexes.Count) % indexes.Count];
                int i1 = indexes[earIndex];
                int i2 = indexes[(earIndex + 1) % indexes.Count];
                triangles.AddRange (ReorderIndexes (vertices, i0, i1, i2));

                indexes.RemoveAt (earIndex);
            }

            triangles.AddRange (ReorderIndexes (vertices, indexes[0], indexes[1], indexes[2]));

            mesh.SetTriangles (triangles, 0);

            mesh.RecalculateBounds ();
            mesh.RecalculateNormals ();
            mesh.RecalculateTangents ();

            // Set uvs
            List<Vector2> uvs = CalculateUVs (vertices, mesh.bounds);
            mesh.SetUVs (0, uvs);

            return mesh;

            static int FindEarIndex (List<Vector3> vertices, List<int> indexes)
            {
                for (int index = 0; index < indexes.Count; index++)
                {
                    int i0 = indexes[(index - 1 + indexes.Count) % indexes.Count];
                    int i1 = indexes[index];
                    int i2 = indexes[(index + 1) % indexes.Count];

                    // Vertices must be ordered by clock-wise
                    float z = Vector3.Cross (vertices[i1] - vertices[i0], vertices[i2] - vertices[i1]).z;
                    if (z <= 0)
                    {
                        continue;
                    }

                    NgTriangle2D triangle = new (vertices[i0], vertices[i1], vertices[i2]);

                    bool contains = false;
                    for (int other = 0; other < indexes.Count; other++)
                    {
                        int io = indexes[other];
                        if (io == i0 || io == i1 || io == i2)
                        {
                            continue;
                        }

                        if (NgPhysics2D.Contains (triangle, vertices[io]))
                        {
                            contains = true;
                            break;
                        }
                    }

                    if (!contains)
                    {
                        return index;
                    }
                }

                return -1;
            }
        }

        static int[] ReorderIndexes (List<Vector3> points, int i0, int i1, int i2)
        {
            float z = Vector3.Cross (points[i1] - points[i0], points[i2] - points[i0]).z;
            if (z <= 0f)
            {
                return new int[] { i0, i1, i2 };
            }
            else
            {
                return new int[] { i0, i2, i1 };
            }
        }

        static List<Vector2> CalculateUVs (List<Vector3> vertices, Bounds bounds)
        {
            float minX = bounds.min.x;
            float maxX = bounds.max.x;
            float minY = bounds.min.y;
            float maxY = bounds.max.y;
            return vertices.ConvertAll (v => new Vector2 (Mathf.InverseLerp (minX, maxX, v.x), Mathf.InverseLerp (minY, maxY, v.y))).ToList ();
        }
    }
}
