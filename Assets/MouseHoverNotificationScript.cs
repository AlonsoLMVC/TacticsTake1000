using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseHoverNotificationScript : MonoBehaviour
{
    private Unit parentUnit;

    void Start()
    {
        // Get the parent Node component (assumes the Node script is on the parent object)
        parentUnit = GetComponentInParent<Unit>();
    }

    void OnMouseEnter()
    {
        if (parentUnit != null)
        {
            parentUnit.OnHoverEnter();
        }
    }

    void OnMouseExit()
    {
        if (parentUnit != null)
        {
            parentUnit.OnHoverExit();
        }
    }
}
