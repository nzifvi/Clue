using UnityEngine;

public class Player : MonoBehaviour
{
    private string playerName;
    private int movementAmount = 0;

    private bool hasMovementPhaseFinished = false;
    private bool hasSuggestionPhaseFinished = false;
    private bool hasAccusationPhaseFinished = false;
    
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
    }
}
