using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUIManager : MonoBehaviour
{
    public GameObject highlight;
    public GameObject selectionIndicator;
    public GameObject selectionArrow;

    // Show or hide highlight
    public void SetHighlightVisibility(bool isVisible)
    {
        if (highlight != null)
            highlight.SetActive(isVisible);
    }

    // Show or hide selection indicator
    public void SetSelectionIndicatorVisibility(bool isVisible)
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(isVisible);
    }

    // Show or hide selection arrow
    public void SetSelectionArrowVisibility(bool isVisible)
    {
        if (selectionArrow != null)
            selectionArrow.SetActive(isVisible);
    }

    // Hide all UI elements
    public void HideAll()
    {
        SetHighlightVisibility(false);
        SetSelectionIndicatorVisibility(false);
        SetSelectionArrowVisibility(false);
    }

    // Show all UI elements
    public void ShowAll()
    {
        SetHighlightVisibility(true);
        SetSelectionIndicatorVisibility(true);
        SetSelectionArrowVisibility(true);
    }
}
