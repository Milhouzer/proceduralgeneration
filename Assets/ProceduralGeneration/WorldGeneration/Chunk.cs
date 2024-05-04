using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milhouzer.ProceduralGeneration;
using System;

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

        public Chunk(int size, Vector3 location)
        {
            Size = size;
            Location = location;
            Hash = GenerateHash();
        }

        // Generate hash for the chunk based on its location
        private string GenerateHash()
        {
            string sizeHex = Size.ToString("X");

            // Convert the Vector3 location components to hexadecimal format
            string xHex = BitConverter.ToInt32(BitConverter.GetBytes(Location.x), 0).ToString("X");
            string yHex = BitConverter.ToInt32(BitConverter.GetBytes(Location.y), 0).ToString("X");
            string zHex = BitConverter.ToInt32(BitConverter.GetBytes(Location.z), 0).ToString("X");

            string hash = string.Concat(sizeHex , xHex , yHex , zHex);

            return hash;
        }

        internal virtual void Load()
        {
            // We don't really optimize for now, here we iterate through the array twice which give O(2n²) ~ O(n²) complexity.
            uint[,] vals = MarchingSquares.MarchSquares(new Vector2(Location.x, Location.z), Size, Size, World.Singleton.GenerationSettings.SCALE, World.Singleton.GenerationSettings.THRESHOLD);

            List<CombineInstance> instances = new List<CombineInstance>();
            CombineInstance instance = new();
            MeshFilter meshFilter;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    meshFilter = World.Singleton.GenerationSettings.MESH_LOOKUP_TABLE[(int)vals[i, j]];
                    instance.mesh = meshFilter.sharedMesh;
                    instance.transform = Matrix4x4.Translate(new Vector3(i, 0, j)) * Matrix4x4.Rotate(Quaternion.Euler(-90f, 0, 0));

                    instances.Add(instance);
                    Debug.Log(instances.Count);
                }
            }

            // Build a mesh that combines all of the requested cubes in our work list.
            GameObject go = new GameObject();
            go.name = Hash;
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.materials = new Material[1] { World.Singleton.GenerationSettings.TILE_MATERIALS[0] };
            
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(instances.ToArray(), true, true);
            
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
