using UnityEngine;

namespace Milhouzer.ProceduralGeneration
{
    /// <summary>
    /// 
    /// </summary>
    public static class MarchingSquares
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static uint[,] MarchSquares(int x, int y)
        {
            return MarchSquares(x, y, 1f, 0.5f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static uint[,] MarchSquares(int x, int y, float scale)
        {
            return MarchSquares(x, y, scale, 0.5f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scale"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static uint[,] MarchSquares(int x, int y, float scale, float threshold)
        {
            uint[,] values = new uint[x,y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    float a = Mathf.PerlinNoise((i - 0.5f) * scale, (j - 0.5f) * scale); // Bottom left
                    float b = Mathf.PerlinNoise((i + 0.5f) * scale, (j - 0.5f) * scale); // Bottom right
                    float c = Mathf.PerlinNoise((i + 0.5f) * scale, (j + 0.5f) * scale); // Top right
                    float d = Mathf.PerlinNoise((i - 0.5f) * scale, (j + 0.5f) * scale); // Top left

                    values[i,j] = MarchSquare(a, b, c, d, threshold);
                    Debug.Log("(" + a + "," + b + "," + c + "," + d + ")" + values[i,j]);
                }
            }

            return values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scale"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static uint[,] MarchSquares(Vector2 offset, int x, int y, float scale, float threshold)
        {
            uint[,] values = new uint[x,y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    float a = Mathf.PerlinNoise((offset.x + i - 0.5f) * scale, (offset.y + j - 0.5f) * scale); // Bottom left
                    float b = Mathf.PerlinNoise((offset.x + i + 0.5f) * scale, (offset.y + j - 0.5f) * scale); // Bottom right
                    float c = Mathf.PerlinNoise((offset.x + i + 0.5f) * scale, (offset.y + j + 0.5f) * scale); // Top right
                    float d = Mathf.PerlinNoise((offset.x + i - 0.5f) * scale, (offset.y + j + 0.5f) * scale); // Top left

                    values[i,j] = MarchSquare(a, b, c, d, threshold);
                    Debug.Log("(" + a + "," + b + "," + c + "," + d + ")" + values[i,j]);
                }
            }

            return values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scale"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static uint MarchSquare(float x, float y, float scale, float threshold)
        {

            float a = Mathf.PerlinNoise((x - 0.5f) * scale, (y - 0.5f) * scale); // Bottom left
            float b = Mathf.PerlinNoise((x + 0.5f) * scale, (y - 0.5f) * scale); // Bottom right
            float c = Mathf.PerlinNoise((x + 0.5f) * scale, (y + 0.5f) * scale); // Top right
            float d = Mathf.PerlinNoise((x - 0.5f) * scale, (y + 0.5f) * scale); // Top left

            return MarchSquare(a, b, c, d, threshold);
        }

        /// <summary>
        /// March square main function
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static uint MarchSquare(float a, float b, float c, float d, float threshold)
        {
            uint v = 0b_0000;
            
            if(a > threshold)
                v |= 0b_0001;

            if(b > threshold)
                v |= 0b_0010;

            if(c > threshold)
                v |= 0b_0100;

            if(d > threshold)
                v |= 0b_1000;

            return v;
        }
    }
}
