
public class Weapon
{
    public string Name { get; private set; }
    public Card Card { get; private set; }

    public Weapon(string name, Card card)
    {
        Name = name;
        Card = card;
    }
}
