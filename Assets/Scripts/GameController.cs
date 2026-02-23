
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class GameController : MonoBehaviour
{


     public enum PlayerID
    {
        Player1,
        Player2,
        Player3,
        Player4,
        Player5,
        Player6
    }
    public GameObject token;
    private GameObject[,] positions = new GameObject[26,26]; 
    // кординати для пішок
    int [] coordinates_start_x_positions = {9,14,16,23,0,0};
    int [] coordinates_start_y_positions = {0,0,24,17,19,6};
    private int player_count;
    private GameObject[] players;
    private String currentPlayer = "player_1";
    private bool gameOver = false;

    void Start()
    {
        player_count = 6;// set a number of players, [2,6]
        players = new GameObject[player_count];
        for(int i = 0; i < players.Length; i++)
        { players[i] = Create("yellow",coordinates_start_x_positions[i], coordinates_start_y_positions[i]) ;
            }
        for(int j = 0; j < players.Length; j++){SetPosition(players[j]);}
    }
    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(token, new Vector3(0,0,0), Quaternion.identity);
        PlayerController sm = obj.GetComponent<PlayerController>();
        sm.name = name;
        sm.setXPos(x);
        sm.setYPos(y);
        sm.Activate();
        return obj;
    }
   public void SetPosition(GameObject obj)
    {
        PlayerController sm = obj.GetComponent<PlayerController>();
        positions[sm.getXPos(),sm.getYPos()] = obj;
    }
   
    public void SetPositionEmpty(int x, int y){positions[x,y] = null;}
    public GameObject GetPosition(int x, int y){return positions[x,y];}

    public bool PositionOnBoard(int x, int y){return true;}


    }
