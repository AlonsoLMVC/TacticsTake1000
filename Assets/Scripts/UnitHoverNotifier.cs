using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitHoverNotifier : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        // Get the parent Node component (assumes the Node script is on the parent object)
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnMouseEnter()
    {
        if (gameManager != null)
        {
            gameManager.HandleUnitMouseEnter(this.GetComponentInParent<Unit>());
        }
    }

    void OnMouseExit()
    {
        if (gameManager != null)
        {
            gameManager.HandleUnitMouseExit(this.GetComponentInParent<Unit>());
        }
    }
}
