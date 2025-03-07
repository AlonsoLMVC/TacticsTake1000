using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySet
{
    public string Name;
    public List<ActionAbility> Abilities;

    public AbilitySet(string name)
    {
        Name = name;
        Abilities = new List<ActionAbility>();
    }

    public void AddAbility(ActionAbility ability)
    {
        Abilities.Add(ability);
    }
}
