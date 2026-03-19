using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        T,
        TT,
        WT
    }
    protected const int size = 200; // SIZE IN PIXELS, MIGHT NEED TO BE CHANGED TO SIZE IN UNITY?
    protected TileType tileType;
    public virtual void Start()
    {
        tileType = TileType.T;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public TileType getTileType()
    {
        return tileType;
    }
}
