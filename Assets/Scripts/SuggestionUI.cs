using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SuggestionUI : MonoBehaviour
{
    public GameController gameController;

    public TMP_Dropdown suspectDropdown;
    public TMP_Dropdown weaponDropdown;
    public TMP_Dropdown roomDropdown;
    public TextMeshProUGUI resultText;

    void Start()
    {
        suspectDropdown.ClearOptions();
        suspectDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Miss Scarlett", "Colonel Mustard", "Mrs White",
            "Reverend Green", "Mrs Peacock", "Professor Plum"
        });

        weaponDropdown.ClearOptions();
        weaponDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Candlestick", "Dagger", "Wrench", "Rope", "Revolver", "Lead Pipe"
        });

        roomDropdown.ClearOptions();
        roomDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Kitchen", "Ballroom", "Conservatory", "Billiard Room",
            "Library", "Study", "Hall", "Lounge", "Dining Room"
        });
    }

    public void OnSubmitSuggestion()
    {
        Player currentPlayer = gameController.CurrentPlayer;

        Card suspect = new Card(suspectDropdown.options[suspectDropdown.value].text, CardType.SUSPECT);
        Card weapon = new Card(weaponDropdown.options[weaponDropdown.value].text, CardType.WEAPON);
        Card room = new Card(roomDropdown.options[roomDropdown.value].text, CardType.ROOM);

        Suggestion suggestion = new Suggestion(currentPlayer, suspect, weapon, room);
        Card disproof = gameController.ProcessSuggestion(suggestion);

        if (disproof != null)
            resultText.text = $"Disproved! A player showed: {disproof.Name}";
        else
            resultText.text = "No one could disprove your suggestion!";

        GameObject accusedPlayerObj = GameObject.Find(suspect.Name);
        GameObject roomObj = GameObject.Find(room.Name);

        if(accusedPlayerObj != null && roomObj != null)
         {
             accusedPlayerObj.transform.position = roomObj.transform.position + new Vector3(0, 1, 0);
         }

        GameObject weaponObj = GameObject.Find(weapon.Name);
        if(weaponObj != null && roomObj != null)
        {
           weaponObj.transform.position = roomObj.transform.position + new Vector3(0, 0.5f, 0);
        }

        Invoke(nameof(HidePanel), 3f);
    }

    public void OnSkipSuggestion()
    {
        gameController.OnMovementFinished();
        gameController.SkipAccusation();
        HidePanel();
    }
    public void HidePanel()
    {
        UIManager.Instance.CloseActionMenu();
    }


}