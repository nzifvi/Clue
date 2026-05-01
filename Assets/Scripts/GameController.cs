using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class GameController : MonoBehaviour
{
    public BoardController Board;
    public DiceManager DiceManager;
    private CameraController cameraController;
    private GameLogic gameLogic;

    private List<Card> deck = new List<Card>();
    private List<Card> envelope = new List<Card>();

    private List<Player> players = new List<Player>();
    private List <Player> eliminated = new List<Player>();

    private int currentPlayerIndex = 0;
    private bool gameOver = false;

    public enum PlayerID { Player1, Player2, Player3, Player4, Player5, Player6 };
    public enum TurnPhase { ROLL, ROLLING, MOVEMENT, SUGGESTION, ACCUSATION, END };
    private TurnPhase currentPhase = TurnPhase.ROLL;

    public Player CurrentPlayer => players.Count > 0 ? players[currentPlayerIndex] : null;
    public TurnPhase CurrentPhase => currentPhase;
    public bool GameOver => gameOver;
    public IReadOnlyList<Card> Envelope => envelope.AsReadOnly();

    [Header("Winner Controller")]
    public WinnerPanelUI winnerPanel;

    [Header("The Murder Envelope")]
    public string realMurderer;
    public string realWeapon;
    public string realRoom;

    [Header("Special Clue Cards")]
    public List<ClueCard> clueCardDeck = new List<ClueCard>();

    void Start()
    {
        int selectedPlayerCount = PlayerPrefs.GetInt("PlayerCount", 6);

        players = FindObjectsByType<Player>(FindObjectsSortMode.None)
            .OrderBy(p => p.ID)
            .ToList();

        for (int i = 0; i < players.Count; i++)
        {
            if (i >= selectedPlayerCount)
            {
                players[i].gameObject.SetActive(false);
            }
        }

        if (players.Count > selectedPlayerCount)
        {
            players.RemoveRange(selectedPlayerCount, players.Count - selectedPlayerCount);
        }

        gameLogic = new GameLogic();
        gameLogic.BuildDeck();
        BuildDeck();
        DealCards();

        DistributeWeapons();

        cameraController = FindObjectsByType<CameraController>(FindObjectsSortMode.None)[0];

        string[] activePlayerNames = new string[players.Count];
        for(int i = 0; i < players.Count; i++) {
            activePlayerNames[i] = players[i].PlayerName;
        }

        FindFirstObjectByType<DetectiveNotepad>().Build(players.Count, activePlayerNames);
        cameraController.moveCamera(GetCurrentPlayerID());
    }

    public void BuildDeck()
    {
        deck.Clear();
        envelope.Clear();

        List<Card> allSuspects = new List<Card>();

        var weapons = new[] { "Candlestick", "Dagger", "Wrench", "Lead Pipe", "Revolver", "Rope" };

        var rooms = new[] { "Kitchen", "Ballroom", "Conservatory", "Billiard Room",
                            "Library", "Study", "Hall", "Lounge", "Dining Room" };

        foreach (Player p in players)
        {
            allSuspects.Add(new Card(p.PlayerName, CardType.SUSPECT));
        }
        var allWeapons = weapons.Select(n => new Card(n, CardType.WEAPON)).ToList();
        var allRooms = rooms.Select(n => new Card(n, CardType.ROOM)).ToList();

        Card envSuspect = DrawRandom(allSuspects);
        Card envWeapon = DrawRandom(allWeapons);
        Card envRoom = DrawRandom(allRooms);

        envelope.Add(envSuspect);
        envelope.Add(envWeapon);
        envelope.Add(envRoom);

        realMurderer = envSuspect.Name;
        realWeapon = envWeapon.Name;
        realRoom = envRoom.Name;

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

    public void DistributeWeapons()
    {
        WeaponToken[] allWeaponTokens = FindObjectsByType<WeaponToken>(FindObjectsSortMode.None);
        RoomTile[] allRooms = FindObjectsByType<RoomTile>(FindObjectsSortMode.None);

        List<RoomTile> availableRooms = new List<RoomTile>(allRooms);

        foreach (WeaponToken weapon in allWeaponTokens)
        {
            if (availableRooms.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableRooms.Count);
                RoomTile assignedRoom = availableRooms[randomIndex];
                weapon.transform.position = assignedRoom.transform.position;
                weapon.CurrentRoom = assignedRoom;
                availableRooms.RemoveAt(randomIndex);
            }
        }
    }

    public void RollForCurrentPLayer()
    {
        if (currentPhase != TurnPhase.ROLL)
        {
            Debug.LogWarning("Not in roll phase");
            return;
        }
        currentPhase = TurnPhase.ROLLING;

        DiceManager.RollDice((die1, die2) =>
        {
            int total = die1 + die2;
            CurrentPlayer.addMovementAmount(total);
            currentPhase = TurnPhase.MOVEMENT;

            Debug.Log($"{CurrentPlayer.PlayerName} rolled {die1}+{die2} = {total}");

            UIManager.Instance.OnDiceRolled();
        });
    }

    public void RollDiceButton()
    {
        RollForCurrentPLayer();
    }

    public void OnMovementFinished()
    {
        if (currentPhase != TurnPhase.MOVEMENT) return;
        currentPhase = TurnPhase.SUGGESTION;

        if (CurrentPlayer.CurrentRoom == null)
        {
            // 25% chance to draw a special Clue Card in the hallway
            if (UnityEngine.Random.value > 0.75f && clueCardDeck.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, clueCardDeck.Count);
                ClueCard drawnCard = clueCardDeck[randomIndex];

                UIManager.Instance.ShowClueCard(drawnCard.CardName, drawnCard.PromptText);
                UIManager.Instance.AddLogMessage($"{CurrentPlayer.PlayerName} drew a Clue Card: {drawnCard.CardName}!");

                UIManager.Instance.rollDiceButton.gameObject.SetActive(false);
                UIManager.Instance.endTurnButton.gameObject.SetActive(false);
            }
        }
    }

    public void ProcessSuggestion(string suspect, string weapon, string room)
    {
        Card suspectCard = new Card(suspect, CardType.SUSPECT);
        Card weaponCard = new Card(weapon, CardType.WEAPON);
        Card roomCard = new Card(room, CardType.ROOM);

        Suggestion suggestion = new Suggestion(CurrentPlayer, suspectCard, weaponCard, roomCard);

        UIManager.Instance.AddLogMessage($"{CurrentPlayer.PlayerName} SUGGESTS the crime was committed by {suspect} in the {room} with the {weapon}.");

        RoomTile currentRoom = CurrentPlayer.CurrentRoom;
        if (currentRoom != null)
        {
            Player accusedPlayer = players.Find(p => p.PlayerName == suspect);
            if (accusedPlayer != null && accusedPlayer != CurrentPlayer)
            {
                accusedPlayer.GetComponent<Rigidbody>().isKinematic = true;
                accusedPlayer.transform.position = currentRoom.transform.position;
                accusedPlayer.GetComponent<Rigidbody>().isKinematic = false;
                accusedPlayer.CurrentRoom = currentRoom;
                Board.MovePlayerToRoom(accusedPlayer, currentRoom);
                UIManager.Instance.AddLogMessage($"{accusedPlayer.PlayerName} was summoned to the {currentRoom.gameObject.name}!");
            }

            WeaponToken[] allWeapons = FindObjectsByType<WeaponToken>(FindObjectsSortMode.None);
            WeaponToken accusedWeapon = System.Array.Find(allWeapons, w => w.WeaponName == weapon);

            if (accusedWeapon != null)
            {
                accusedWeapon.transform.position = currentRoom.transform.position + new Vector3(-1f, 0.5f, -1f);
                accusedWeapon.CurrentRoom = currentRoom;
            }
        }

        Card disproof = ProcessSuggestion(suggestion);

        if (disproof != null)
            UIManager.Instance.AddLogMessage($"The suggestion was DISPROVED! (A card was shown)");
        else
            UIManager.Instance.AddLogMessage($"No one could disprove the suggestion!");
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

    public void ProcessAccusation(string suspect, string weapon, string room)
    {
        if (eliminated.Contains(CurrentPlayer))
        {
            UIManager.Instance.AddLogMessage($"{CurrentPlayer.PlayerName} is already eliminated and cannot accuse!");
            return;
        }

        Card suspectCard = new Card(suspect, CardType.SUSPECT);
        Card weaponCard = new Card(weapon, CardType.WEAPON);
        Card roomCard = new Card(room, CardType.ROOM);

        Accusation accusation = new Accusation(CurrentPlayer, suspectCard, weaponCard, roomCard);
        UIManager.Instance.AddLogMessage($"{CurrentPlayer.PlayerName} ACCUSES {suspect} in the {room} with the {weapon}!");

        bool correct = CheckAccusation(accusation);

        if (correct)
        {
            UIManager.Instance.AddLogMessage($"*** {CurrentPlayer.PlayerName} WINS! The murder is solved! ***");
            Debug.Log($"{accusation.SuggestingPlayer.PlayerName} wins!");
            gameOver = true;
        }
        else
        {
            UIManager.Instance.AddLogMessage($"{CurrentPlayer.PlayerName} was WRONG and is eliminated!");
            Debug.Log($"{accusation.SuggestingPlayer.PlayerName} was wrong and is eliminated");

            eliminated.Add(accusation.SuggestingPlayer);

            if (players.Count - eliminated.Count <= 1)
            {
                Player lastStanding = players.First(p => !eliminated.Contains(p));
                UIManager.Instance.AddLogMessage($"{lastStanding.PlayerName} wins by elimination!");
                gameOver = true;
            }
        }

        currentPhase = TurnPhase.END;
        AdvanceTurn();
    }

    public void SkipAccusation()
    {
        if (currentPhase != TurnPhase.ACCUSATION) return;
        currentPhase = TurnPhase.END;
        AdvanceTurn();
    }

    public void AdvanceTurn()
    {
        Debug.Log("1. Advance Turn Button Clicked!");

        if (gameOver) return;

        for (int i = 0; i < players.Count; i++)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            if (!eliminated.Contains(CurrentPlayer))
                break;
        }

        currentPhase = TurnPhase.ROLL;
        Debug.Log($"2. It is now {CurrentPlayer.PlayerName}'s turn.");

        if (UIManager.Instance != null)
        {
            Debug.Log("3. UIManager found. Showing Pass Device Screen.");
            UIManager.Instance.ShowPassDeviceScreen(CurrentPlayer.PlayerName);
        }
        else
        {
            Debug.LogError("CRITICAL: UIManager.Instance is NULL! The UIManager script is missing or its Awake() method didn't run.");
        }

        if (cameraController != null)
        {
            Debug.Log("4. CameraController found. Moving camera.");
            cameraController.moveCamera(GetCurrentPlayerID());
        }
        else
        {
            Debug.LogError("CRITICAL: CameraController is not assigned in the GameController Inspector!");
        }
    }

    public bool CheckAccusation(Accusation accusation)
    {
        bool suspectMatch = envelope.Any(c => c.Name == accusation.Suspect.Name);
        bool weaponMatch = envelope.Any(c => c.Name == accusation.Weapon.Name);
        bool roomMatch = envelope.Any(c => c.Name == accusation.Room.Name);

        return suspectMatch && weaponMatch && roomMatch;
    }

    public void MakeAccusation(Player player, Card suspect, Card weapon, Card room)
    {
        bool isCorrect = gameLogic.CheckAccusation(suspect, weapon, room);

        if (isCorrect)
        {
            winnerPanel.ShowWinner(player, suspect, weapon, room);
            UIManager.Instance.AddLogMessage($"{player.PlayerName} won the game!");
        }
        else
        {
            UIManager.Instance.AddLogMessage($"{player.PlayerName} made a false accusation and is out!");
        }
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
