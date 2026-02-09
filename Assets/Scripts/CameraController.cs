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
    
    public enum PlayerID {
        Player1 = 1,
        Player2,
        Player3,
        Player4,
        Player5,
        Player6
    }
    private Dictionary<PlayerID, CameraMovementPackage> playerPositions = new Dictionary<PlayerID, CameraMovementPackage>();
    private PlayerID currentPlayer;
    
    private bool isMoving = false;
    
    private float radius = 410f;
    private float height = 160f;
    private float tangentialSpeed = 50.0f;
    private float rotationalSpeed = 5.0f;
    private float currentYAngle = 0f;
    
    
    void Start()
    {
        playerPositions.Add(
            PlayerID.Player1,
            new CameraMovementPackage(-310, 180, -310, 45, 45, 0)
        );
        playerPositions.Add(
            PlayerID.Player2,
            new CameraMovementPackage(-440, 180, 0, 45, 90, 0)
        );
        playerPositions.Add(
            PlayerID.Player3,
            new CameraMovementPackage(-310, 180, 310, 45, 135, 0)
        );
        playerPositions.Add(
            PlayerID.Player4,
            new CameraMovementPackage(310, 180, 310, 45, 180, 0)
        );
        playerPositions.Add(
            PlayerID.Player5,
            new CameraMovementPackage(440, 180, 0, 45, 225, 0)
        );
        playerPositions.Add(
            PlayerID.Player6,
            new CameraMovementPackage(310, 180, -310, 45, 270, 0)
        );
        
        currentPlayer = PlayerID.Player1;
        currentYAngle = playerPositions[currentPlayer].yAngle;

        updateCameraPosition();
    }
    
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            moveCamera(PlayerID.Player1);
        }else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            moveCamera(PlayerID.Player2);
        }else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            moveCamera(PlayerID.Player3);
        }else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            moveCamera(PlayerID.Player4);
        }else if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            moveCamera(PlayerID.Player5);
        }else if (Keyboard.current.digit6Key.wasPressedThisFrame)
        {
            moveCamera(PlayerID.Player6);
        }

        if (isMoving)
        {
            float targetYAngle = playerPositions[currentPlayer].yAngle;
            currentYAngle = Mathf.MoveTowardsAngle(currentYAngle, targetYAngle, tangentialSpeed * Time.deltaTime );
            
            updateCameraPosition();

            Quaternion targetRotation = Quaternion.Euler(
                playerPositions[currentPlayer].xAngle,
                playerPositions[currentPlayer].yAngle,
                playerPositions[currentPlayer].zAngle
            );
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationalSpeed * Time.deltaTime);
            
            if (
                Math.Abs(Mathf.DeltaAngle(currentYAngle, targetYAngle)) < 0.01f
                &&
                Quaternion.Angle(transform.rotation, targetRotation) < 0.01f
            )
            {
                isMoving = false;
                currentYAngle = targetYAngle;
                transform.rotation = targetRotation;
            }
        }
    }

    public void moveCamera(PlayerID playerID)
    {
        currentPlayer = playerID;
        isMoving = true;
    }

    private void updateCameraPosition()
    {
        float theta = currentYAngle * Mathf.Deg2Rad;
        Vector3 camPos = new Vector3(
            radius * Mathf.Cos(theta),
            height,
            radius * Mathf.Sin(theta)
        );
        transform.position = camPos;
    }
}
