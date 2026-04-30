using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DetectiveNotepad : MonoBehaviour
{
    [Header("Prefabs - drag these in from the Project window")]
    public GameObject rowPrefab; //tmp label + chiold called CellContainer pls
    public GameObject sectionHeaderPrefab; // tmp label only
    public GameObject cellPrefab; // button + tmp child pls

    [Header("Layout - drag the ScrollRect content object here")]
    public Transform contentParent;
    public Transform playerHeaderRow;
    public TextMeshProUGUI[] playerLabels; // one per playher slot drag from the header row

    public enum CellState { Unknown, HasCard, NoCard, MaybeHasCard }

    private Dictionary<string, CellState[]> noteData = new Dictionary<string, CellState[]>();
    //one tmp text reference per player
    private Dictionary<string, TextMeshProUGUI[]> cellVisuals = new Dictionary<string, TextMeshProUGUI[]>();

    private int playerCount;

    static readonly string[] Suspects = { "Col. Mustard", "Miss Scarlett", "Mrs Peacock", "Mrs White", "Prof. Plum", "Rev. Green" };
    static readonly string[] Weapons = { "Candlestick", "Knife", "Lead Pipe", "Revolver", "Rope", "Wrench" };
    static readonly string[] Rooms = { "Ballroom", "Billiard Room", "Conservatory", "Dining Room", "Hall", "Kitchen", "Library", "Lounge", "Study" };

    //called from GameController.Start()
    public void Build(int numPlayers, string[] names)
    {
        playerCount = numPlayers;

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        noteData.Clear();
        cellVisuals.Clear();

        //fill in the player name labels along the top header row
        for (int p = 0; p < numPlayers; p++)
        {
            if (p < playerLabels.Length)
                playerLabels[p].text = names[p];
        }

        SpawnSection("Suspects", Suspects);
        SpawnSection("Weapons", Weapons);
        SpawnSection("Rooms", Rooms);
    }

    void SpawnSection(string title, string[] cards)
    {
        GameObject header = Instantiate(sectionHeaderPrefab, contentParent);
        header.GetComponentInChildren<TextMeshProUGUI>().text = title;

        foreach (string card in cards)
        {
            noteData[card] = new CellState[playerCount];
            cellVisuals[card] = new TextMeshProUGUI[playerCount];

            GameObject row = Instantiate(rowPrefab, contentParent);
            row.GetComponentInChildren<TextMeshProUGUI>().text = card;

            //  rowPrefab needs a child GameObject named CellContainer
            Transform cellContainer = row.transform.Find("CellContainer");
            if (cellContainer == null)
            {
                Debug.LogError("rowPrefab is missing a child called 'CellContainer' - please add one in the Editor");
                return;
            }

            for (int p = 0; p < playerCount; p++)
            {
                //locally copy this loop variable otherwise thje button won't run when it's clicked don't ask me why claude said this was trhe correct way to do this part...
                int playerIndex = p;
                string cardName = card;

                GameObject cell = Instantiate(cellPrefab, cellContainer);

                TextMeshProUGUI cellText = cell.GetComponentInChildren<TextMeshProUGUI>();
                if (cellText == null)
                {
                    Debug.LogError("cellPrefab has no TextMeshProUGUI child - add one in the Editor");
                    return;
                }

                Button btn = cell.GetComponent<Button>();
                if (btn == null)
                {
                    Debug.LogError("cellPrefab has no Button component - add one in the Editor");
                    return;
                }

                cellVisuals[cardName][playerIndex] = cellText;
                btn.onClick.AddListener(() => CycleState(cardName, playerIndex));
                UpdateCellVisual(cellText, CellState.Unknown);
            }
        }
    }

    void CycleState(string card, int player)
    {
        CellState current = noteData[card][player];
        CellState next = (CellState)(((int)current + 1) % 4);
        noteData[card][player] = next;
        UpdateCellVisual(cellVisuals[card][player], next);
    }

    void UpdateCellVisual(TextMeshProUGUI text, CellState state)
    {
        text.text = state switch
        {
            CellState.HasCard => "✓",
            CellState.NoCard => "✗",
            CellState.MaybeHasCard => "?",
            _ => "—"
        };
        text.color = state switch
        {
            CellState.HasCard => new Color(0.23f, 0.43f, 0.07f),
            CellState.NoCard => new Color(0.47f, 0.12f, 0.12f),
            CellState.MaybeHasCard => new Color(0.39f, 0.22f, 0.02f),
            _ => new Color(0.5f, 0.5f, 0.5f)
        };
    }

    public void MarkDisproof(string cardName, int disprovingPlayerIndex)
    {
        if (!noteData.ContainsKey(cardName))
        {
            Debug.LogWarning($"MarkDisproof: card '{cardName}' not found in notepad");
            return;
        }

        noteData[cardName][disprovingPlayerIndex] = CellState.HasCard;
        UpdateCellVisual(cellVisuals[cardName][disprovingPlayerIndex], CellState.HasCard);
    }

    //Add a Clear button in the Editor if you want, allowing player to clear their own notes
    public void ClearAll()
    {
        foreach (string card in noteData.Keys)
        {
            for (int p = 0; p < playerCount; p++)
            {
                noteData[card][p] = CellState.Unknown;
                UpdateCellVisual(cellVisuals[card][p], CellState.Unknown);
            }
        }
    }
}