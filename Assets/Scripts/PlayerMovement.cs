using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private int movementAmount = 0;
    private Vector2 movementVector;
    private Rigidbody rb;
    private bool isMovementKeyLocked = false;
    private const float stepSize = 1f;

    private int xPos;
    private int yPos;

    private BoardController board;
    private Player player;
    private GameController gameController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        board = FindObjectsByType<BoardController>(FindObjectsSortMode.None)[0];
        player = GetComponent<Player>();
        gameController = FindObjectsByType<GameController>(FindObjectsSortMode.None)[0];

        xPos = Mathf.RoundToInt(transform.position.x);
        yPos = Mathf.RoundToInt(transform.position.z);
    }
    
    private void OnMovement(InputValue value)
    {
        Debug.Log("OnMovement called");
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

        if (gameController.CurrentPlayer != player) return;
        if (gameController.CurrentPhase != GameController.TurnPhase.MOVEMENT) return;

        if (movementAmount > 0)
        {
            isMovementKeyLocked = true;
            Vector3 move = new Vector3(
            Mathf.Round(movementVector.x),
            0f,
            Mathf.Round(movementVector.y)
            ) * stepSize;

            int targetX = Mathf.RoundToInt(rb.position.x + move.x);
            int targetZ = Mathf.RoundToInt(rb.position.z + move.z);

            //if (board != null && !board.IsWalkable(targetX, targetZ))
            //{
             //   isMovementKeyLocked = false;
             //   return;
          //  }

            rb.MovePosition(rb.position + move);
            movementAmount--;

            xPos = Mathf.RoundToInt(rb.position.x);
            yPos = Mathf.RoundToInt(rb.position.z);

            if (board != null && player != null)
               board.OnPlayerMoved(player, xPos, yPos);

            if (movementAmount <= 0)
                gameController.OnMovementFinished();
        }
    }

    public void SetMovementAmount(int amount) => movementAmount = amount;
    public int MovesRemaining => movementAmount;

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
