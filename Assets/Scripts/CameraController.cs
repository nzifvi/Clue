using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public struct CameraMovementPackage
    {
        public float xPos;
        public float yPos;
        public float zPos;
        public float xAngle;
        public float yAngle;
        public float zAngle;

        public CameraMovementPackage(float xPos, float yPos, float zPos, float xAngle, float yAngle, float zAngle)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.zPos = zPos;
            this.xAngle = xAngle;
            this.yAngle = yAngle;
            this.zAngle = zAngle;
        }
    }

    private Dictionary<GameController.PlayerID, CameraMovementPackage> playerPositions = new Dictionary<GameController.PlayerID, CameraMovementPackage>();
    private GameController.PlayerID currentPlayer;

    private bool isMoving = false;

    private float radius = 10f;
    private float height = 20f;
    private float tangentialSpeed = 50.0f;
    private float currentYAngle = 0f;


    void Start()
    {
        playerPositions.Add(
            GameController.PlayerID.Player1,
            new CameraMovementPackage(0, 20, 0, 25, 227, 0)
        );
        playerPositions.Add(
            GameController.PlayerID.Player2,
            new CameraMovementPackage(0, 20, 0, 25, 185, 0)
        );
        playerPositions.Add(
            GameController.PlayerID.Player3,
            new CameraMovementPackage(0, 20, 0, 25, 135, 0)
        );
        playerPositions.Add(
            GameController.PlayerID.Player4,
            new CameraMovementPackage(0, 20, 0, 25, 90, 0)
        );
        playerPositions.Add(
            GameController.PlayerID.Player5,
            new CameraMovementPackage(0, 20, 0, 25, 45, 0)
        );
        playerPositions.Add(
            GameController.PlayerID.Player6,
            new CameraMovementPackage(0, 20, 0, 25, 5, 0)
        );

        currentPlayer = GameController.PlayerID.Player1;
        currentYAngle = playerPositions[currentPlayer].yAngle;

        updateCameraPosition();
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            moveCamera(GameController.PlayerID.Player1);
        }else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            moveCamera(GameController.PlayerID.Player2);
        }else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            moveCamera(GameController.PlayerID.Player3);
        }else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            moveCamera(GameController.PlayerID.Player4);
        }else if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            moveCamera(GameController.PlayerID.Player5);
        }else if (Keyboard.current.digit6Key.wasPressedThisFrame)
        {
            moveCamera(GameController.PlayerID.Player6);
        }

        if (isMoving)
        {
            float targetYAngle = playerPositions[currentPlayer].yAngle;
            currentYAngle = Mathf.MoveTowardsAngle(currentYAngle, targetYAngle, tangentialSpeed * Time.deltaTime );

            updateCameraPosition();


            if (Math.Abs(Mathf.DeltaAngle(currentYAngle, targetYAngle)) < 0.01f)
            {
                isMoving = false;
                currentYAngle = targetYAngle;
            }
        }
    }

    public void moveCamera(GameController.PlayerID playerID)
    {
        currentPlayer = playerID;
        isMoving = true;
    }

    private void updateCameraPosition()
    {
        float theta = currentYAngle * Mathf.Deg2Rad;
        Vector3 boardCentre = new Vector3(19.5f, 0, 4.5f);
        Vector3 camPos = new Vector3(
            boardCentre.x + radius * Mathf.Cos(theta),
            height,
            boardCentre.z + radius * Mathf.Sin(theta)
        );
        transform.position = camPos;
        transform.LookAt(boardCentre);
    }

    public void showBoardView()
    {
        transform.position = new Vector3(
            19.5f, 35f, 4.5f
        );
        transform.rotation = Quaternion.Euler(
            90, 0, 0
        );
    }

    public bool isCameraMoving()
    {
        return isMoving;
    }

    public CameraMovementPackage getCameraMovementPackage(GameController.PlayerID playerID)
    {
        return playerPositions[playerID];
    }
}