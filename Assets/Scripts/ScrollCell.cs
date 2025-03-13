using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ScrollCell : MonoBehaviour
{
    // Start is called before the first frame update

    public Unit unit;
    public Image portraitSprite;
    public TextMeshProUGUI turnOrderText;

    public void SetUnitData(Unit newUnit, int turnOrder)
    {
        if (newUnit == null) return; // Prevent null reference errors

        unit = newUnit;

        // Set the unit's portrait if an image exists
        if (portraitSprite != null && unit.displaySprite != null)
        {
            portraitSprite.sprite = unit.displaySprite;
        }

        // Set the turn order text
        if (turnOrderText != null)
        {
            turnOrderText.text = turnOrder.ToString();
        }
    }

    public void setTurnOrder(int turnOrder)
    {
        turnOrderText.text = turnOrder.ToString();
    }
}
