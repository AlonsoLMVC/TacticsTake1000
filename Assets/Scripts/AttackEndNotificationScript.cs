using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEndNotificationScript : MonoBehaviour
{
    public GameObject playerObject;
    public void notifyEndOfAttack()
    {
        playerObject.GetComponent<PlayerController>().endAttack();

    }
}
