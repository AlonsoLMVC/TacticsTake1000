using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ActionAbility
{
    public string Name;
    public int Power;
    public int MinRange;
    public int MaxRange;
    public int MPCost;
    public bool IsMagic;
    public AbilitySet Category; // Link to its ability set

    public ActionAbility(string name, int power, int minRange, int maxRange, int mpCost, bool isMagic, AbilitySet category)
    {
        Name = name;
        Power = power;
        MinRange = minRange;
        MaxRange = maxRange;
        MPCost = mpCost;
        IsMagic = isMagic;
        Category = category;
    }
}
