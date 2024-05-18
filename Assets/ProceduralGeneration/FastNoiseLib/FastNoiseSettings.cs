
using UnityEngine;
using static ProceduralToolkit.FastNoiseLib.FastNoise;

#if FN_USE_DOUBLES
using FN_DECIMAL = System.Double;
#else
using FN_DECIMAL = System.Single;
#endif

namespace ProceduralToolkit.FastNoiseLib
{

    [CreateAssetMenu(fileName = "FastNoiseSettings", menuName = "ProceduralGeneration/FastNoiseLib/FastNoiseSettings", order = 0)]
    public class FastNoiseSettings : ScriptableObject 
    {
        public int seed = 1337;
        public FN_DECIMAL frequency = (FN_DECIMAL)0.01;
        public Interp interp = Interp.Quintic;
        public NoiseType noiseType = NoiseType.Simplex;

        public int octaves = 3;
        public FN_DECIMAL lacunarity = (FN_DECIMAL)2.0;
        public FN_DECIMAL gain = (FN_DECIMAL)0.5;
        public FractalType fractalType = FractalType.FBM;

        public FN_DECIMAL fractalBounding;

        public CellularDistanceFunction cellularDistanceFunction = CellularDistanceFunction.Euclidean;
        public CellularReturnType cellularReturnType = CellularReturnType.CellValue;
        public FastNoise cellularNoiseLookup = null;
        public int cellularDistanceIndex0 = 0;
        public int cellularDistanceIndex1 = 1;
        public float cellularJitter = 0.45f;

        public FN_DECIMAL gradientPerturbAmp = (FN_DECIMAL)1.0;

        public delegate void PropertyChangeEvent();
        public event PropertyChangeEvent OnPropertyChanged;
        private void OnValidate() {
            OnPropertyChanged?.Invoke();
        }
    }
}