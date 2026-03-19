//this one is easy lol
using UnityEngine;
using System
using System.Collections.Generic;

public class DiceRoller : MonoBehaviour {
  public Tuple<int, int> RollTwoDice(int dSides) { 
    int roll1 = Random.Range(1, dSides + 1);
    int roll2 = Random.Range(1, dSides + 1);

    return Tuple.Create(roll1, roll2);
  }
}

//Dice roll saved ro a tuple
