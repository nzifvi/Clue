using UnityEngine;
using System;
using System.Collections.Generic;

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
    private PlayerID currentPlayer = PlayerID.Player1;
    private Rigidbody positionVector;
    void Start()
    {
        playerPositions.Add(
            PlayerID.Player1,
            new CameraMovementPackage(-310, 60, -310, 0, 45, 0)
            );
        playerPositions.Add(
            PlayerID.Player2,
            new CameraMovementPackage(-440, 60, 0, 0, 0, 0)
            );
        playerPositions.Add(
            PlayerID.Player3,
            new CameraMovementPackage(-310, 60, 310, 0, 45, 0)
            );
        playerPositions.Add(
            PlayerID.Player4,
            new CameraMovementPackage(310, 60, 310, 0, 45, 0)
            );
        playerPositions.Add(
            PlayerID.Player5,
            new CameraMovementPackage(440, 60, 0, 0, 0, 0)
            );
        playerPositions.Add(
            PlayerID.Player6,
            new CameraMovementPackage(310, 60, -310, 0, 45, 0)
            );


        positionVector = GetComponent<Rigidbody>();
        positionVector.useGravity = false;

        positionVector.position = new Vector3(
            playerPositions[currentPlayer].xPos,
            playerPositions[currentPlayer].yPos,
            playerPositions[currentPlayer].zPos
        );

    }
    
    void Update()
    {
        moveCamera(PlayerID.Player2);
    }

    public void moveCamera(PlayerID newPlayer)
    {
        currentPlayer = newPlayer;

        positionVector.position = new Vector3(
            playerPositions[currentPlayer].xPos,
            playerPositions[currentPlayer].yPos,
            playerPositions[currentPlayer].zPos
        );

        Vector3 angularVelocity = new Vector3(0, 100, 0);
        
        Quaternion rotationDifference = new Quaternion(
            angularVelocity.x * Time.fixedDeltaTime,
            angularVelocity.y * Time.fixedDeltaTime,
            angularVelocity.z * Time.fixedDeltaTime,
            1
            );
        
        positionVector.MoveRotation(positionVector.rotation * rotationDifference);
    }
}
