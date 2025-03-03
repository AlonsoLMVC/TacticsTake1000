using UnityEngine;
using UnityEngine.EventSystems;

public class UIDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Click Detected");

            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Mouse is over a UI element.");
            }
            else
            {
                Debug.Log("Mouse is NOT over a UI element.");
            }
        }
    }
}
