using System;
using System.Collections;
using ProceduralToolkit.FastNoiseLib;
using UnityEngine;

namespace Milhouzer.ProceduralGeneration.WorldGeneration
{
    public class World : MonoBehaviour
    {
        public static World Singleton;

        [SerializeField]
        private WorldGenerationSettings generationSettings;
        public WorldGenerationSettings GenerationSettings => generationSettings;

        
        /// <TODO>
        /// Make river factory
        /// </TODO>
        [SerializeField]
        private GridPerlinWormSettings wormSettings;
        GridPerlinWorm worm;
        Coroutine wormMaker;
        Transform wormParent;


        private Vector3 currentPosition = Vector3.zero;
        private ChunkPreview chunkPreview;

        [SerializeField]
        private FastNoiseSettings groundNoiseSettings;
        public FastNoise GroundNoise { get; private set; }
        [SerializeField]
        private FastNoiseSettings riverNoiseSettings;
        public FastNoise RiverNoise { get; private set; }

        private void Awake() 
        {
            groundNoiseSettings.OnPropertyChanged += GroundNoiseSettingsChanged;
            riverNoiseSettings.OnPropertyChanged += RiverNoiseSettingsChanged;
            GroundNoise = new FastNoise(groundNoiseSettings);
            RiverNoise = new FastNoise(riverNoiseSettings);
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

        private void OnDestroy() {
            groundNoiseSettings.OnPropertyChanged -= GroundNoiseSettingsChanged;
            riverNoiseSettings.OnPropertyChanged -= RiverNoiseSettingsChanged;
        }

        private void GroundNoiseSettingsChanged()
        {
            GroundNoise = new FastNoise(groundNoiseSettings);
        }

        private void RiverNoiseSettingsChanged()
        {
            RiverNoise = new FastNoise(riverNoiseSettings);
        }

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
                worm = new GridPerlinWorm(wormSettings, riverNoiseSettings);
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
}