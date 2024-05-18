using System;
using System.Collections;
using System.Collections.Generic;
using ProceduralToolkit.FastNoiseLib;
using UnityEngine;

namespace Milhouzer.ProceduralGeneration.WorldGeneration
{
    public class GridPerlinWorm
    {
        /// <TODO>
        /// Use this noise instead of Mathf.PerlinNoise
        /// </TODO>
        public FastNoise Noise { get; private set; }

        GridPerlinWormSettings Settings;

        private Vector2 currentPosition;
        private Vector2 currentDirection;
        private float switchDirection = 1f;

        private int count = 0;
        private int currentLength = 0;

        public GridPerlinWorm(GridPerlinWormSettings settings)
        {
            Settings = settings;
            currentDirection = GetStartingDirection();
        }

        public Vector2 MoveNext()
        {
            currentLength++;
            return Settings.Converge ? MoveTowardsConvergencePoint() : Move();
        }
        
        public Vector2 MoveTowardsConvergencePoint()
        {
            Vector2 convergeDirection = (Settings.ConvergencePoint - currentPosition).normalized;

            Vector2 endDirection = (GetNoiseDirection() * (1 - Settings.Weight)  + convergeDirection * Settings.Weight ).normalized;

            currentPosition += endDirection;
            return currentPosition;
        }

        public Vector2 Move()
        {
            Vector2 direction = GetNoiseDirection();
            currentPosition += direction;
            return currentPosition;
        }

        private static float NextFloat(System.Random random)
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            // choose -149 instead of -126 to also generate subnormal floats (*)
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }


        private Vector2 GetStartingDirection()
        {
            float theta = UnityEngine.Random.Range(-1f, 1f);
            theta = RangeMap(theta, -1f, 1f, 0, Settings.Range);
            return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));;

        }

        private Vector2 GetNoiseDirection()
        {
            float theta = SumNoise(currentPosition.x, currentPosition.y);
            theta = RangeMap(theta, 0, 1, - switchDirection * Settings.Range, switchDirection * Settings.Range);
            currentDirection = (Quaternion.AngleAxis(theta, Vector3.forward) * currentDirection).normalized;

            // Reset direction periodically to prevent drift
            if (Settings.SwitchDirection)
            {
                if(count % Settings.SwitchDirectionPeriod == 0)
                {
                    switchDirection *= -1f;
                    count -= Settings.SwitchDirectionPeriod;
                }
                
                count++;
            }

            return currentDirection;
        }
        

        public float SumNoise(float x, float y)
        {
            float amplitude = 1;
            float frequency = Settings.NoiseFrequency;
            float noiseSum = 0;
            float amplitudeSum = 0;
            for (int i = 0; i < Settings.Octaves; i++)
            {
                /// <TODO>
                /// 
                /// </TODO>
                noiseSum += amplitude * Mathf.PerlinNoise(currentLength * frequency, 0.5f);
                amplitudeSum += amplitude;
                amplitude *= Settings.Persistance;
                frequency *= 2;

            }
            return noiseSum / amplitudeSum; // set range back to 0-1

        }

        public float RangeMap(float inputValue, float inMin, float inMax, float outMin, float outMax)
        {
            return outMin + (inputValue - inMin) * (outMax - outMin) / (inMax - inMin);
        }
    }
}
