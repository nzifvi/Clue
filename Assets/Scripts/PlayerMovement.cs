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
        transform.position = new Vector3(xPos + 0.5f, transform.position.y, yPos + 0.5f);

        if(board != null)
            board.SetTileOccupied(xPos, yPos, true);

        Debug.Log($"Player starting at grid position: {xPos}, {yPos}");
    }
    
    private void OnMovement(InputValue value)
    {
        movementVector = value.Get<Vector2>();

        if (movementVector == Vector2.zero)
        {
            isMovementKeyLocked = false;
            return;
        }

        if (isMovementKeyLocked) return;
        if (gameController.CurrentPlayer != player) return;
        if (gameController.CurrentPhase != GameController.TurnPhase.MOVEMENT) return;

        Debug.Log($"Movement Amount remaining: {movementAmount}");

        if (movementAmount > 0)
        {
            isMovementKeyLocked = true;

            Transform camTransform = Camera.main.transform;

            Vector3 camForward = new Vector3(camTransform.forward.x, 0, camTransform.forward.z).normalized;
            Vector3 camRight = new Vector3(camTransform.right.x, 0, camTransform.right.z).normalized;

            Vector3 intendedDirection = (camRight * movementVector.x) + (camForward * movementVector.y);

            int moveX = 0;
            int moveZ = 0;

            if (Mathf.Abs(intendedDirection.x) > Mathf.Abs(intendedDirection.z))
                moveX = intendedDirection.x > 0 ? 1 : -1;
            else
                moveZ = intendedDirection.z > 0 ? 1 : -1;

            int targetX = xPos + moveX;
            int targetZ = yPos + moveZ;

            Debug.Log($"Trying to move to: {targetX}, {targetZ}");

            if (board != null && !board.IsWalkable(targetX, targetZ))
            {
                Debug.Log($"Blocked at {targetX}, {targetZ}");
                isMovementKeyLocked = false;
                return;
            }

            xPos = targetX;
            yPos = targetZ;
            rb.MovePosition(new Vector3(targetX + 0.5f, rb.position.y, targetZ + 0.5f));
            movementAmount--;

            Debug.Log($"Moved to {targetX}, {targetZ}. Remaining: {movementAmount}");

            if (board != null && player != null)
                board.OnPlayerMoved(player, xPos, yPos);

            if (movementAmount <= 0)
                gameController.OnMovementFinished();
        }
    }

    public void SetMovementAmount(int amount) => movementAmount = amount;

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
