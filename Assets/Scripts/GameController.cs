using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private List<Player> playersArray = new List<Player>();
    private int currentPlayer = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playersArray.AddRange(FindObjectsOfType<Player>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
