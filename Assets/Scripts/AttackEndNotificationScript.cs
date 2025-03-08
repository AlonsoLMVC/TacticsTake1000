using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEndNotificationScript : MonoBehaviour
{
    public GameManager gm;

    private void Start()        
    {
        gm = GameObject.FindAnyObjectByType<GameManager>();

        if(gm == null)
        {
            Debug.Log("Game Manager is null in attack notification script.");
        }
        else
        {
            Debug.Log("Game Manager is not null in attack notification script.");

        }
    }

    public void notifyEndOfAttack()
    {
        
        gm.endAttack();

    }
}

