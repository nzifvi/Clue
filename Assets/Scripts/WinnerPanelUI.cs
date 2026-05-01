using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinnerPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelContainer;
    public TextMeshProUGUI winnerNameText;
    public TextMeshProUGUI solutionText;
    public Image winnerPortrait;

    public void ShowWinner(Player winner, Card suspect, Card weapon, Card room)
    {
        panelContainer.SetActive(true);
        winnerNameText.text = $"{winner.PlayerName} Solved the Crime!";
        solutionText.text = $"It was {suspect.Name} in the {room.Name} with the {weapon.Name}.";

        UIManager.Instance.CloseActionMenu();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}