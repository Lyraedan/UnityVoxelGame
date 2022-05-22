using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public bool drawAir = false;
    public bool drawSolids = false;
    public bool drawChunkGizmos = false;
    public Material textureAtlas;
    List<Chunk> chunks = new List<Chunk>();

    void Start()
    {
        GenerateChunkAt(Vector3.zero);

        /*
        GenerateChunkAt(new Vector3(16, 0, 0));
        GenerateChunkAt(new Vector3(0, 0, 16));

        GenerateChunkAt(new Vector3(-16, 0, 0));
        GenerateChunkAt(new Vector3(0, 0, -16));

        GenerateChunkAt(new Vector3(0, 16, 0));
        GenerateChunkAt(new Vector3(0, -16, 0));
        */
    }

    void GenerateChunkAt(Vector3 position)
    {
        GameObject chunkGO = new GameObject("Chunk");
        Chunk chunk = chunkGO.AddComponent<Chunk>();
        chunk.atlas = textureAtlas;
        chunk.Generate(position);
        chunks.Add(chunk);
    }

    private void OnDrawGizmos()
    {
        if (drawChunkGizmos)
        {
            foreach (Chunk c in chunks)
            {
                c.DrawGizmos();
            }
        }
    }
}
