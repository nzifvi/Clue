using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject passDevicePanel;
    public GameObject mainHudPanel;
    public GameObject promptPanel;
    public GameObject actionMenuPanel;
    public GameObject notepadPanel;

    [Header("Main HUD Buttons")]
    public Button rollDiceButton;
    public Button endTurnButton;
    public Button accuseButton;
    public Button cameraToggleButton;

    [Header("Player Hand")]
    public Transform handContainer;
    public GameObject cardChipPrefab;

    [Header("Action Menu Dropdowns")]
    public TMP_Dropdown suspectDropdown;
    public TMP_Dropdown weaponDropdown;
    public TMP_Dropdown roomDropdown;

    private bool isCurrentActionAccusation;

    [Header("Dynamic Text")]
    public TextMeshProUGUI playerTurnText;
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI actionMenuTitle;

    [Header("Camera Reference")]
    public CameraController mainCameraController;

    [Header("Game Log")]
    public GameObject gameLogPanel;
    public TextMeshProUGUI gameLogText;

    [Header("Clue Cards UI")]
    public GameObject clueCardPanel;
    public TextMeshProUGUI clueCardNameText;
    public TextMeshProUGUI clueCardPromptText;

    private bool isBirdseyeView = false;

    private void Awake()
    {

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- 1. TURN TRANSITIONS ---
    public void ShowPassDeviceScreen(string playerName)
    {
        mainHudPanel.SetActive(false);
        promptPanel.SetActive(false);
        actionMenuPanel.SetActive(false);
        notepadPanel.SetActive(false);
        passDevicePanel.SetActive(true);

        playerTurnText.text = playerName + "'s Turn!";
    }

    public void OnStartTurnClicked()
    {
        Debug.Log("Start Turn Clicked!");
        passDevicePanel.SetActive(false);
        mainHudPanel.SetActive(true);

        GameController gc = FindFirstObjectByType<GameController>();

        if (gc != null && gc.CurrentPlayer != null)
        {
            UpdateHand(gc.CurrentPlayer.Hand);
        }
        else
        {
            Debug.LogError("UIManager: Could not find GameController or CurrentPlayer to update hand!");
        }

        rollDiceButton.gameObject.SetActive(true);
        endTurnButton.gameObject.SetActive(false);
    }

    public void OnDiceRolled()
    {
        rollDiceButton.gameObject.SetActive(false);
        endTurnButton.gameObject.SetActive(true);
    }

    // --- 2. DOORS & PROMPTS ---
    public void ShowDoorPrompt(string roomName)
    {
        promptPanel.SetActive(true);
        promptText.text = $"Do you want to enter the {roomName}?";

        rollDiceButton.gameObject.SetActive(false);
    }

    public void OnAcceptDoorPrompt()
    {
        ClosePrompt();
        GameController gc = FindFirstObjectByType<GameController>();
        Player currentPlayer = gc.CurrentPlayer;

        RoomTile[] allRooms = FindObjectsByType<RoomTile>(FindObjectsSortMode.None);
        RoomTile closestRoom = null;
        float minDistance = Mathf.Infinity;

        foreach (RoomTile room in allRooms)
        {
            float dist = Vector3.Distance(currentPlayer.transform.position, room.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestRoom = room;
            }
        }

        if (closestRoom != null)
        {
            currentPlayer.GetComponent<Rigidbody>().isKinematic = true;
            currentPlayer.transform.position = closestRoom.transform.position + new Vector3(0, 1, 0);
            currentPlayer.GetComponent<Rigidbody>().isKinematic = false;

            gc.Board.MovePlayerToRoom(currentPlayer, closestRoom);
        }

        ToggleActionMenu(false);
    }

    public void OnDeclineDoorPrompt()
    {
        ClosePrompt();

        rollDiceButton.gameObject.SetActive(true);
        endTurnButton.gameObject.SetActive(true);
    }

    public void ClosePrompt()
    {
        promptPanel.SetActive(false);
    }

    public void ToggleGameLog()
    {
        gameLogPanel.SetActive(!gameLogPanel.activeSelf);
    }

    public void AddLogMessage(string message)
    {
        gameLogText.text += "- " + message + "\n";
        Debug.Log("GAME LOG: " + message);
    }

    // --- 3. SUGGESTIONS & ACCUSATIONS ---
    public void ToggleActionMenu(bool isAccusation)
    {
        if (actionMenuPanel.activeSelf)
        {
            actionMenuPanel.SetActive(false);
            return;
        }

        isCurrentActionAccusation = isAccusation;
        actionMenuPanel.SetActive(true);
        actionMenuTitle.text = isAccusation ? "Make an Accusation!" : "Make a Suggestion!";
    }

    public void OnConfirmActionClicked()
    {
        string chosenSuspect = suspectDropdown.options[suspectDropdown.value].text;
        string chosenWeapon = weaponDropdown.options[weaponDropdown.value].text;
        string chosenRoom = roomDropdown.options[roomDropdown.value].text;

        actionMenuPanel.SetActive(false);

        if (isCurrentActionAccusation)
        {
            FindFirstObjectByType<GameController>().ProcessAccusation(chosenSuspect, chosenWeapon, chosenRoom);
        }
        else
        {
            FindFirstObjectByType<GameController>().ProcessSuggestion(chosenSuspect, chosenWeapon, chosenRoom);
        }
    }

    public void CloseActionMenu()
    {
        actionMenuPanel.SetActive(false);
    }

    // --- 4. CAMERA TOGGLE ---
    public void ToggleCamera()
    {
        isBirdseyeView = !isBirdseyeView;
        Debug.Log("Camera switched to: " + (isBirdseyeView ? "Birdseye" : "Normal"));

        if (mainCameraController != null)
        {
            mainCameraController.ToggleBirdseyeView(isBirdseyeView);
        }
    }

    public void ToggleNotepad()
    {
        notepadPanel.SetActive(!notepadPanel.activeSelf);
    }

    public void UpdateHand(Hand hand)
    {
        Debug.Log($"UpdateHand called! Hand has {hand.Cards.Count} cards.");
        foreach (Transform child in handContainer)
            Destroy(child.gameObject);

        foreach (Card card in hand.Cards)
        {
            GameObject chip = Instantiate(cardChipPrefab, handContainer);
            UnityEngine.UI.Image img = chip.GetComponent<UnityEngine.UI.Image>();
            TextMeshProUGUI textComp = chip.GetComponentInChildren<TextMeshProUGUI>();

            Sprite cardSprite = Resources.Load<Sprite>("Cards/" + card.Name.Trim());


            if (cardSprite != null)
            {
                img.sprite = cardSprite;
                img.color = Color.white;
                if (textComp != null) textComp.text = "";
            }
            else
            {
                Debug.LogError($"FAILED TO LOAD: 'Assets/Resources/Cards/{card.Name.Trim()}'. Check spelling/extension!");
                if (textComp != null) textComp.text = card.Name;
                img.color = new Color(0.10f, 0.13f, 0.35f);
            }
        }
    }

    public void ShowClueCard(string name, string prompt)
    {
        clueCardNameText.text = name;
        clueCardPromptText.text = prompt;
        clueCardPanel.SetActive(true);
    }

    public void CloseClueCard()
    {
        clueCardPanel.SetActive(false);
        rollDiceButton.gameObject.SetActive(true);
        endTurnButton.gameObject.SetActive(true);
    }
}