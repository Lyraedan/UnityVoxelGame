using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBedrock : Block
{
    public BlockBedrock(Vector3 worldPosition, Vector3Int position)
    {
        Construct(worldPosition,
                  position,
                  BlockIDs.BEDROCK,
                  new Vector2(1, 1));
    }
}
