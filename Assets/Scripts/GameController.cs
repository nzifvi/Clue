using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class GameController : MonoBehaviour
{
    public BoardController Board;
    public DiceRoller Dice;
    private CameraController cameraController;
    private HUD hud;

    private List<Card> deck = new List<Card>();
    private List<Card> envelope = new List<Card>();

    private List<Player> players = new List<Player>();
    private List <Player> eliminated = new List<Player>();

    private int currentPlayerIndex = 0;
    private bool gameOver = false;

    public enum PlayerID { Player1, Player2, Player3, Player4, Player5, Player6 };
    public enum TurnPhase { ROLL, MOVEMENT, SUGGESTION, ACCUSATION, END };
    private TurnPhase currentPhase = TurnPhase.ROLL;

    public Player CurrentPlayer => players.Count > 0 ? players[currentPlayerIndex] : null;
    public TurnPhase CurrentPhase => currentPhase;
    public bool GameOver => gameOver;
    public IReadOnlyList<Card> Envelope => envelope.AsReadOnly();

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        players.AddRange(FindObjectsByType<Player>(FindObjectsSortMode.None));
        BuildDeck();
        DealCards();
        cameraController = FindObjectsByType<CameraController>(FindObjectsSortMode.None)[0];
        hud = FindObjectsByType<HUD>(FindObjectsSortMode.None)[0];
    }

    public void BuildDeck()
    {
        deck.Clear();
        envelope.Clear();

        var suspects = new[] { "Miss Scarlett", "Colonel Mustard", "Mrs White",
                               "Reverend Green", "Mrs Peacock", "Professor Plum" };

        var weapons = new[] { "Candlestick", "Knife", "Wrench" };

        var rooms = new[] { "Kitchen", "Ballroom", "Conservatory", "Billiard Room",
                            "Library", "Study", "Hall", "Lounge", "Dining Room" };

        var allSuspects = suspects.Select(n => new Card(n, CardType.SUSPECT)).ToList();
        var allWeapons = weapons.Select(n => new Card(n, CardType.WEAPON)).ToList();
        var allRooms = rooms.Select(n => new Card(n, CardType.ROOM)).ToList();

        envelope.Add(DrawRandom(allSuspects));
        envelope.Add(DrawRandom(allWeapons));
        envelope.Add(DrawRandom(allRooms));

        deck.AddRange(allSuspects);
        deck.AddRange(allWeapons);
        deck.AddRange(allRooms);
        Shuffle(deck);
    }

    public void DealCards()
    {
        if (players.Count == 0) return;
        int i = 0;
        foreach (Card card in deck)
        {
            players[i % players.Count].Hand.AddCard(card);
            i++;
        }
    }

    public Tuple<int,int> RollForCurrentPLayer()
    {
        if (currentPhase != TurnPhase.ROLL)
        {
            Debug.LogWarning("Not in roll phase");
            return null;
        }

        var result = Dice.RollTwoDice(6);
        int total = result.Item1 + result.Item2;
        CurrentPlayer.addMovementAmount(total);

        currentPhase = TurnPhase.MOVEMENT;
        Debug.Log($"{CurrentPlayer.PlayerName} rolled {result.Item1}+{result.Item2} = {total}");

        if (hud != null)
            hud.ShowDiceResult(result.Item1, result.Item2);

        return result;

    }

    public void RollDiceButton()
    {
        RollForCurrentPLayer();
    }

    public void OnMovementFinished()
    {
        if (currentPhase != TurnPhase.MOVEMENT) return;
        currentPhase = TurnPhase.SUGGESTION;
    }

    public Card ProcessSuggestion(Suggestion suggestion)
    {
        if (currentPhase != TurnPhase.SUGGESTION)
        {
            Debug.LogWarning("Not in suggestion phase");
            return null;
        }

        int startIndex = (currentPlayerIndex + 1) % players.Count;
        for (int i = 0; i < players.Count - 1; i++)
        {
            int idx = (startIndex + i) % players.Count;
            Player p = players[idx];
            if (eliminated.Contains(p)) continue;

            Card disproof = p.Hand.TryDisprove(
                suggestion.Suspect, suggestion.Weapon, suggestion.Room);

            if (disproof != null)
            {
                Debug.Log($"{p.PlayerName} disproves with {disproof.Name}");
                currentPhase = TurnPhase.ACCUSATION;
                return disproof;
            }
        }

        Debug.Log("No-one could disprove the suggestion");
        currentPhase = TurnPhase.ACCUSATION;
        return null;
    }

    public bool ProcessAccusation(Accusation accusation)
    {
        if (currentPhase != TurnPhase.ACCUSATION)
        {
            Debug.LogWarning("Not in accusation phase");
            return false;
        }

        bool correct = CheckAccusation(accusation);

        if (correct)
        {
            Debug.Log($"{accusation.SuggestingPlayer.PlayerName} wins!");
            gameOver = true;
        }
        else
        {
            Debug.Log($"{accusation.SuggestingPlayer.PlayerName} was wrong and is eliminated");
            eliminated.Add(accusation.SuggestingPlayer);

            if (players.Count - eliminated.Count <= 1)
            {
                Player lastStanding = players.First(p => !eliminated.Contains(p));
                Debug.Log($"{lastStanding.PlayerName} wins by elimination!");
                gameOver = true;
            }
        }

        currentPhase = TurnPhase.END;
        AdvanceTurn();
        return correct;
    }

    public void SkipAccusation()
    {
        if (currentPhase != TurnPhase.ACCUSATION) return;
        currentPhase = TurnPhase.END;
        AdvanceTurn();
    }

    private void AdvanceTurn()
    {
        if (gameOver) return;

        for (int i = 0; i < players.Count; i++)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            if (!eliminated.Contains(CurrentPlayer))
                break;
        }

        currentPhase = TurnPhase.ROLL;
        Debug.Log($"It is now {CurrentPlayer.PlayerName}'s turn");

        if (cameraController != null)
            cameraController.moveCamera(GetCurrentPlayerID());
    }

    public bool CheckAccusation(Accusation accusation)
    {
        bool suspectMatch = envelope.Any(c => c == accusation.Suspect);
        bool weaponMatch = envelope.Any(c => c == accusation.Weapon);
        bool roomMatch = envelope.Any(c => c == accusation.Room);
        return suspectMatch && weaponMatch && roomMatch;
    }

    private Card DrawRandom(List<Card> source)
    {
        int i = UnityEngine.Random.Range(0, source.Count);
        Card c = source[i];
        source.RemoveAt(i);
        return c;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public PlayerID GetCurrentPlayerID()
    {
        return (PlayerID)currentPlayerIndex;
    }
}
