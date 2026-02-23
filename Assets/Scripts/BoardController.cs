


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardController : MonoBehaviour{
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

