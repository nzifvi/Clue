using UnityEngine;

[System.Serializable]
public class ClueCard
{
    public string CardName;
    [TextArea(3, 5)]
    public string PromptText;
}