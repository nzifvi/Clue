using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject GameController;
    GameObject reference = null;
    int matrixX;
    int matrixY;

    public bool isEnteringRoom = false;
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




