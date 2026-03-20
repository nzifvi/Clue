using System.Collections.Generic;
using UnityEngine;

public class RoomTile : Tile
{
    public string RoomName;
    public List<Tile> EntryPoints = new List<Tile>(); // Doors to this room
    public List<Player> OccupyingPlayers = new List<Player>();
    public List<Weapon> WeaponsInRoom = new List<Weapon>();

    public override void Start()
    {
        tileType = TileType.RT;
    }

    public override bool IsWalkable()
    {
        // Multiple players can be in a room
        return true;
    }

    // Add public RoomTile CurrentRoom; to Player.cs to track which room a player is in

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

    public class Weapon
    {
        //Replace with full weapon.cs
    }



    public void AddWeapon(Weapon weapon) => WeaponsInRoom.Add(weapon);
    public void RemoveWeapon(Weapon weapon) => WeaponsInRoom.Remove(weapon);
}