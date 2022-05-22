using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAir : Block
{
    public BlockAir(Vector3 worldPosition, Vector3Int position)
    {
        Construct(worldPosition,
                  position,
                  BlockIDs.AIR,
                  new Vector2(-1, -1));
    }
}
