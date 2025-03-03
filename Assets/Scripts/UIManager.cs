using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainPanel;   // Assign in the Unity Editor
    public GameObject actionPanel; // Assign in the Unity Editor

    // Toggles the visibility of the Main Panel
    public void ToggleMainPanel(bool isActive)
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(isActive);
        }
    }

    // Toggles the visibility of the Action Panel
    public void ToggleActionPanel(bool isActive)
    {
        if (actionPanel != null)
        {
            actionPanel.SetActive(isActive);
        }
    }
}
