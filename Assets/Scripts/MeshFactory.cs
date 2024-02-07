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
                new Vector3 (x, y, 0f),
                new Vector3 (-x, y, 0f),
                new Vector3 (-x, -y, 0f),
                new Vector3 (x, -y, 0f)
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
                new Vector2 (1f, 1f),
                new Vector2 (0f, 1f),
                new Vector2 (0f, 0f),
                new Vector2 (1f, 0f)
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
            while (indexes.Count > 0)
            {
                int earIndex = FindEarIndex (vertices, indexes);
                if (earIndex == -1)
                {
                    break;
                }

                triangles.AddRange (ReorderIndexes (vertices, (earIndex + indexes.Count - 1) % indexes.Count, earIndex, (earIndex + 1) % indexes.Count));

                indexes.RemoveAt (earIndex);
            }

            mesh.RecalculateBounds ();
            mesh.RecalculateNormals ();
            mesh.RecalculateTangents ();

            // Set uvs
            List<Vector2> uvs = CalculateUVs (vertices, mesh.bounds);
            mesh.SetUVs (0, uvs);

            return mesh;

            static int FindEarIndex (List<Vector3> vertices, List<int> indexes)
            {
                // TODO: Ear clipping
                return -1;
            }
        }

        static int[] ReorderIndexes (List<Vector3> points, int i0, int i1, int i2)
        {
            float z = Vector3.Cross (points[i1] - points[i0], points[i2] - points[i0]).z;
            if (z < 0f)
            {
                return new int[] { i0, i1, i2 };
            }
            else if (z > 0f)
            {
                return new int[] { i0, i2, i1 };
            }
            else
            {
                return null;
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
