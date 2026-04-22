using UnityEngine;
using System.Collections;
using System;

public class DiceManager : MonoBehaviour
{
    public DiceVisual dice1;
    public DiceVisual dice2;
    public DiceRoller diceRoller;

    public Vector3 spawnPoint1 = new Vector3(18, 1, 5);
    public Vector3 spawnPoint2 = new Vector3(19, 1, 5);

    private Action<int, int> onRollComplete;

    public void RollDice(Action<int, int> callback)
    {
        dice1.ResetDice(spawnPoint1);
        dice2.ResetDice(spawnPoint2);

        onRollComplete = callback;
        StartCoroutine(RollBothDice());
    }

    private IEnumerator RollBothDice()
    {
        var result = diceRoller.RollTwoDice(6);
        int die1 = result.Item1;
        int die2 = result.Item2;

        dice1.Roll(die1);
        dice2.Roll(die2);

        yield return new WaitUntil(() =>
            !dice1.IsRolling() && !dice2.IsRolling());

            onRollComplete?.Invoke(die1, die2);

    }
}
