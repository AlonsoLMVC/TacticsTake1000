using UnityEngine;


//if I make it a monobehaviour things break at the moment.
public class Node
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

    public Node(int x, int y, bool isWalkable, GameObject tileObject, int altitude)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        this.tileObject = tileObject;
        this.altitude = altitude;
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
}
