using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardController : MonoBehaviour
{
    public Tilemap WalkableTilemap;
    public Tilemap RoomTilemap;
    public Tilemap DoorTilemap;

    // Constants for the number of rows and columns in the game board (CHANGE THESE IF THE BOARD SIZE CHANGES)
    private const int ROWS = 27;
    private const int COLS = 25;

    private Tile[,] grid = new Tile[COLS, ROWS];

    // List to hold all the room tiles for easy access when we need to check if a tile is a room tile
    private List<RoomTile> rooms = new List<RoomTile>();

    // Stores last tile each player was on like Ben asked
    private Dictionary<Player, Tile> lastTile = new Dictionary<Player, Tile>();

    void Start()
    {
        BuildGrid();
        Debug.Log($"Grid built. Tile at (5,5): {GetTile(5,5)?.GetType().Name ?? "NULL"}");
    }

    // Builds the grid by checking each tilemap for the presence of a tile at each co-ord
    void BuildGrid()
    {
        for (int x = 0; x < COLS; x++)
        {
            for (int y = 0; y < ROWS; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                if (RoomTilemap.HasTile(cell))
                {
                    RoomTile room = MakeTile<RoomTile>(x, y);
                    rooms.Add(room);
                    grid[x, y] = room;
                }
                else if (DoorTilemap.HasTile(cell))
                {
                    TraversableTile door = MakeTile<TraversableTile>(x, y);
                    door.SetTTType(TraversableTile.TTType.DOOR_TT);
                    grid[x, y] = door;
                }
                else if (WalkableTilemap.HasTile(cell))
                {
                    TraversableTile walkable = MakeTile<TraversableTile>(x, y);
                    walkable.SetTTType(TraversableTile.TTType.GENERIC_TT);
                    grid[x, y] = walkable;
                }
                else
                {
                    grid[x, y] = MakeTile<WallTile>(x, y);
                }
            }
        }
    }

    // Helper method to create a tile of type T at the specified coordinates
    T MakeTile<T>(int x, int y) where T : Tile
    {
        GameObject g = new GameObject("Tile_" + x + "_" + y);
        g.transform.position = new Vector3(x, 0.01f, y);
        T tile = g.AddComponent<T>();
        tile.X = x;
        tile.Y = y;
        return tile;
    }

    public bool IsWalkable(int x, int y)
    {
       Tile tile = GetTile(x, y);
       if (tile == null)
       {
            Debug.LogError($"MOVE BLOCKED: No tile exists in the grid at {x}, {y}. Check your Tilemap bounds!");
            return false;
       }
       if (tile.IsOccupied)
       {
            Debug.LogWarning($"MOVE BLOCKED: Tile at {x}, {y} is already occupied by another player.");
            return false;
       }
       if (tile is WallTile)
       {
            Debug.LogWarning($"MOVE BLOCKED: {x}, {y} is a WallTile.");
            return false;
       }
       return tile.IsWalkable();


       /*Tile tile = GetTile(x, y);
       if (tile == null)
       {
           Debug.LogError($"No tile at {x}, {y}");
           return false;
       }
       return true;*/
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= COLS || y < 0 || y >= ROWS)
            return null; 
        return grid[x, y];
    }

    public bool IsValidMove(Player player, int toX, int toY)
    {
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        int fromX = pm.getXPos();
        int fromY = pm.getYPos();

        int dx = Mathf.Abs(toX - fromX);
        int dy = Mathf.Abs(toY - fromY);
        bool isOneStep = (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        if (!isOneStep)
        {
            Debug.Log("Invalid move: must move exactly one cardinal step.");
            return false;
        }

        Tile destination = GetTile(toX, toY);
        Tile currentTile = GetTile(fromX, fromY);

        // 1. Check if trying to walk into an empty void or a WallTile
        if (destination == null || destination is WallTile)
        {
            Debug.Log("Invalid move: Hit a wall or edge of board!");
            return false;
        }

        // 2. ENTERING A ROOM: If destination is yellow Room, current MUST be red Door
        if (destination is RoomTile && !(currentTile is RoomTile))
        {
            TraversableTile currTraversable = currentTile as TraversableTile;
            if (currTraversable == null || currTraversable.GetTTType() != TraversableTile.TTType.DOOR_TT)
            {
                Debug.Log("Invalid move: You must stand on a Red Door Tile to enter the room!");
                return false;
            }
        }

        // 3. LEAVING A ROOM: If current is yellow Room, destination MUST be red Door
        if (currentTile is RoomTile && !(destination is RoomTile))
        {
            TraversableTile destTraversable = destination as TraversableTile;
            if (destTraversable == null || destTraversable.GetTTType() != TraversableTile.TTType.DOOR_TT)
            {
                Debug.Log("Invalid move: You must exit the room through a Red Door Tile!");
                return false;
            }
        }

        // 4. Prevent backtracking
        if (lastTile.ContainsKey(player) && lastTile[player] == destination)
        {
            Debug.Log("Invalid move: cannot move back to the previous tile");
            return false;
        }

        return true;
    }

    public void OnPlayerMoved(Player player, int toX, int toY)
    {
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        int fromX = pm.getXPos();
        int fromY = pm.getYPos();

        // Record the tile they are leaving as their last tile
        Tile previousTile = GetTile(fromX, fromY);
        if (previousTile != null)
            lastTile[player] = previousTile;

        SetTileOccupied(fromX, fromY, false);
        SetTileOccupied(toX, toY, true);

        RoomTile room = GetRoom(toX, toY);
        if (room != null)
            MovePlayerToRoom(player, room);
    }

    public RoomTile GetRoom(int x, int y)
    {
        return GetTile(x, y) as RoomTile;
    }

    // Move the player to the new room
    public void MovePlayerToRoom(Player player, RoomTile newRoom)
    {
        if (player.CurrentRoom != null)
        {
            player.CurrentRoom.LeaveRoom(player);
            if (UIManager.Instance != null)
                UIManager.Instance.AddLogMessage($"{player.PlayerName} left the {player.CurrentRoom.gameObject.name}.");
        }
        newRoom.EnterRoom(player);

        if (UIManager.Instance != null)
            UIManager.Instance.AddLogMessage($"{player.PlayerName} entered the {newRoom.gameObject.name}.");
    }

    //Change TT to Occupied/unoccupied
    public void SetTileOccupied(int x, int y, bool occupied)
    {
        Tile tile = GetTile(x, y);
        if (tile == null)
            return;

        tile.IsOccupied = occupied;

        TraversableTile tt = tile as TraversableTile;
        if (tt != null)
        {
            if (occupied)
                tt.SetOccupationState(TraversableTile.OccupiedState.OCCUPIED);
            else
                tt.SetOccupationState(TraversableTile.OccupiedState.UNOCCUPIED);
        }
    }

    public void addWeaponToRoom(Weapon weapon, RoomTile room)
    {
        room.AddWeapon(weapon);
    }

    public void removeWeaponFromRoom(Weapon weapon, RoomTile room)
    {
        room.RemoveWeapon(weapon);
    }

    public List<RoomTile> GetAllRooms()
    {
        return rooms;
    }
}
