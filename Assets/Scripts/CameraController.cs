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

    private float radius = 22.5f;
    private float height = 22.5f;
    private float tangentialSpeed = 50.0f;
    private float currentYAngle = 0f;
    private bool isBirdseye = false;
    private Transform currentPlayerTarget;
    public Vector3 birdseyePosition = new Vector3(12f, 30f, 13f);
    public Vector3 birdseyeRotation = new Vector3(90, 0, 0);



    void Start()
    {
        playerPositions.Add(
            GameController.PlayerID.Player1,
            new CameraMovementPackage(23.5f, 20, 7.5f, 25, -90, 0)
        );
        playerPositions.Add(
            GameController.PlayerID.Player2,
            new CameraMovementPackage(16.5f, 20, 0.5f, 25, 0, 0)
        );
        playerPositions.Add(
            GameController.PlayerID.Player3,
            new CameraMovementPackage(0.5f, 20, 11.5f, 25, 90, 0)
        );
        playerPositions.Add(
            GameController.PlayerID.Player4,
            new CameraMovementPackage(0.5f, 20, 16.5f, 25, 90, 0)
        );
        playerPositions.Add(
            GameController.PlayerID.Player5,
            new CameraMovementPackage(5.5f, 20, 25.5f, 25, 180, 0)
        );
        playerPositions.Add(
            GameController.PlayerID.Player6,
            new CameraMovementPackage(18.5f, 20, 25.5f, 25, 180, 0)
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

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleBirdseyeView(!isBirdseye);
        }

        if (isMoving && !isBirdseye)
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
        if (!isBirdseye)
        {
            currentPlayer = playerID;
            isMoving = true;
        }
    }

    private void updateCameraPosition()
    {
        float theta = currentYAngle * Mathf.Deg2Rad;
        Vector3 boardCentre = new Vector3(12f, 5f, 13f);
        Vector3 camPos = new Vector3(
            boardCentre.x + radius * Mathf.Cos(theta),
            height,
            boardCentre.z + radius * Mathf.Sin(theta)
        );
        transform.position = camPos;
        transform.LookAt(boardCentre);
    }

    public void ToggleBirdseyeView(bool active)
    {
        isBirdseye = active;
        isMoving = false; // Stop any current rotation animation

        if (isBirdseye)
        {
            transform.position = birdseyePosition;
            transform.rotation = Quaternion.Euler(birdseyeRotation);
        }
        else
        {
            // Snap back to current player's angle
            currentYAngle = playerPositions[currentPlayer].yAngle;
            updateCameraPosition();
        }
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