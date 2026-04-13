using UnityEngine;

public class WallTile : Tile
{
    public override void Start()
    {
        tileType = TileType.WT;
    }

    public override bool IsWalkable()
    {
        return false;
    }
}
