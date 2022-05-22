using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDirt : Block
{
    public BlockDirt(Vector3 worldPosition, Vector3Int position)
    {
        Construct(worldPosition,
                  position,
                  BlockIDs.DIRT,
                  new Vector2(0, 1));
    }
}
