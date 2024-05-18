using System.Collections.Generic;
using UnityEngine;
using System;
using ProceduralToolkit.FastNoiseLib;

namespace Milhouzer.ProceduralGeneration.WorldGeneration
{
    public class Chunk
    {
        public int Size { get; }

        /// <summary>
        /// World position of the chunk. The CHUNK_SIZE factor should always already be taken in account for consistency.
        /// </summary>
        /// <value></value>
        public Vector3 Location { get; }

        public string Hash { get; }
        public string Name { get; }

        private readonly float[,] map;
        

        public Chunk(int size, Vector3 location)
        {
            Size = size;
            Location = location;
            Name = $"chk_{location.ToString().PadLeft(4, '0')}{location.y.ToString().PadLeft(4, '0')}{location.z.ToString().PadLeft(4, '0')}";
            Hash = GenerateHash();
            
            map = new float[Size + 1, Size + 1];

            FastNoise noise = World.Singleton.GroundNoise;
            for (int i = 0; i < Size + 1; i++)
            {
                for (int j = 0; j < Size + 1; j++)
                {
                    Debug.Log(map + ", " + noise + ", " + location);
                    map[i,j] = noise.GetNoise(location.x + i, location.z + j);
                }
            }
            // We don't really optimize for now, here we iterate through the array twice which give O(2n²) ~ O(n²) complexity.
            // vals = MarchingSquares.MarchSquares(new Vector2(Location.x, Location.z), Size, Size, World.Singleton.GenerationSettings.SCALE, World.Singleton.GenerationSettings.THRESHOLD);
        }

        public float this[int i, int j]
        {
            get => map[i,j];
        }

        // Generate hash for the chunk based on its location
        private string GenerateHash()
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Name);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        internal virtual void Load()
        {
            List<CombineInstance> instances = new List<CombineInstance>();
            // List<Vector2> maximas = FindLocalMaximas();

            for (int i = 0; i < map.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < map.GetLength(1) - 1; j++)
                {
                    int v = (int)MarchingSquares.MarchSquare(map[i, j + 1], map[i + 1, j + 1], map[i + 1, j], map[i, j], World.Singleton.GenerationSettings.THRESHOLD);

                    instances.Add(WorldMaker.MakeTileMesh(v, i, j, World.Singleton.GenerationSettings.CHUNK_HEIGHT));
                    
                    // MakeVegetation(v, i, j);

                    // We assume that maximas are created the same way they are red. 
                    // if(i == maximas[0].x && j == maximas[0].y)
                    // {
                    //     MakeRiverAnchor(i, j);

                    //     maximas.RemoveAt(0);
                    // }
                }
            }

            // Build a mesh that combines all of the requested cubes in our work list.
            GameObject go = new GameObject();
            go.name = Hash;
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.materials = new Material[1] { World.Singleton.GenerationSettings.TILE_MATERIALS[0] };
            
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(instances.ToArray(), true, true);
            mesh.Optimize();
            
            var filter = go.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            
            
            go.AddComponent<MeshCollider>();
            go.transform.position = Location;
            go.transform.parent = World.Singleton.transform;
            go.SetActive(true);
        }

        private List<Vector2> FindLocalMaximas()
        {
            Vector2[] directions = new Vector2[] {
                new Vector2(1, 0),
                new Vector2(-1, 0),
                new Vector2(0, 1),
                new Vector2(0, -1),
            };

            List<Vector2> maximas = new();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    float v = map[i,j];
                    bool isMaxima = true;
                    foreach(Vector2 dir in directions)
                    {
                        Vector2 newPos = new Vector2(i + dir.x, j + dir.y);
                        if(newPos.x < 0 || newPos.y < 0 || newPos.x > map.GetLength(0) || newPos.y > map.GetLength(1))
                        {
                            continue;
                        }

                        if(map[(int)newPos.x, (int)newPos.y] > v)
                        {
                            isMaxima = false;
                            break;
                        }
                    }

                    if(isMaxima)
                    {
                        maximas.Add(new Vector2(i, j));
                    }
                }
            }

            return maximas;
        }

        internal virtual void MakeVegetation(int v, int i, int j)
        {
            if(v == 0)
            {
                float r = UnityEngine.Random.Range(0, 1f);
                if(r > World.Singleton.GenerationSettings.VEGETATION_PROBABILITY)
                    return;
                    
                GameObject.Instantiate(
                    World.Singleton.GenerationSettings.VEGETATION_LOOKUP_TABLE[0],
                    new Vector3(Location.x + i, 0, Location.z + j),
                    Quaternion.identity,
                    World.Singleton.transform
                );
            }
        }

        internal virtual void MakeRiverAnchor(int i, int j)
        {

        }
        

        // Unload is unused for the moment because we don't store the objects. Find a clever way to store it. (ideally not using a monobehaviour)
        internal virtual void Unload()
        {
            throw new NotImplementedException();
        }
    }

    public class ChunkPreview : Chunk
    {
        GameObject preview;

        public ChunkPreview(int size, Vector3 location) : base(size, location) { }

        internal override void Load()
        {
            preview = GameObject.Instantiate(
                World.Singleton.GenerationSettings.CHUNK_PREVIEW_MODEL,
                Location,
                Quaternion.identity
            );

            preview.name = Location.x + "," + Location.z + "," + Location.y;
        }

        internal override void Unload()
        {
            GameObject.Destroy(preview);
            preview = null;
        }
    }
}
