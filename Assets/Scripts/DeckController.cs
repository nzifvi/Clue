using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class DeckController : MonoBehaviour
{
    private const int weaponCardAmount = 6;
    private const int characterCardAmount = 6;
    private const int roomCardCount = 9;

    private Dictionary<GameController.PlayerID, List<Card>> decks = new Dictionary<GameController.PlayerID, List<Card>>();
    private List<ClueCard> clueCards = new List<ClueCard>();
    
    void Start()
    {
        List<Card> unshuffledCards = new List<Card>();
        // get cards from game via GetObjectsOfType function (do later)
        
        
    }
    
    void Update()
    {
        
    }

    private List<Card> riffleShuffle(List<Card> cards)
    {
        Stack<Card> stack1 = new Stack<Card>();
        Stack<Card> stack2 = new Stack<Card>();
        
        for (int i = 0; i < cards.Count; i++)
        {
            if (i < cards.Count / 2)
            {
                stack1.Push(cards[i]);
            }
            else
            {
                stack2.Push(cards[i]);
            }
        }
        cards.Clear();
        int j = 0;
        while (stack1.Count > 0 && stack2.Count > 0)
        {
            if (j % 2 == 0 && stack1.Count > 0)
            {
                cards.Add(stack1.Pop());
            }
            else if(j % 2 != 0 && stack2.Count > 0)
            {
                cards.Add(stack2.Pop());
            }
            j++;
        }
        
        return cards;
    }
}
