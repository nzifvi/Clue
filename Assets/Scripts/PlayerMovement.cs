using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private int movementAmount = 5;
    private Vector2 movementVector;
    private Rigidbody2D rb;
    private bool isMovementKeyLocked = false;
    private const int stepSize = 8;

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
}
