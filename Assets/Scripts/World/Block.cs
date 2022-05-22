using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public int id = -1;
    public static int textureSize = 16;
    public static Vector3 blockSize = new Vector3(1, 1, 1);
    public Vector2 textureCoords = new Vector2(0, 0);
    public Vector3 worldPosition = new Vector3(0, 0, 0);
    public Vector3Int position = new Vector3Int(0, 0, 0);

    public void Construct(Vector3 worldPosition, Vector3Int position, int id, Vector2 textureCoords)
    {
        this.id = id;
        this.textureCoords = textureCoords;
        this.worldPosition = worldPosition;
        this.position = position;
    }

    public void DrawGizmos()
    {
        if (id == BlockIDs.AIR)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(worldPosition + position, blockSize);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(worldPosition + position, blockSize);
        }
    }
}
