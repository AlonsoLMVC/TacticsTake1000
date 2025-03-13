using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCalculator
{

    public List<Unit> turnOrder = new List<Unit>();


    public List<Unit> generateTurnOrder(List<Unit> units)
    {

        while (turnOrder.Count <= 30)
        {
            string CTs = "";
            foreach (Unit u in units)
            {
                u.CT += u.Speed;

                if (u.CT >= 100)
                {
                    turnOrder.Add(u);
                    u.CT -= 100;
                }

                CTs += u.Job + " " + u.CT + "////";
            }

            //Debug.Log(CTs);
        }

        return turnOrder;
    }

    public Unit getNextUnit(List<Unit> units)
    {
        bool nextUnitFound = false;

        while (nextUnitFound == false)
        {
            foreach (Unit u in units)
            {
                u.CT += u.Speed;

                if (u.CT >= 100)
                {
                    turnOrder.Add(u);
                    u.CT -= 100;
                    return u;
                }

            }
        }

        return null;
    }






}
