using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class GameLogicTests
{
    [Test]
    public void BuildDeck_EnvelopeContainsExactlyThreeCards()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();
        Assert.AreEqual(3, logic.Envelope.Count);
    }

    [Test]
    public void BuildDeck_EnvelopeOneSuspectOneWeaponOneRoom()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();

        Assert.AreEqual(1, logic.Envelope.Count(c => c.Type == CardType.SUSPECT));
        Assert.AreEqual(1, logic.Envelope.Count(c => c.Type == CardType.WEAPON));
        Assert.AreEqual(1, logic.Envelope.Count(c => c.Type == CardType.ROOM));
    }


    [Test]
    public void BuildDeck_DeckContainsTwentyOneMimusThreeCards()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();
        Assert.AreEqual(18, logic.Deck.Count);
    }

    [Test]
    public void BuildDeck_EnvelopeCardsNotInDeck()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();
        foreach (Card envelopCard in logic.Envelope)
            Assert.IsFalse(logic.Deck.Contains(envelopCard));
    }


    [Test]
    public void DealCards_AllDeckCardsDealtAcrossHands()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();

        var hands = new List<Hand> { new Hand(), new Hand(), new Hand() };
        logic.DealCards(hands);
        int totalDealt = hands.Sum(h => h.Cards.Count);
        Assert.AreEqual(18, totalDealt);
    }

    [Test]
    public void DealCards_CardsDistributedEvenly_WhenDivisible()
    {

        var logic = new GameLogic(max => 0);
        logic.BuildDeck();
        var hands = new List<Hand> { new Hand(), new Hand(), new Hand() };
        logic.DealCards(hands);
        Assert.AreEqual(6, hands[0].Cards.Count);
        Assert.AreEqual(6, hands[1].Cards.Count);
        Assert.AreEqual(6, hands[2].Cards.Count);
    }




    [Test]
    public void CheckAccusation_ReturnsTrue_WhenAllThreeMatch()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();

        Card s = logic.Envelope.First(c => c.Type == CardType.SUSPECT);
        Card w = logic.Envelope.First(c => c.Type == CardType.WEAPON);
        Card r = logic.Envelope.First(c => c.Type == CardType.ROOM);

        Assert.IsTrue(logic.CheckAccusation(s, w, r));
    }

    [Test]
    public void CheckAccusation_ReturnsFalse_WhenSuspectWrong()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();

        Card wrongSuspect = logic.Deck.First(c => c.Type == CardType.SUSPECT);
        Card w = logic.Envelope.First(c => c.Type == CardType.WEAPON);
        Card r = logic.Envelope.First(c => c.Type == CardType.ROOM);

        Assert.IsFalse(logic.CheckAccusation(wrongSuspect, w, r));
    }

    [Test]
    public void CheckAccusation_ReturnsFalse_WhenWeaponWrong()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();

        Card s = logic.Envelope.First(c => c.Type == CardType.SUSPECT);
        Card wrongWeapon = logic.Deck.First(c => c.Type == CardType.WEAPON);
        Card r = logic.Envelope.First(c => c.Type == CardType.ROOM);

        Assert.IsFalse(logic.CheckAccusation(s, wrongWeapon, r));
    }




    [Test]
    public void ProcessSuggestion_ReturnsNull_WhenNoOneCanDisprove()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();


        var hand1 = new Hand();
        var hand2 = new Hand();


        Card s = logic.Envelope.First(c => c.Type == CardType.SUSPECT);
        Card w = logic.Envelope.First(c => c.Type == CardType.WEAPON);
        Card r = logic.Envelope.First(c => c.Type == CardType.ROOM);

        Card result = logic.ProcessSuggestion(
            new List<Hand> { hand1, hand2 }, s, w, r);

        Assert.IsNull(result);
    }

    [Test]
    public void ProcessSuggestion_ReturnsCard_WhenPlayerCanDisprove()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();

        Card s = logic.Envelope.First(c => c.Type == CardType.SUSPECT);
        Card w = logic.Envelope.First(c => c.Type == CardType.WEAPON);
        Card r = logic.Envelope.First(c => c.Type == CardType.ROOM);

        var hand = new Hand();
        hand.AddCard(s);

        Card result = logic.ProcessSuggestion(new List<Hand> { hand }, s, w, r);

        Assert.AreEqual(s, result);
    }

    [Test]
    public void ProcessSuggestion_AsksPlayersInOrder_ReturnsFirst()
    {
        var logic = new GameLogic(max => 0);
        logic.BuildDeck();

        Card s = logic.Deck.First(c => c.Type == CardType.SUSPECT);
        Card w = logic.Deck.First(c => c.Type == CardType.WEAPON);
        Card r = logic.Deck.First(c => c.Type == CardType.ROOM);

        var hand1 = new Hand(); hand1.AddCard(s);
        var hand2 = new Hand(); hand2.AddCard(w);

        Card result = logic.ProcessSuggestion(new List<Hand> { hand1, hand2 }, s, w, r);

        Assert.AreEqual(s, result);
    }




    [Test]
    public void Hand_TryDisprove_ReturnsNull_WhenHandEmpty()
    {
        var hand = new Hand();
        var s = new Card("Miss Scarlett", CardType.SUSPECT);
        var w = new Card("Knife", CardType.WEAPON);
        var r = new Card("Kitchen", CardType.ROOM);
        Assert.IsNull(hand.TryDisprove(s, w, r));
    }

    [Test]
    public void Hand_TryDisprove_ReturnsMatchingCard()
    {
        var knife = new Card("Knife", CardType.WEAPON);
        var hand = new Hand();
        hand.AddCard(knife);

        var s = new Card("Miss Scarlett", CardType.SUSPECT);
        var r = new Card("Kitchen", CardType.ROOM);

        Assert.AreEqual(knife, hand.TryDisprove(s, knife, r));
    }
}
