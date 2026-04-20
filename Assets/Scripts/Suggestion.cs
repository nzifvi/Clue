using UnityEngine;

public class Suggestion
{
    public Player SuggestingPlayer { get; private set; }
    public Card Suspect { get; private set; }
    public Card Weapon { get; private set; }
    public Card Room { get; private set; }

    public Suggestion(Player player, Card suspect, Card weapon, Card room)
    {
        SuggestingPlayer = player;
        Suspect = suspect;
        Weapon = weapon;
        Room = room;
    }
}
