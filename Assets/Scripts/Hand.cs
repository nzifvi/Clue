using UnityEngine;
using System.Collections.Generic;

public class Hand
{
    public List<Card> Cards { get; private set; } = new List<Card>();

    public void AddCard(Card card) => Cards.Add(card);
    public bool HasCard(Card card) => Cards.Contains(card);

    public Card TryDisprove(Card suspect, Card weapon, Card room)
    {
        foreach (Card c in Cards)
            if (c == suspect || c == weapon || c == room)
                return c;
        return null;
    }
}
