using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardType
    {
        
    }

    protected CardType cardType;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public CardType getCardType()
    {
        return cardType;
    }

    public void setCardType(CardType cardType)
    {
        this.cardType = cardType;
    }
}
