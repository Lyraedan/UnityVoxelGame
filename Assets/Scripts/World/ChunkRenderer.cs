using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour
{
    private Chunk chunk;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> tris = new List<int>();
    public List<Vector3> normals = new List<Vector3>();
    public List<Vector2> uvs = new List<Vector2>();

    [HideInInspector] public Mesh mesh;
    [HideInInspector] public MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    const int UP = 0;
    const int DOWN = 1;
    const int LEFT = 2;
    const int RIGHT = 3;
    const int FORWARD = 5;
    const int BACK = 4;

    public void Init(Chunk chunk)
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshCollider = gameObject.AddComponent<MeshCollider>();

        mesh = new Mesh();

        this.chunk = chunk;
        meshRenderer.sharedMaterial = chunk.atlas;
    }

    public void GenerateMesh()
    {
        int index = 0;

        for (int x = 0; x < Chunk.chunkSize.x; x++)
        {
            for (int y = 0; y < Chunk.chunkSize.y; y++)
            {
                for (int z = 0; z < Chunk.chunkSize.z; z++)
                {
                    Block block = chunk.GetBlockAt(x, y, z);
                    var adjacent = chunk.BlockIsAdjacent(BlockIDs.AIR, x, y, z);
                    if (adjacent.found)
                    {
                        if (block.id != BlockIDs.AIR)
                        {
                            if (adjacent.blocks[UP] != null)
                            {
                                if (adjacent.blocks[UP].id == BlockIDs.AIR)
                                    index = GenerateFace(UP, index, x, y, z, adjacent.blocks[UP]);
                            } else
                            {
                                index = GenerateFace(UP, index, x, y, z, block);
                            }
                            if (adjacent.blocks[DOWN] != null)
                            {
                                if (adjacent.blocks[DOWN].id == BlockIDs.AIR)
                                    index = GenerateFace(DOWN, index, x, y, z, adjacent.blocks[DOWN]);
                            } else
                            {
                                index = GenerateFace(DOWN, index, x, y, z, block);
                            }
                            if (adjacent.blocks[LEFT] != null)
                            {
                                if (adjacent.blocks[LEFT].id == BlockIDs.AIR)
                                    index = GenerateFace(LEFT, index, x, y, z, adjacent.blocks[LEFT]);
                            } else
                            {
                                index = GenerateFace(LEFT, index, x, y, z, block);
                            }
                            if (adjacent.blocks[RIGHT] != null)
                            {
                                if (adjacent.blocks[RIGHT].id == BlockIDs.AIR)
                                    index = GenerateFace(RIGHT, index, x, y, z, adjacent.blocks[RIGHT]);
                            } else
                            {
                                index = GenerateFace(RIGHT, index, x, y, z, block);
                            }
                            if (adjacent.blocks[FORWARD] != null)
                            {
                                if (adjacent.blocks[FORWARD].id == BlockIDs.AIR)
                                    index = GenerateFace(FORWARD, index, x, y, z, adjacent.blocks[FORWARD]);
                            } else
                            {
                                index = GenerateFace(FORWARD, index, x, y, z, block);
                            }
                            if (adjacent.blocks[BACK] != null)
                            {
                                if (adjacent.blocks[BACK].id == BlockIDs.AIR)
                                    index = GenerateFace(BACK, index, x, y, z, adjacent.blocks[BACK]);
                            } else
                            {
                                index = GenerateFace(BACK, index, x, y, z, block);
                            }
                        }
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    int GenerateFace(int direction, int index, int x, int y, int z, Block block)
    {
        Vector3 topLeft = Vector3.zero;
        Vector3 topRight = Vector3.zero;
        Vector3 bottomLeft = Vector3.zero;
        Vector3 bottomRight = Vector3.zero;

        Vector3 position = chunk.worldPosition + new Vector3(x - 0.5f, y, z - 0.5f);

        float heightA = position.y + 0.5f;
        float heightB = position.y - 0.5f;

        switch (direction)
        {
            case UP: // Up
                topLeft = new Vector3(position.x, heightA, position.z);
                topRight = new Vector3(position.x + Block.blockSize.x, heightA, position.z);
                bottomLeft = new Vector3(position.x, heightA, position.z + Block.blockSize.z);
                bottomRight = new Vector3(position.x + Block.blockSize.x, heightA, position.z + Block.blockSize.z);
                break;
            case DOWN: // Down
                topLeft = new Vector3(position.x, heightB, position.z);
                topRight = new Vector3(position.x + Block.blockSize.x, heightB, position.z);
                bottomLeft = new Vector3(position.x, heightB, position.z + Block.blockSize.z);
                bottomRight = new Vector3(position.x + Block.blockSize.x, heightB, position.z + Block.blockSize.z);
                break;
            case LEFT: // Left
                topLeft = new Vector3(position.x, heightA, position.z);
                topRight = new Vector3(position.x, heightA, position.z + Block.blockSize.z);

                bottomLeft = new Vector3(position.x, heightB, position.z);
                bottomRight = new Vector3(position.x, heightB, position.z + Block.blockSize.z);
                break;
            case RIGHT: // Right
                topLeft = new Vector3(position.x + Block.blockSize.x, heightA, position.z);
                topRight = new Vector3(position.x + Block.blockSize.x, heightA, position.z + Block.blockSize.z);

                bottomLeft = new Vector3(position.x + Block.blockSize.x, heightB, position.z);
                bottomRight = new Vector3(position.x + Block.blockSize.x, heightB, position.z + Block.blockSize.z);
                break;
            case FORWARD: // Forward
                topLeft = new Vector3(position.x, heightA, position.z);
                topRight = new Vector3(position.x + Block.blockSize.x, heightA, position.z);

                bottomLeft = new Vector3(position.x, heightB, position.z);
                bottomRight = new Vector3(position.x + Block.blockSize.x, heightB, position.z);
                break;
            case BACK: // Back
                topLeft = new Vector3(position.x, heightA, position.z + Block.blockSize.z);
                topRight = new Vector3(position.x + Block.blockSize.x, heightA, position.z + Block.blockSize.z);

                bottomLeft = new Vector3(position.x, heightB, position.z + Block.blockSize.z);
                bottomRight = new Vector3(position.x + Block.blockSize.x, heightB, position.z + Block.blockSize.z);
                break;
        }

        vertices.Add(topLeft);
        vertices.Add(topRight);
        vertices.Add(bottomLeft);
        vertices.Add(bottomRight);

        bool invert = false;
        switch(direction)
        {
            case UP:
                invert = false;
                break;
            case DOWN:
                invert = true;
                break;
            case LEFT:
                invert = false;
                break;
            case RIGHT:
                invert = true;
                break;
            case FORWARD:
                invert = true;
                break;
            case BACK:
                invert = false;
                break;
        }

        if (!invert)
        {
            tris.Add(index);
            tris.Add(index + 2);
            tris.Add(index + 1);

            tris.Add(index + 1);
            tris.Add(index + 2);
            tris.Add(index + 3);
        } else
        {
            tris.Add(index + 1);
            tris.Add(index + 2);
            tris.Add(index);

            tris.Add(index + 3);
            tris.Add(index + 2);
            tris.Add(index + 1);
        }

        Vector3 normalA = CalculateNormal(topLeft, bottomLeft, bottomRight);
        Vector3 normalB = CalculateNormal(topLeft, bottomRight, topRight);

        normals.Add(normalA);
        normals.Add(normalA);
        normals.Add(normalA);
        normals.Add(normalB);

        float width = Block.textureSize;
        float height = Block.textureSize;
        uvs.Add(new Vector2(block.textureCoords.x * Block.textureSize, block.textureCoords.y * Block.textureSize));
        uvs.Add(new Vector2(block.textureCoords.x * Block.textureSize + width, block.textureCoords.y * Block.textureSize));
        uvs.Add(new Vector2(block.textureCoords.x * Block.textureSize, block.textureCoords.y * Block.textureSize + height));
        uvs.Add(new Vector2(block.textureCoords.x * Block.textureSize + width, block.textureCoords.y * Block.textureSize + height));

        int nextIndex = index + 4;
        return nextIndex;
    }

    public Vector3 CalculateNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = new Vector3(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
        Vector3 b = new Vector3(p3.x - p1.x, p3.y - p1.y, p3.z - p1.z);

        Vector3 normal = new Vector3(a.y * b.z - a.z * b.y,
                                     a.z * b.x - a.x * b.z,
                                     a.x * b.y - a.y * b.x);
        return normal.normalized;
    }
}
