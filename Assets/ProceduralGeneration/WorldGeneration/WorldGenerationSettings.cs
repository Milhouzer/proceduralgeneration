using Milhouzer.ProceduralGeneration;
using ProceduralToolkit.FastNoiseLib;
using UnityEngine;

namespace Milhouzer.WorldGeneration
{
    [CreateAssetMenu(fileName = "WorldGenerationSettings", menuName = "Milhouzer/WorldGeneration/WorldGenerationSettings", order = 0)]
    public class WorldGenerationSettings : ScriptableObject 
    {
        [SerializeField]
        public FastNoise Noise;

        [SerializeField]
        public int SEED = 1337;
        public int CHUNK_SIZE = 32;
        public float CHUNK_HEIGHT = 1;
        public float SCALE = 0.05f;
        public float THRESHOLD = 0.55f;
        public float VEGETATION_PROBABILITY = 0.5f;
        public TilesMeshLookupTable TILES_BOT_PARTS_LOOKUP_TABLE;
        public TilesMeshLookupTable TILES_MID_PARTS_LOOKUP_TABLE;
        public TilesMeshLookupTable TILES_TOP_PARTS_LOOKUP_TABLE;
        public TilesMeshLookupTable VEGETATION_LOOKUP_TABLE;

        public GameObject CHUNK_PREVIEW_MODEL;
        public Material[] TILE_MATERIALS;
    }
}