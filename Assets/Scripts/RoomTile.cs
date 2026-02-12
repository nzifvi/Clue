using System.Collections.Generic;
using UnityEngine;

public class RoomTile : Tile
{
    public string RoomName;
    public List<Tile> EntryPoints = new List<Tile>(); // Doors
    public List<Player> OccupyingPlayers = new List<Player>();
    public List<Weapon> WeaponsInRoom = new List<Weapon>();

    public override void Start()
    {
        tileType = TileType.RT;
    }

    public override bool IsWalkable()
    {
        return true; // Multiple players can be in a room
    }

    public void EnterRoom(Player player)
    {
        if (!OccupyingPlayers.Contains(player))
        {
            OccupyingPlayers.Add(player);
            player.CurrentRoom = this;
        }
    }

    public void LeaveRoom(Player player)
    {
        if (OccupyingPlayers.Contains(player))
        {
            OccupyingPlayers.Remove(player);
            player.CurrentRoom = null;
        }
    }

    public void AddWeapon(Weapon weapon) => WeaponsInRoom.Add(weapon);
    public void RemoveWeapon(Weapon weapon) => WeaponsInRoom.Remove(weapon);
}
