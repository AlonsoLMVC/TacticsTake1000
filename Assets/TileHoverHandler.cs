using UnityEngine;

public class TileHoverHandler : MonoBehaviour
{
    private TileUIManager tileUIManager;
    private Node nodeReference;

    void Start()
    {
        tileUIManager = GetComponent<TileUIManager>(); // Get Tile UI Manager attached to the tile
        nodeReference = GetComponent<Node>(); // Assuming the Node script is on the same object
    }

    void OnMouseEnter()
    {
        if (tileUIManager != null)
        {
            tileUIManager.SetSelectionIndicatorVisibility(true);
            tileUIManager.SetSelectionArrowVisibility(true);
        }


    }

    void OnMouseExit()
    {
        if (tileUIManager != null)
        {
            tileUIManager.SetSelectionIndicatorVisibility(false);
            tileUIManager.SetSelectionArrowVisibility(false);
        }


    }
}
