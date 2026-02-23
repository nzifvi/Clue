using UnityEngine;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;


public class PlayerController : MonoBehaviour
{

// References
    public GameObject controller;
    public GameObject movePlate;
    public GameObject board_controller;
    //private int movementAmount = 2;
    private int xPos = -1;
    private int yPos = -1;

    private string player;
    Boolean isInTheRoom = false;

// references to possible sprites that a player can be ( just cirtles at the moment)
    public Sprite blue, pink, red, Circle;

    private bool hasMovementPhaseFinished = false;
    private bool hasSuggestionPhaseFinished = false;
    private bool hasAccusationPhaseFinished = false;
    
    private PlayerMovement playerMovementObj;
    void Awake()
    {
        playerMovementObj = GetComponent<PlayerMovement>();
    }
    
    void Update()
    {
        
    }

    // when players is created
    public void Activate()
    {
         controller = GameObject.FindGameObjectWithTag("GameController");
         board_controller = GameObject.FindGameObjectWithTag("BoardController");
    
        SetCoordinates();

        switch (this.name)
        {
            case "blue": this.GetComponent<SpriteRenderer>().sprite = blue; break;
            case "yellow": this.GetComponent<SpriteRenderer>().sprite = Circle; break;
            case "pink": this.GetComponent<SpriteRenderer>().sprite = pink; break;
            case "red": this.GetComponent<SpriteRenderer>().sprite = red; break;
        }
    }
    public void SetCoordinates()
    {
        float x = xPos;
        float y = yPos;
        x += 0.5f;
        y += 0.5f;

        this.transform.position = new Vector3(x,y,-2.0f);
    }
   
    public bool hasTurnFinished()
    {
        return hasMovementPhaseFinished && hasSuggestionPhaseFinished && hasAccusationPhaseFinished;
    }
    
    public void addMovementAmount(int newMovementAmount)
    {
        movementAmount = newMovementAmount;
    }

   // public void addMovementAmount(int newMovementAmount)
   // {
   //     movementAmount = newMovementAmount;
   // }
    public int getXPos(){return xPos;}
    public int getYPos(){return yPos;}
    public void setXPos(int xPos){this.xPos = xPos;}
    public void setYPos(int yPos){this.yPos = yPos;}
    
    private void OnMouseUp()
    {
        DestroyMovePlates();
        InitiateMovePlates();
    }
    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("PlayerMovement");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }
    public void InitiateMovePlates()
    {
             BoardController sc = board_controller.GetComponent<BoardController>();
             if(CanWalkThisTile(xPos-1,yPos)){ MovePlate(xPos-1,yPos);};
             if(CanWalkThisTile(xPos,yPos+1) || sc.CanEnterTheRoom(xPos,yPos+1)){MovePlate(xPos,yPos+1);};
             if(CanWalkThisTile(xPos,yPos-1)|| sc.CanEnterTheRoom(xPos,yPos-1)){MovePlate(xPos,yPos-1);};
             if(CanWalkThisTile(xPos+1,yPos)|| sc.CanEnterTheRoom(xPos+1,yPos)){MovePlate(xPos+1,yPos);};

             if(sc.CanEnterTheRoom(xPos - 1, yPos)){MovePlate(xPos-1,yPos);  }// save rooms coordinates
             if(sc.CanEnterTheRoom(xPos + 1, yPos)){MovePlate(xPos-1,yPos);}
             if(sc.CanEnterTheRoom(xPos , yPos - 1)){MovePlate(xPos,yPos -1);  }
             if(sc.CanEnterTheRoom(xPos , yPos +1 )){MovePlate(xPos,yPos + 1); }
             }

     public Boolean CanWalkThisTile(int x, int y)
    {
            BoardController sc = board_controller.GetComponent<BoardController>();
            Vector3 v = new Vector3(x,y,0); 
            Vector3Int pos = Vector3Int.FloorToInt(v);
            Tilemap fg = sc.getTileMap("Layer1_Walkable_Tiles");
            pos = fg.WorldToCell(pos);
            return fg.HasTile(pos);
         
    }
        
    public void MovePlate(int x, int y)
    {
        GameController sc = controller.GetComponent<GameController>();
        MovePlateSpawn(x,y);
    }
    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;
        x += 0.5f;
        y += 0.5f;

        GameObject mp = Instantiate(movePlate, new Vector3(x,y,-2.1f), Quaternion.identity);
        PlayerMovement sc = mp.GetComponent<PlayerMovement>();
        sc.SetReference(gameObject);
        sc.SetCoordinates(matrixX,matrixY);
        sc.SetReference(gameObject);
        sc.SetCoordinates(matrixX,matrixY);
    }
   
           
}
