using System.Collections.Generic;
using UnityEngine;

public static class UnitFactory
{
    public static Dictionary<string, int[]> JobBaseStats = new Dictionary<string, int[]>()
    {
        // Format: { HP, MP, WAtk, WDef, MPow, MRes, Speed, Evade, SRes }
        { "Archer",       new int[] {120, 40, 12, 8, 8, 8, 7, 10, 5} },
        { "Black Mage",   new int[] {100, 60, 8, 6, 14, 10, 6, 5, 10} },
        { "Blue Mage",    new int[] {115, 50, 10, 8, 12, 9, 7, 8, 8} },
        { "Fighter",      new int[] {140, 30, 15, 12, 6, 7, 8, 5, 5} },
        { "Hunter",       new int[] {125, 35, 14, 9, 7, 8, 9, 12, 6} },
        { "Illusionist",  new int[] {110, 70, 7, 6, 16, 12, 5, 5, 12} },
        { "Ninja",        new int[] {110, 30, 14, 7, 9, 8, 10, 15, 7} },
        { "Paladin",      new int[] {150, 30, 13, 14, 7, 10, 6, 5, 10} },
        { "Soldier",      new int[] {135, 25, 12, 11, 6, 7, 7, 6, 5} },
        { "Thief",        new int[] {115, 30, 10, 7, 6, 7, 10, 18, 8} },
        { "White Mage",   new int[] {105, 65, 7, 6, 13, 14, 5, 5, 12} }
    };


}
