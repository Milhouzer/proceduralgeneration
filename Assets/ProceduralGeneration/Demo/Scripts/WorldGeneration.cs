using System.Collections.Generic;
using Milhouzer.MarchingSquares;
using UnityEngine;

namespace Milhouzer.ProceduralGeneration.Demo
{
    public class WorldGeneration : MonoBehaviour
    {
        public TilesLookupTable lookupTable;
        public int SizeX = 64;
        public int SizeY = 64;
        public float Scale = 0.5f;
        public float Threshold;

        private void Awake() 
        {
            // We don't really optimize for now, here we iterate through the array twice which give O(2n²) ~ O(n²) complexity.
            uint[,] vals = MarchingSquares.MarchSquares(SizeX, SizeY, Scale, Threshold);
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    GameObject go = Instantiate(lookupTable[(int)vals[i,j]], new Vector3(i,0f, j), Quaternion.identity);
                    go.name += i + ", " + j;
                }
            }
        }
    }
}
