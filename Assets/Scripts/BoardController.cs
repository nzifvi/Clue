using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Runtime.Serialization;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.Tilemaps;
public class BoardController : MonoBehaviour
{

     Dictionary<string, Tilemap> tilemaps = new Dictionary<string,Tilemap>();
    [SerializeField] public Tilemap Layer1_Walkable_Tiles;
    [SerializeField] public Tilemap Layer2_Rooms;
    [SerializeField]private Tilemap Layer3_Empty_Tiles;
     [SerializeField]private Tilemap Layer4_Doors;     
     public enum TileType //???
    {
        Corridor,
        Room,
        Door,
        None,
    }
    private const int rowLength = 10;
    private const int colLength = 40;
    private List<List<GameObject>> grid = new  List<List<GameObject>>();

     [Serializable]
    public class TileMapData
    {
        public string key;
        public List<TileInfo> tiles = new List<TileInfo>();
    }
    [Serializable]
    public class TileInfo
    {
        public TileBase tile;
        public Vector3Int position;
       // TileType typeTile;
        public TileInfo(Vector3Int pos)
        {
            position = pos;
            // adding type?????
        } 
    }
    // private Tilemap Layer4_Doors????
    [Obsolete]
    void Start()
    {
       Tilemap[] maps = FindObjectsOfType<Tilemap>();
       foreach (var map in maps)
        {
            tilemaps.Add(map.name, map);
        }
        foreach( var mapObj in tilemaps)
        {
            TileMapData mapData = new TileMapData();
            mapData.key = mapObj.Key;
            BoundsInt boundsForThisMap = mapObj.Value.cellBounds;
            for ( int x = boundsForThisMap.xMin; x < boundsForThisMap.xMax; x++)
            {for(int y = boundsForThisMap.yMin; y < boundsForThisMap.yMax; y++){
                Vector3Int pos = new Vector3Int(x,y,0);
                TileBase tile = mapObj.Value.GetTile(pos);
                if (tile != null){
                        TileInfo ti = new TileInfo(pos);
                        mapData.tiles.Add(ti);}
            }
        }
        }

            for (int i = 0; i < rowLength; i++)
        {
            grid.Add(new List<GameObject>());
            for (int j = 0; j < colLength; j++)
            {
                GameObject g = new GameObject();
                g.name = "Tile" + i + j;
                g.AddComponent<TraversableTile>();
                
                g.transform.position = new Vector3(20 * i, 20 * j, 0);
                g.transform.localScale = new Vector3(10, 10, 0);
                
                SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("TTTestTile");
                grid[i].Add(g);
            }
        }
    }

    
    public Boolean CanEnterTheRoom(int x, int y)
    {
        Vector3 v = new Vector3(x,y,0);
        Vector3Int pos = Vector3Int.FloorToInt(v);
        pos = Layer4_Doors.WorldToCell(pos);
        bool has  = Layer4_Doors.HasTile(pos);
        return has;
  
    } 

   public Tilemap getTileMap(string name)
{
    return tilemaps["Layer1_Walkable_Tiles"];
}

}
    // size of each sell on the grid


    
  




    
   