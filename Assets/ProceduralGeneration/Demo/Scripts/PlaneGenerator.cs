using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Milhouzer.MarchingSquares
{
    /// <summary>
    /// Just a tesselated plane generator.
    /// </summary>
    public class PlaneGenerator : MonoBehaviour
    {
        List<Vector3> vertices;
        List<int> triangles;

        public Mesh plane;

        public float size;
        public int resolution;

        

        private void OnValidate() {
            GenerateAndAssign();
        }

        [ContextMenu("GenerateAndAssign")]
        public void GenerateAndAssign()
        {
            GeneratePlane(Vector2.one * size, resolution);
            AssignMesh();
        }

        void GeneratePlane(Vector2 size, int resolution)
        {
            vertices = new List<Vector3>();
            float xPerStep = size.x/resolution;
            float yPerStep = size.y/resolution;
            for (int y = 0; y < resolution+1; y++)
            {
                for (int x = 0; x < resolution +1; x++)
                {
                    vertices.Add(new Vector3(x * xPerStep, 0f, y * yPerStep));
                }
            }

            triangles = new List<int>();
            for (int row = 0; row < resolution; row++)
            {
                for(int column = 0; column < resolution; column++)
                {
                    int i = row * (resolution + 1) + column;

                    triangles.Add(i);
                    triangles.Add(i + resolution + 1);
                    triangles.Add(i + resolution + 2);
                    
                    triangles.Add(i);
                    triangles.Add(i + resolution + 2);
                    triangles.Add(i + 1);
                }
            }

        }

        void AssignMesh()
        {
            plane.Clear();
            plane.vertices = vertices.ToArray();
            plane.triangles = triangles.ToArray();
            plane.RecalculateNormals();
        }
    }
}
