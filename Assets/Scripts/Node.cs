using UnityEngine;



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

    public bool hasUnitOnTile;

    public GameManager gameManager;
    public PlayerController playerController;




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
        isHighlighted = isVisible;
        if (highlight != null)
            highlight.SetActive(isVisible);
    }
    
    public void SetHighlightDefault()
    {
        if (highlight != null)
        {
            SpriteRenderer renderer = highlight.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = Color.blue;
            }
        }
    }

    public void SetHighlightInRangeOfEnemyAttack()
    {
        if (highlight != null)
        {
            SpriteRenderer renderer = highlight.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = new Color(0.5f, 0f, 0.5f); // Purple (R:0.5, G:0, B:0.5)
            }
        }
    }

    public void SetHighlightInRangeOfPlayerAttack()
    {
        if (highlight != null)
        {
            SpriteRenderer renderer = highlight.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = Color.red;
            }
        }
    }

    

    public void SetSelectionIndicatorModeNormal()
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.transform.localScale = Vector3.one;
            selectionIndicator.transform.localPosition = Vector3.zero;

            // Change child sprite colors to white
            SetChildrenSpriteColor(selectionIndicator, Color.white);
        }
    }

    public void SetSelectionIndicatorModeAlly()
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.transform.localScale = new Vector3(1f, 2f, 1f);;
            selectionIndicator.transform.localPosition = new Vector3(0, -0.5f, 0);

            // Change child sprite colors to blue
            SetChildrenSpriteColor(selectionIndicator, Color.cyan);
            
        }
    }

    public void SetSelectionIndicatorModeEnemy()
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.transform.localScale = new Vector3(1f, 2f, 1f);;
            selectionIndicator.transform.localPosition = new Vector3(0, -0.5f, 0);

            // Change child sprite colors to red
            SetChildrenSpriteColor(selectionIndicator, Color.red);
        }
    }

    private void SetChildrenSpriteColor(GameObject parent, Color color)
    {
        foreach (Transform child in parent.transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }
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
