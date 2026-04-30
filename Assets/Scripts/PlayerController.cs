using UnityEngine;

public class Player : MonoBehaviour
{
    public GameController.PlayerID ID;

    [SerializeField] private string playerName;
    public string PlayerName { get => playerName; set => playerName = value; }

    private int movementAmount = 0;

    private bool hasMovementPhaseFinished = false;
    private bool hasSuggestionPhaseFinished = false;
    private bool hasAccusationPhaseFinished = false;

    public Hand Hand { get; private set; } = new Hand();
    public RoomTile CurrentRoom { get; set; }

    
    private PlayerMovement playerMovementObj;
    void Awake()
    {
        playerMovementObj = GetComponent<PlayerMovement>();
    }
    
    void Update()
    {
        
    }

    public bool hasTurnFinished()
    {
        return hasMovementPhaseFinished && hasSuggestionPhaseFinished && hasAccusationPhaseFinished;
    }
    
    public void addMovementAmount(int newMovementAmount)
    {
        movementAmount = newMovementAmount;
        //playerMovementObj.SetMovementAmount(newMovementAmount);
        GetComponent<PlayerMovement>().SetMovementAmount(movementAmount);
    }

    public int MovesRemaining => movementAmount;
}
