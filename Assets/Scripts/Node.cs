using UnityEngine;


//if I make it a monobehaviour things break at the moment.
public class Node : MonoBehaviour
{
    public int x, y; // Grid position
    public bool isWalkable;
    public GameObject tileObject;

    public Node parent; // A* Parent

    public int gCost;
    public int hCost;
    public int altitudeCost;
    public int fCost => gCost + hCost + altitudeCost;

    public int altitude;

    public bool isHighlighted;

    Unit currentUnit;

    public Node(int x, int y, bool isWalkable, GameObject tileObject, int altitude)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        this.tileObject = tileObject;
        this.altitude = altitude;
        isHighlighted = false;
        UpdateColor();
    }

    public void setValues(int x, int y, bool isWalkable, GameObject tileObject, int altitude) {

        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        this.tileObject = tileObject;
        this.altitude = altitude;
        isHighlighted = false;
        UpdateColor();
    }

    public void ToggleWalkability()
    {
        isWalkable = !isWalkable;
        UpdateColor();
    }

    void UpdateColor()
    {
        if (tileObject != null)
        {
            Renderer renderer = tileObject.GetComponent<Renderer>();

            if (!isWalkable)
            {
                renderer.material.color = Color.black;
            }
            else
            {
                float colorValue = Mathf.Clamp01(altitude / 10f);
                renderer.material.color = new Color(colorValue, colorValue, colorValue);
            }
        }
    }

    


    public Vector2 getGridCoordinates()
    {
        return new Vector2(x, y);
    }





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




    private TileUIManager tileUIManager;
    private Node nodeReference;

    void OnMouseEnter()
    {
        SetSelectionIndicatorVisibility(true);
        SetSelectionArrowVisibility(true);
        


    }

    void OnMouseExit()
    {
        SetSelectionIndicatorVisibility(false);
        SetSelectionArrowVisibility(false);
        


    }



}
