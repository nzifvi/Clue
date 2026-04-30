using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("References")]
    public GameController gameController;

    [Header("Info bar")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI movesLeftText;
    public TextMeshProUGUI roomNameText;

    [Header("Phase indicators")]
    public Button[] phaseButtons;
    public Color phaseActive = new Color(0.98f, 0.93f, 0.85f);
    public Color phaseDone = new Color(0.91f, 0.95f, 0.87f);
    public Color phaseInactive = new Color(0.96f, 0.96f, 0.96f);

    [Header("Panels")]
    public GameObject dicePanel;
    public GameObject suggestionPanel;
    public GameObject accusationPanel;

    [Header("Dice")]
    public TextMeshProUGUI die1Text;
    public TextMeshProUGUI die2Text;
    public TextMeshProUGUI diceTotalText;

    [Header("Hand")]
    public Transform handContainer;
    public GameObject cardChipPrefab;

    private GameController.TurnPhase lastPhase;

    //cp = currentPlayer and pm = playerMovement, just to save some typing
    void Update()
    {
        if (gameController == null) return;

        Player cp = gameController.CurrentPlayer;

        playerNameText.text = cp?.PlayerName ?? "—";

        //Add public int MovesRemaining => movementAmount; in PlayerMovement.cs anywhere is fine tbh
        PlayerMovement pm = cp?.GetComponent<PlayerMovement>();
        movesLeftText.text = pm != null ? pm.MovesRemaining.ToString() : "—";

        roomNameText.text = cp?.CurrentRoom?.RoomName ?? "Corridor";

        var phase = gameController.CurrentPhase;
        dicePanel.SetActive(phase == GameController.TurnPhase.ROLL);
        accusationPanel.SetActive(phase == GameController.TurnPhase.ACCUSATION);

        if (phase == GameController.TurnPhase.SUGGESTION && lastPhase != GameController.TurnPhase.SUGGESTION)
            suggestionPanel.SetActive(true);

        UpdatePhaseBar(phase);
        lastPhase = phase;
    }

    void UpdatePhaseBar(GameController.TurnPhase phase)
    {
        int current = (int)phase;
        for (int i = 0; i < phaseButtons.Length; i++)
        {
            var img = phaseButtons[i].GetComponent<Image>();
            if (i < current) img.color = phaseDone;
            else if (i == current) img.color = phaseActive;
            else img.color = phaseInactive;
        }
    }

    public void ShowDiceResult(int die1, int die2)
    {
        die1Text.text = die1.ToString();
        die2Text.text = die2.ToString();
        diceTotalText.text = $"Rolled: {die1} + {die2} = {die1 + die2}";
    }

    public void UpdateHand(Hand hand)
    {
        foreach (Transform child in handContainer)
            Destroy(child.gameObject);

        foreach (Card card in hand.Cards)
        {
            GameObject chip = Instantiate(cardChipPrefab, handContainer);

            var img = chip.GetComponent<Image>();
            if (card.CardImage != null)
            {
                img.sprite = card.CardImage;
                img.color = Color.white;
                chip.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
            else
            {
                chip.GetComponentInChildren<TextMeshProUGUI>().text = card.Name;
                img.color = new Color(0.10f, 0.13f, 0.35f);
            }
        }
    }
}