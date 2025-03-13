using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BottomPanelUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructions;

    /// <summary>
    /// Updates the instruction text displayed on the bottom panel.
    /// </summary>
    public void UpdateInstructions(string text)
    {
        if (instructions != null)
        {
            instructions.text = text;
        }
    }

    /// <summary>
    /// Clears the instruction text.
    /// </summary>
    public void ClearInstructions()
    {
        if (instructions != null)
        {
            instructions.text = "";
        }
    }
}
