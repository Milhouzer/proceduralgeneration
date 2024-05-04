using System.Collections;
using System.Collections.Generic;
using Milhouzer.ProceduralGeneration;
using UnityEngine;

namespace Milhouzer.WorldGeneration
{
    public class World : MonoBehaviour
    {
        public static World Singleton;

        [SerializeField]
        private WorldSettings generationSettings;
        public WorldSettings GenerationSettings => generationSettings;

        private Vector3 currentPosition = Vector3.zero;
        private ChunkPreview chunkPreview;

        private void Awake() 
        {
            Singleton = this;

            chunkPreview = new ChunkPreview(generationSettings.CHUNK_SIZE, currentPosition);
        }

        private void Update() 
        {
            bool inputChanged = false;

            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentPosition += new Vector3(-1, 0, 0);
                inputChanged = true;
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentPosition += new Vector3(1, 0, 0);
                inputChanged = true;
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentPosition += new Vector3(0, 0, 1);
                inputChanged = true;
            }
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentPosition += new Vector3(0, 0, -1);
                inputChanged = true;
            }
            
            if(Input.GetKeyDown(KeyCode.Return))
            {
                Chunk chunk = WorldMaker.MakeChunk(generationSettings.CHUNK_SIZE, currentPosition * generationSettings.CHUNK_SIZE);
                chunk.Load();
            }

            if(inputChanged)
            {
                DrawChunkPreview();
            }
        }

        private void DrawChunkPreview()
        {
            if(chunkPreview != null)
            {
                chunkPreview.Unload();
            }

            chunkPreview = new ChunkPreview(generationSettings.CHUNK_SIZE, currentPosition * generationSettings.CHUNK_SIZE);
            chunkPreview.Load();
        }
    }

    public static class WorldMaker
    {
        public static Chunk MakeChunk(int size, Vector3 location)
        {
            Chunk chunk = new Chunk(size, location);

            return chunk;
        }
    }

    [System.Serializable]
    public class WorldSettings
    {
        public int CHUNK_SIZE = 32;
        public float SCALE = 0.05f;
        public float THRESHOLD = 0.55f;
        public Vector3 WORLD_CENTER = Vector3.zero;
        public TilesLookupTable TILES_LOOKUP_TABLE;
        public TilesMeshLookupTable MESH_LOOKUP_TABLE;

        public GameObject CHUNK_PREVIEW_MODEL;
        public Material[] TILE_MATERIALS;
    }
}