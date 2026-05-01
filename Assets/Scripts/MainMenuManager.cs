using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGameWithPlayers(int numberOfPlayers)
    {
        PlayerPrefs.SetInt("PlayerCount", numberOfPlayers);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainGame");
    }
}