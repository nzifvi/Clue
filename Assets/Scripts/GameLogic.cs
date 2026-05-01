using System;
using System.Collections.Generic;
using System.Linq;

public class GameLogic
{
    public List<Card> Envelope { get; } = new List<Card>();
    public List<Card> Deck { get; } = new List<Card>();

    private readonly Func<int,int> _rng;

    public GameLogic(Func<int,int> rng = null)
    {
        _rng = rng ?? (max => new System.Random().Next(0, max));
    }

    public void BuildDeck()
    {
        Envelope.Clear();
        Deck.Clear();

        var suspects = new[] { "Miss Scarlett","Colonel Mustard","Mrs White",
                               "Reverend Green","Mrs Peacock","Professor Plum" }
                       .Select(n => new Card(n, CardType.SUSPECT)).ToList();
        var weapons  = new[] { "Candlestick","Dagger","Lead Pipe",
                               "Revolver","Rope","Wrench" }
                       .Select(n => new Card(n, CardType.WEAPON)).ToList();
        var rooms    = new[] { "Kitchen","Ballroom","Conservatory","Billiard Room",
                               "Library","Study","Hall","Lounge","Dining Room" }
                       .Select(n => new Card(n, CardType.ROOM)).ToList();

        Envelope.Add(DrawRandom(suspects));
        Envelope.Add(DrawRandom(weapons));
        Envelope.Add(DrawRandom(rooms));

        Deck.AddRange(suspects);
        Deck.AddRange(weapons);
        Deck.AddRange(rooms);
    }

    public void DealCards(List<Hand> hands)
    {
        if (hands.Count == 0) return;
        int i = 0;
        foreach (Card card in Deck) { hands[i++ % hands.Count].AddCard(card); }
    }

    public bool CheckAccusation(Card suspect, Card weapon, Card room)
    {
        return Envelope.Any(c => c == suspect)
        && Envelope.Any(c => c == weapon)
        && Envelope.Any(c => c == room);
    }

    public Card ProcessSuggestion (List<Hand> otherHands, Card suspect, Card weapon, Card room)
    {
        foreach (Hand hand in otherHands)
        {
            Card d = hand.TryDisprove(suspect, weapon, room);
            if (d != null) return d;
        }
        return null;
    }

    private Card DrawRandom(List<Card> source)
    {
        int i = _rng(source.Count);
        Card c = source[i];
        source.RemoveAt(i);
        return c;
    }
}
