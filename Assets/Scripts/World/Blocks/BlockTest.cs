using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTest : Block
{
    public BlockTest(Vector3 worldPosition, Vector3Int position)
    {
        Construct(worldPosition,
                  position,
                  BlockIDs.GRASS,
                  new Vector2(0, 0));
    }
}
