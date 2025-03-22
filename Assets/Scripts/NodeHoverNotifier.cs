using UnityEngine;

public class NodeHoverNotifier : MonoBehaviour
{

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnMouseEnter()
    {
        if (gameManager != null)
        {
            gameManager.HandleNodeMouseEnter(GetComponent<Node>());
           
        }


    }

    void OnMouseExit()
    {
        if (gameManager != null)
        {
            gameManager.HandleNodeMouseExit(GetComponent<Node>());
            
        }


    }
}
