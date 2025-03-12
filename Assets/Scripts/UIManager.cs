using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject actionPanel; // Assign in the Unity Editor

    // Toggles the visibility of the Main Panel


    // Toggles the visibility of the Action Panel
    public void setActionPanelActive(bool isActive)
    {
        if (actionPanel != null)
        {
            actionPanel.SetActive(isActive);
        }
    }


}
