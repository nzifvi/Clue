using UnityEngine;

public enum CardType { SUSPECT, WEAPON, ROOM }

public class Card
{
    public string Name { get; private set; }
    public CardType Type { get; private set; }

    public Card(string name, CardType type)
    {
        Name = name;
        Type = type;
    }
}
