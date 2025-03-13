using UnityEngine;

public class UIManager : MonoBehaviour
{


    public GameObject actionPanel;
    public BottomPanelUIManager bottomPanel;
    public ProfilePictureUIManager profilePicturePanel;
    public DetailsPanelUIManager detailsPanel;
    public GameObject terrainPanel;
    public GameObject scrollPanel;
    public GameObject scrollBarPanel;
    public FloatingPanelUIManager floatingPanel;

    // Toggles the visibility of the Action Panel
    public void SetActionPanelActive(bool isActive) => SetPanelActive(actionPanel, isActive);

    // Toggles the visibility of the Bottom Panel
    public void SetBottomPanelActive(bool isActive) => SetPanelActive(bottomPanel.gameObject, isActive);

    // Toggles the visibility of the Profile Picture Panel
    public void SetProfilePicturePanelActive(bool isActive) => SetPanelActive(profilePicturePanel.gameObject, isActive);

    // Toggles the visibility of the Details Panel
    public void SetDetailsPanelActive(bool isActive) => SetPanelActive(detailsPanel.gameObject, isActive);

    // Toggles the visibility of the Terrain Panel
    public void SetTerrainPanelActive(bool isActive) => SetPanelActive(terrainPanel, isActive);

    // Toggles the visibility of the Scroll Panel
    public void SetScrollPanelActive(bool isActive) => SetPanelActive(scrollPanel, isActive);

    // Toggles the visibility of the Scroll Bar Panel
    public void SetScrollBarPanelActive(bool isActive) => SetPanelActive(scrollBarPanel, isActive);

    // Toggles the visibility of the Floating Panel
    public void SetFloatingPanelActive(bool isActive) => SetPanelActive(floatingPanel.gameObject, isActive);

    // Generic method to toggle visibility of any panel
    private void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }

    // Hides all panels at once
    public void HideAllPanels()
    {
        SetPanelActive(actionPanel, false);
        SetPanelActive(bottomPanel.gameObject, false);
        SetPanelActive(profilePicturePanel.gameObject, false);
        SetPanelActive(detailsPanel.gameObject, false);
        SetPanelActive(terrainPanel, false);
        SetPanelActive(scrollPanel, false);
        SetPanelActive(scrollBarPanel, false);
        SetPanelActive(floatingPanel.gameObject, false);
    }

    // Sets UI panel states based on the GameState
    public void SetUIState(GameManager.GameState state)
    {
        HideAllPanels(); // Reset all panels before applying state-specific settings

        switch (state)
        {
            case GameManager.GameState.DestinationSelect:
                SetPanelActive(actionPanel, false);
                SetPanelActive(bottomPanel.gameObject, true);
                SetPanelActive(profilePicturePanel.gameObject, true);
                SetPanelActive(detailsPanel.gameObject, true);
                SetPanelActive(terrainPanel, true);
                SetPanelActive(scrollPanel, true);
                SetPanelActive(scrollBarPanel, true);
                SetPanelActive(floatingPanel.gameObject, false);
                break;

            case GameManager.GameState.CommandSelect:
                SetPanelActive(actionPanel, true);
                SetPanelActive(bottomPanel.gameObject, true);
                SetPanelActive(profilePicturePanel.gameObject, true);
                SetPanelActive(detailsPanel.gameObject, true);
                SetPanelActive(terrainPanel, true);
                SetPanelActive(scrollPanel, true);
                SetPanelActive(scrollBarPanel, true);
                SetPanelActive(floatingPanel.gameObject, true);
                break;

            case GameManager.GameState.TargetSelect:
                SetPanelActive(actionPanel, false);
                SetPanelActive(bottomPanel.gameObject, true);
                SetPanelActive(profilePicturePanel.gameObject, true);
                SetPanelActive(detailsPanel.gameObject, true);
                SetPanelActive(terrainPanel, true);
                SetPanelActive(scrollPanel, true);
                SetPanelActive(scrollBarPanel, true);
                SetPanelActive(floatingPanel.gameObject, true);
                break;

            case GameManager.GameState.InAction:
                SetPanelActive(actionPanel, false);
                SetPanelActive(bottomPanel.gameObject, true);
                SetPanelActive(profilePicturePanel.gameObject, false);
                SetPanelActive(detailsPanel.gameObject, false);
                SetPanelActive(terrainPanel, false);
                SetPanelActive(scrollPanel, false);
                SetPanelActive(scrollBarPanel, false);
                SetPanelActive(floatingPanel.gameObject, false);
                break;

            case GameManager.GameState.StandbyDirectionSelect:
                SetPanelActive(actionPanel, false);
                SetPanelActive(bottomPanel.gameObject, true);
                SetPanelActive(profilePicturePanel.gameObject, true);
                SetPanelActive(detailsPanel.gameObject, true);
                SetPanelActive(terrainPanel, true);
                SetPanelActive(scrollPanel, true);
                SetPanelActive(scrollBarPanel, true);
                SetPanelActive(floatingPanel.gameObject, false);
                break;
        }
    }
}
