using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GridManager : MonoBehaviour
{
    private const int rowLength = 10;
    private const int colLength = 40;
    private List<List<GameObject>> grid = new  List<List<GameObject>>();
    
    void Start()
    {
        for (int i = 0; i < rowLength; i++)
        {
            grid.Add(new List<GameObject>());
            for (int j = 0; j < colLength; j++)
            {
                GameObject g = new GameObject();
                g.name = "Tile" + i + j;
                g.AddComponent<TraversableTile>();
                
                g.transform.position = new Vector3(20 * i, 20 * j, 0);
                g.transform.localScale = new Vector3(10, 10, 0);
                
                SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("TTTestTile");
                grid[i].Add(g);
            }
        }
    }
    
    void Update()
    { 
        
    }

}

