using System.Collections;
using System.Collections.Generic;
using Milhouzer.ProceduralGeneration;
using Milhouzer.ProceduralGeneration.WorldGeneration;
using ProceduralToolkit.FastNoiseLib;
using UnityEngine;

namespace Milhouzer.WorldGeneration
{
    public class World : MonoBehaviour
    {
        public static World Singleton;

        [SerializeField]
        private WorldGenerationSettings generationSettings;
        public WorldGenerationSettings GenerationSettings => generationSettings;

        private Vector3 currentPosition = Vector3.zero;
        private ChunkPreview chunkPreview;

        public FastNoise groundNoise;

        private void Awake() 
        {
            groundNoise = new FastNoise(1337);
            Singleton = this;
        }
        void Start()
        {
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

            if(Input.GetKeyUp(KeyCode.W))
            {
                StartProcess();
            }

            if(inputChanged)
            {
                DrawChunkPreview();
            }
        }
        

        public GridPerlinWormSettings wormSettings;
        GridPerlinWorm worm;
        Coroutine wormMaker;
        Transform wormParent;

        public void StartProcess()
        {
            if(wormMaker != null)
            {
                StopCoroutine(wormMaker);
                wormMaker = null;
                Destroy(wormParent.gameObject);
            }else
            {
                GameObject go = new GameObject();
                wormParent = go.transform;
                worm = new GridPerlinWorm(wormSettings);
                wormMaker = StartCoroutine(Process());
            }
        }

        IEnumerator Process()
        {
            Debug.Log("Execute process");
            Vector2 next = wormSettings.StartingPoint;
            int i = 0;
            while(i < wormSettings.Length)
            {
                next = worm.MoveNext();
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = new Vector3((int)next.x, 0, (int)next.y);
                go.transform.localScale = wormSettings.Scale;
                go.transform.parent = wormParent;

                i++;
                yield return new WaitForSecondsRealtime(wormSettings.Frequency);
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

    [System.Serializable]
    public class WorldSettings
    {
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