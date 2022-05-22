using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Material atlas;
    public Vector3 worldPosition = new Vector3(0, 0, 0);
    public Vector3Int chunkSize = new Vector3Int(16, 16, 16);
    public Block[,,] blocks;

    public new ChunkRenderer renderer;

    private void Start()
    {

    }

    public void Generate(Vector3 worldPosition)
    {
        blocks = new Block[chunkSize.x, chunkSize.y, chunkSize.z];
        this.worldPosition = worldPosition;
        renderer = gameObject.AddComponent<ChunkRenderer>();
        renderer.Init(this);

        FillWithAir();

        for (int x = 1; x < chunkSize.x - 1; x++)
        {
            for (int y = 1; y < chunkSize.y - 1; y++)
            {
                for (int z = 1; z < chunkSize.z - 1; z++)
                {
                    Vector3Int position = new Vector3Int(x, y, z);
                    SetBlockAt(x, y, z, new BlockTest(worldPosition, position));
                }
            }
        }

        renderer.GenerateMesh();
    }

    void FillWithAir()
    {
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    blocks[x, y, z] = new BlockAir(worldPosition, new Vector3Int(x, y, z));
                }
            }
        }
    }

    void SetBlockAt(int x, int y, int z, Block block)
    {
        blocks[x, y, z] = block;
    }

    public AdjacentChecker BlockIsAdjacent(int id, int x, int y, int z)
    {
        Block blockAtPosition = GetBlockAt(x, y, z);
        AdjacentChecker checker = new AdjacentChecker();
        for (int xp = -1; xp < 2; xp++)
        {
            for (int yp = -1; yp < 2; yp++)
            {
                for (int zp = -1; zp < 2; zp++)
                {
                    Block adjacent = GetBlockAt(x + xp, y + yp, z + zp);
                    if (adjacent != null)
                    {
                        if (adjacent.id == id)
                        {
                            checker.found = true;
                        }
                    }
                }
            }
        }

        // Get surrounding blocks in a + shape
        checker.blocks = new Block[] {
                                        // Up, down, left, right, forward, back
                                        GetBlockAt(x, y + 1, z),
                                        GetBlockAt(x, y - 1, z),
                                        GetBlockAt(x - 1, y, z),
                                        GetBlockAt(x + 1, y, z),
                                        GetBlockAt(x, y, z + 1),
                                        GetBlockAt(x, y, z - 1)
                                     };
        return checker;
    }

    public void DrawGizmos()
    {
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    Block block = blocks[x, y, z];
                    if (block.id == BlockIDs.AIR)
                    {
                        if (WorldGenerator.instance.drawAir)
                            block.DrawGizmos();
                    }
                    else // Draw other stuff, do not draw inner blocks
                    {
                        var adjacent = BlockIsAdjacent(0, x, y, z);
                        if (adjacent.found)
                        {
                            if (WorldGenerator.instance.drawSolids)
                                block.DrawGizmos();
                        }
                    }
                }
            }
        }
    }

    public Block GetBlockAt(int x, int y, int z)
    {
        if (x < 0 || x >= chunkSize.x ||
            y < 0 || y >= chunkSize.y ||
            z < 0 || z >= chunkSize.z)
            return null;

        return blocks[x, y, z];
    }
}

public class AdjacentChecker
{
    public bool found = false;
    public Block[] blocks;
}
