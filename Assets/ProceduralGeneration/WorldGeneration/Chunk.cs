using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milhouzer.ProceduralGeneration;
using System;
using System.Text;

namespace Milhouzer.WorldGeneration
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

        public Chunk(int size, Vector3 location)
        {
            Size = size;
            Location = location;
            Name = $"chk_{location.ToString().PadLeft(4, '0')}{location.y.ToString().PadLeft(4, '0')}{location.z.ToString().PadLeft(4, '0')}";
            Hash = GenerateHash();
        }

        // Generate hash for the chunk based on its location
        private string GenerateHash()
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Name);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        internal virtual void Load()
        {
            // We don't really optimize for now, here we iterate through the array twice which give O(2n²) ~ O(n²) complexity.
            uint[,] vals = MarchingSquares.MarchSquares(new Vector2(Location.x, Location.z), Size, Size, World.Singleton.GenerationSettings.SCALE, World.Singleton.GenerationSettings.THRESHOLD);

            List<CombineInstance> instances = new List<CombineInstance>();

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    instances.Add(WorldMaker.MakeTileMesh((int)vals[i,j], i, j, World.Singleton.GenerationSettings.CHUNK_HEIGHT));
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
            go.SetActive(true);
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
