using UnityEngine;
using System.Collections.Generic;

namespace Milhouzer.ProceduralGeneration.WorldGeneration
{
    public static class WorldMaker
    {
        public static Chunk MakeChunk(int size, Vector3 location)
        {
            Chunk chunk = new Chunk(size, location);

            return chunk;
        }

        public static CombineInstance MakeTileMesh(int v, int i, int j, float h)
        {
            List<CombineInstance> instances = new List<CombineInstance>();

            MeshFilter meshFilter = World.Singleton.GenerationSettings.TILES_BOT_PARTS_LOOKUP_TABLE[v];

            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                Debug.Log(meshFilter.sharedMesh.name);
                instances.Add(
                    new CombineInstance()
                    {
                        mesh = meshFilter.sharedMesh,
                        transform = meshFilter.transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(90f, 0, 0))
                    }
                );
            }
            
            meshFilter = World.Singleton.GenerationSettings.TILES_MID_PARTS_LOOKUP_TABLE[v];
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                instances.Add(
                    new CombineInstance()
                    {
                        mesh = meshFilter.sharedMesh,
                        transform = Matrix4x4.Scale(new Vector3(1f, 1f, h))
                    }
                );
            }
            
            meshFilter = World.Singleton.GenerationSettings.TILES_TOP_PARTS_LOOKUP_TABLE[v];           
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                instances.Add(
                    new CombineInstance()
                    {
                        mesh = meshFilter.sharedMesh,
                        // Mesh orientation is +Z up, this should be modified to match unity when exporting .fbx from blender.
                        // We scale the z part of the scale because the meshes in blender are 2.5m high (but the origins is at 0) they should always be unitary.
                        transform = Matrix4x4.Translate(new Vector3(0, 0, h * 2f))
                    }
                );
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(instances.ToArray(), true, true);
            combinedMesh.Optimize();

            return new CombineInstance
            {
                mesh = combinedMesh,
                transform = Matrix4x4.Translate(new Vector3(i, 0, j)) * Matrix4x4.Rotate(Quaternion.Euler(-90f, 0, 0))
            };
        }
    }
}