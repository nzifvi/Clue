using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CameraControllerTest
{
    private GameObject game;
    private CameraController cameraController;
    // A Test behaves as an ordinary method
    [SetUp]
    public void SetUp()
    {
        game = new GameObject(
            "Camera"
        );
        cameraController = game.AddComponent<CameraController>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(
            game
        );
    }

    [UnityTest]
    public IEnumerator testRoundStartPosition()
    {
        // assuming each round starts with player 1
        yield return null;

        float theta = 227f * Mathf.Deg2Rad;
        Vector3 expectedPos = new Vector3(
            410f * Mathf.Cos(theta),
            160f,
            410f * Mathf.Sin(theta)
        );
        
        Assert.AreEqual(expectedPos.x, game.transform.position.x, 0.01f);
        Assert.AreEqual(expectedPos.y, game.transform.position.y, 0.01f);
        Assert.AreEqual(expectedPos.z, game.transform.position.z, 0.01f);
    }

    [UnityTest]
    public IEnumerator testBoardViewAngle()
    {
        yield return null;

        cameraController.showBoardView();

        Quaternion expectedRotation = Quaternion.Euler(
            90,
            0,
            0
        );

        Assert.AreEqual(
            0f,
            Quaternion.Angle(game.transform.rotation, expectedRotation),
            0.01f
        );
        
        
    }

    [UnityTest]
    public IEnumerator testBoardViewPosition()
    {
        yield return null;
        
        cameraController.showBoardView();
        
        Assert.AreEqual(0f, game.transform.position.x, 0.01f);
        Assert.AreEqual(800f, game.transform.position.y, 0.01f);
        Assert.AreEqual(0f, game.transform.position.z, 0.01f);
    }

    [UnityTest]
    public IEnumerator testPlayerToBoardView()
    {
        yield return null;
        
        cameraController.showBoardView();
        cameraController.moveCamera(GameController.PlayerID.Player1);

        yield return new WaitForSeconds(10f);
        
        Assert.IsFalse(cameraController.isCameraMoving());
        Assert.AreEqual(160f, game.transform.position.y, 0.01f);
        
    }

    [UnityTest]
    public IEnumerator testPlayerToPlayerCameraBehaviour()
    {
        yield return null;

        var players = new GameController.PlayerID[]
        {
            GameController.PlayerID.Player1,
            GameController.PlayerID.Player2,
            GameController.PlayerID.Player3,
            GameController.PlayerID.Player4,
            GameController.PlayerID.Player5,
            GameController.PlayerID.Player6,
            GameController.PlayerID.Player1
        };

        foreach (GameController.PlayerID player in players)
        {
            // 1 -> 2 => 2 -> 3 => 3 -> 4 => 4 -> 5 => 5 -> 6 => 6 -> 1
            cameraController.moveCamera(player);
            yield return new WaitForSeconds(10f);

            var package = cameraController.getCameraMovementPackage(player);
            float theta = package.yAngle * Mathf.Deg2Rad;

            Assert.AreEqual(
                410f * Mathf.Cos(theta),
                game.transform.position.x,
                0.01f
            );
            Assert.AreEqual(
                160f,
                game.transform.position.y,
                0.01f
            );
            Assert.AreEqual(
                410f * Mathf.Sin(theta),
                game.transform.position.z,
                0.01f
            );
        }
    }
}
