using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Material atlas;
    public Vector3 worldPosition = new Vector3(0, 0, 0);
    public static Vector3Int chunkSize = new Vector3Int(16, 16, 16);
    public Block[,,] blocks;

    public NoiseSettings[] noiseSettings = new NoiseSettings[] {
        new NoiseSettings() {
            noiseType = NoiseSettings.NoiseType.Simplex,
            enabled = true,
            interations = 1,
            strength = 10,
            roughness = 2,
            scale = 0.29f,
            persistance = 1,
            lacunarity = 1,
            minValue = -1.26f
        },
        new NoiseSettings() {
            noiseType = NoiseSettings.NoiseType.Perlin,
            enabled = true,
            interations = 1,
            strength = 1,
            roughness = 13.85f,
            scale = 2,
            persistance = 1,
            lacunarity = 1,
            minValue = 5.13f
        },
        new NoiseSettings() {
            noiseType = NoiseSettings.NoiseType.Fractal,
            enabled = true,
            interations = 4,
            strength = 1.26f,
            roughness = 1,
            scale = -1.35f,
            persistance = 1,
            lacunarity = 1,
            minValue = -2
        }
    };

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
        for(int x = 0; x < chunkSize.x; x++)
        {
            for(int z = 0; z < chunkSize.y; z++)
            {
                int startY = chunkSize.y / 3;
                // Generate surface
                int noise = startY + CalculateHeight(x, z);
                Vector3Int position = new Vector3Int(x, noise, z);
                SetBlockAt(x, noise, z, new BlockTest(worldPosition, position));

                // Fill ground underneath
                for (int y = 0; y <= noise; y++)
                {
                    position = new Vector3Int(x, y, z);
                    SetBlockAt(x, y, z, new BlockDirt(worldPosition, position));
                }

                position = new Vector3Int(x, 0, z);
                SetBlockAt(x, 0, z, new BlockBedrock(worldPosition, position));
            }
        }

        int xp = chunkSize.x / 2;
        int zp = chunkSize.z / 2;

        for (int x = -1; x < 2; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    Vector3Int position = new Vector3Int(xp + x, y, zp + z);
                    SetBlockAt(x, y, z, new BlockAir(worldPosition, position));
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
        if (x < 0 || x >= chunkSize.x ||
            y < 0 || y >= chunkSize.y ||
            z < 0 || z >= chunkSize.z)
            return;

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

    int CalculateHeight(int x, int z)
    {
        int sample = 0;
        float offset = 5000.0f;
        float amplitude = 1;

        int strength = 1;
        float scaleX = (chunkSize.x * Block.blockSize.x);
        float scaleZ = (chunkSize.z * Block.blockSize.z);

        float chunkX = worldPosition.x + scaleX;
        float chunkZ = worldPosition.z + scaleZ;

        float roughness = 2f;
        float frequancy = 1f;

        float noiseX = WorldGenerator.instance.seed + (offset + chunkX + x) * roughness / scaleX;
        float noiseZ = WorldGenerator.instance.seed + (offset + chunkZ + z) * roughness / scaleZ;

        sample += Mathf.RoundToInt(Mathf.PerlinNoise(noiseX, noiseZ) * frequancy);
        frequancy *= amplitude;

        sample *= strength;

        return sample;
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
                        } else
                        {
                            if (WorldGenerator.instance.drawSolids && WorldGenerator.instance.drawCulled)
                                block.DrawCulledGizmos();
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
