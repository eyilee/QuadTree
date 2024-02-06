﻿using UnityEngine;

namespace ProjectNothing
{
    public class RectangleMesh
    {
        public static Mesh CreateMesh (Vector2 size)
        {
            float x = size.x * 0.5f;
            float y = size.y * 0.5f;

            Mesh mesh = new ();

            Vector3[] verticies = new Vector3[]
            {
                new Vector3 (x, y, 0),
                new Vector3 (-x, y, 0),
                new Vector3 (-x, -y, 0),
                new Vector3 (x, -y, 0)
            };

            mesh.SetVertices (verticies);

            int[] triangles = new int[]
            {
                0, 3, 1,
                1, 3, 2
            };

            mesh.SetTriangles (triangles, 0);

            Vector2[] uvs = new Vector2[]
            {
                new Vector2 (1, 1),
                new Vector2 (0, 1),
                new Vector2 (0, 0),
                new Vector2 (1, 0)
            };

            mesh.SetUVs (0, uvs);

            mesh.RecalculateNormals ();
            mesh.RecalculateTangents ();

            return mesh;
        }
    }
}
