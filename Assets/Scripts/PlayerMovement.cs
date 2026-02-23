using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.Collections;

public class PlayerMovement : MonoBehaviour
{
    private int movementAmount = 5;
    private Vector2 movementVector;
    private Rigidbody2D rb;
    private bool isMovementKeyLocked = false;
    private const int stepSize = 8;
       public GameObject GameController;
    GameObject reference = null;
    int matrixX;
    int matrixY;
    public bool isEnteringRoom = false;

    private int xPos;
    private int yPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
    }
    
    private void OnMovement(InputValue value)
    {
        movementVector = value.Get<Vector2>();

        if (movementVector == Vector2.zero)
        {
            isMovementKeyLocked = false;
            return;
        }

        if (isMovementKeyLocked)
        {
            return;
        }
        else
        {
            if (movementAmount > 0)
            {
                isMovementKeyLocked = true;
                Vector2 unitVector = new Vector2(
                    movementVector.x / movementVector.magnitude,
                    movementVector.y / movementVector.magnitude
                );
                rb.MovePosition(rb.position + new Vector2(stepSize * unitVector.x, stepSize * unitVector.y));
                movementAmount--;
            }
        }
    }
    
    public int getXPos()
    {
        return xPos;
    }

    public int getYPos()
    {
        return yPos;
    }

    public void setXPos(int xPos)
    {
        this.xPos = xPos;
    }

    public void setYPos(int yPos)
    {
        this.yPos = yPos;
    }
     public void Start()
    {
        ///code for entering rooms?
        if (isEnteringRoom)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }
    public void OnMouseUp()
    {
        GameController  = GameObject.FindGameObjectWithTag("GameController");

        GameController.GetComponent<GameController>().SetPositionEmpty(reference.GetComponent<PlayerController>().getXPos(),
        reference.GetComponent<PlayerController>().getYPos());
        reference.GetComponent<PlayerController>().setXPos(matrixX);
        reference.GetComponent<PlayerController>().setYPos(matrixY);
        reference.GetComponent<PlayerController>().SetCoordinates();

        GameController.GetComponent<GameController>().SetPosition(reference);
        reference.GetComponent<PlayerController>().DestroyMovePlates();
        }
    

      public void SetCoordinates(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }
    public void SetReference(GameObject obj)
    {
        reference = obj;
    }
    public GameObject GetReference()

    {
        return reference;
    }
}


 


   






