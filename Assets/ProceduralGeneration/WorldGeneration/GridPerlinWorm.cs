using System;
using System.Collections;
using System.Collections.Generic;
using ProceduralToolkit.FastNoiseLib;
using UnityEngine;

namespace Milhouzer.ProceduralGeneration.WorldGeneration
{
    public class GridPerlinWorm
    {
        private FastNoise Noise;

        GridPerlinWormSettings Settings;

        private Vector2 currentPosition;
        private Vector2 currentDirection;
        private float switchDirection = 1f;

        private int count = 0;
        private int currentLength = 0;

        public GridPerlinWorm(GridPerlinWormSettings settings, FastNoiseSettings noiseSettings)
        {
            Settings = settings;
            Noise = new FastNoise(noiseSettings);
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

        private Vector2 GetStartingDirection()
        {
            float theta = UnityEngine.Random.Range(-1f, 1f);
            theta = RangeMap(theta, -1f, 1f, 0, Settings.Range);
            return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));;

        }

        private Vector2 GetNoiseDirection()
        {
            float theta = World.Singleton.RiverNoise.GetNoise(currentPosition.x, currentPosition.y);
            theta = RangeMap(theta, -1, 1, - switchDirection * Settings.Range, switchDirection * Settings.Range);
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

        public float RangeMap(float inputValue, float inMin, float inMax, float outMin, float outMax)
        {
            return outMin + (inputValue - inMin) * (outMax - outMin) / (inMax - inMin);
        }
    }
}
