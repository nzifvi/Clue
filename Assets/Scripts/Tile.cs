using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public enum TileType
    {
        T,   // Traversable tile
        TT,  // Traversable tile (doors)
        WT,  // Wall tile
        RT   // Room tile
    }

    protected TileType tileType;

    public int X { get; set; }
    public int Y { get; set; }

    public bool IsOccupied { get; set; } = false;

    public virtual void Start()
    {
        tileType = TileType.T;
    }
    public abstract bool IsWalkable();

    public TileType GetTileType() => tileType;
}