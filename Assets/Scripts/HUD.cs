using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public GameController gameController;

    public TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI currentPhaseText;
    public TextMeshProUGUI diceResultText;

    public void ShowDiceResult(int die1, int die2)
    {
        diceResultText.text = $"Rolled: {die1} + {die2} = {die1 + die2}";
    }

    public GameObject rollDice;
    public GameObject endMovement;
    public GameObject skipAccusation;
    public SuggestionUI suggestionUI;
    private GameController.TurnPhase lastPhase;

    void Update()
    {
        if (gameController == null) return;

        currentPlayerText.text = $"Current Player: {gameController.CurrentPlayer?.PlayerName ?? "None"}";
        currentPhaseText.text = $"Phase: {gameController.CurrentPhase}";

        rollDice.SetActive(gameController.CurrentPhase == GameController.TurnPhase.ROLL
            || gameController.CurrentPhase == GameController.TurnPhase.ROLLING);
        endMovement.SetActive(false);
        skipAccusation.SetActive(gameController.CurrentPhase == GameController.TurnPhase.ACCUSATION);

        if (gameController.CurrentPhase == GameController.TurnPhase.SUGGESTION
            && lastPhase != GameController.TurnPhase.SUGGESTION)
        {
            suggestionUI.ShowSuggestionPanel();
        }

        lastPhase = gameController.CurrentPhase;
    }
}
