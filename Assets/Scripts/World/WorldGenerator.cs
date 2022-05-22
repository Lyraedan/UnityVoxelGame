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

    public int seed;
    public float viewSize = 1f;
    public bool isInfininte = false;
    public bool drawAir = false;
    public bool drawSolids = false;
    public bool drawCulled = false;
    public bool drawChunkGizmos = true;
    public Material textureAtlas;
    Dictionary<string, Chunk> chunks = new Dictionary<string, Chunk>();

    void Start()
    {
        if (seed == 0)
            seed = UnityEngine.Random.Range(0, 10000);

        if(!isInfininte)
        {
            GenerateChunkAt(Vector3.zero);
        } else
        {
            GenerateChunkIfWeNeedTo();
        }
    }

    private void Update()
    {
        if(isInfininte)
            GenerateChunkIfWeNeedTo();
    }

    void GenerateChunkAt(Vector3 position)
    {
        string key = $"Chunk_{position.x}_{position.y}_{position.z}";
        if (!chunks.ContainsKey(key))
        {
            GameObject chunkGO = new GameObject(key);
            chunkGO.transform.SetParent(transform);
            Chunk chunk = chunkGO.AddComponent<Chunk>();
            chunk.atlas = textureAtlas;
            chunk.Generate(position);
            chunks.Add(key, chunk);
        }
    }

    void GenerateChunkIfWeNeedTo()
    {
        float camX = ChunkX();
        float camY = ChunkY();
        float camZ = ChunkZ();

        for (float x = camX - viewSize; x <= camX + viewSize; x++)
        {
            //for (float y = camY - viewSize; y <= camY + viewSize; y++)
            //{
            for (float z = camZ - viewSize; z <= camZ + viewSize; z++)
            {
                Vector3 position = new Vector3(x * (Chunk.chunkSize.x * Block.blockSize.x),
                                               //y * (Chunk.chunkSize.y * Block.blockSize.y),
                                               0,
                                               z * (Chunk.chunkSize.z * Block.blockSize.z));
                if (!ChunkExistsAt(position))
                {
                    GenerateChunkAt(position);
                }
                //  }
            }
        }
    }

    public bool ChunkExistsAt(Vector3 position)
    {
        return chunks.ContainsKey($"Chunk_{position.x}_{position.y}_{position.z}");
    }

    float ChunkX()
    {
        return Mathf.Round(Camera.main.transform.position.x / (Chunk.chunkSize.x * Block.blockSize.x));
    }

    float ChunkY()
    {
        return Mathf.Round(Camera.main.transform.position.x / (Chunk.chunkSize.y * Block.blockSize.y));
    }

    float ChunkZ()
    {
        return Mathf.Round(Camera.main.transform.position.z / (Chunk.chunkSize.z * Block.blockSize.z));
    }

    private void OnDrawGizmos()
    {
        if (drawChunkGizmos)
        {
            foreach (string key in chunks.Keys)
            {
                Chunk c = chunks[key];
                c.DrawGizmos();
            }
        }
    }
}
