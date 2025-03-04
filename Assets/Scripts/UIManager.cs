using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainPanel;   // Assign in the Unity Editor
    public GameObject actionPanel; // Assign in the Unity Editor

    // Toggles the visibility of the Main Panel
    public void setMainPanelActive(bool isActive)
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(isActive);
        }
    }

    // Toggles the visibility of the Action Panel
    public void setActionPanelActive(bool isActive)
    {
        if (actionPanel != null)
        {
            actionPanel.SetActive(isActive);
        }
    }


}
