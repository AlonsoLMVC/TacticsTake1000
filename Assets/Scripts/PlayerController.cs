using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private List<Node> path;
    private int pathIndex;
    private bool isMoving = false;

    public bool IsMoving => isMoving;
 //  Stores the node the character is currently standing on

    public float placementOffset;

    private Vector3 moveDirection;

    private Compass compass;

    private GameManager gameManager;

    public UIManager uiManager;

    public Unit currentUnit;

    public Node currentHoveredNode;

    public bool nextAttackIsMagic;


    private void Start()
    {
        
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
        compass = GameObject.FindAnyObjectByType<Compass>();



    }





    private void Update()
    {
        if (isMoving && path != null && pathIndex < path.Count)
        {
            MoveAlongPath();
        }
        
    }


    public void setDirectionFacing(Vector2 newDirection)
    {
        currentUnit.directionFacing = newDirection;
        currentUnit.updateSpriteRotation();
    }

 public void setIndicatorDirectionFacing(Vector2 newDirection)
    {
        currentUnit.directionIndicator.SetEnlargedSphere(newDirection);
    }   



    public void SetPath(List<Node> newPath)
    {
        
        if (newPath == null || newPath.Count == 0) return;
        
        path = newPath;
        pathIndex = 0;

        if (!isMoving) //  Ensure it only starts movement if not already moving
        {
            MoveAlongPath();
        }
    }


    public void SetDirectionIndicatorActive(bool isActive)
    {
        //Debug.Log("setting direction indicator to " + isActive);
        if (currentUnit.directionIndicator != null)
        {
            currentUnit.directionIndicator.gameObject.SetActive(isActive);
        }
    }

    




    private void MoveAlongPath()
    {
        if (path == null || path.Count == 0)
        {
            isMoving = false;
            return;
        }


        if (!isMoving && gameManager.currentState == GameManager.GameState.DestinationSelect) // Only start if not already moving
        {
            
            gameManager.ChangeState(GameManager.GameState.InAction);
            isMoving = true;

            //currentUnit.currentNode.isWalkable = true;

            Vector3 currentPosition = currentUnit.gameObject.transform.position; 
            Node targetNode = path[0];

            float currentAltitude = currentUnit.currentNode.altitude;
            float targetAltitude = targetNode.altitude;

            //we reset the details of the node the unit is on before it even moves
            currentUnit.currentNode.isWalkable = true;
            currentUnit.currentNode.hasUnitOnTile = false;


            //Debug.Log($"Altitude Difference: " + (currentAltitude - targetAltitude));

            if (Mathf.Abs(targetAltitude - currentAltitude) >= currentUnit.currentNode.tileObject.transform.localScale.y) // Detect height differences
            {
                //Debug.Log("Height difference detected!");
                StartCoroutine(PerformJump(targetNode)); // Trigger jump
            }
            else
            {
                StartCoroutine(MoveToTile(targetNode)); // Normal movement
            }
        }
    }





    private IEnumerator PerformJump(Node targetNode)
    {
        isMoving = true;

        Vector3 startPos = currentUnit.gameObject.transform.position;
        Vector3 targetPos = new Vector3(targetNode.tileObject.transform.position.x, targetNode.tileObject.transform.position.y + placementOffset, targetNode.tileObject.transform.position.z);

        // Update move direction
        moveDirection = (targetPos - startPos).normalized;

        // Set animation parameters and take into account any kind of camera and compass rotation
        Vector2 xzDir = new Vector2((float)Math.Round(moveDirection.x), (float)Math.Round(moveDirection.z));
        Vector2 blendTreeValues = compass.convertDirectionToBlendTreeDirection(xzDir);
        Debug.Log(xzDir.ToString());

        currentUnit.directionFacing = xzDir;


        currentUnit.mainAnimator.SetFloat("directionX", blendTreeValues.x);
        currentUnit.mainAnimator.SetFloat("directionZ", blendTreeValues.y);


        float jumpDuration = 0.4f;
        float elapsedTime = 0f;

        float jumpHeight = 0.7f; // Maximum height of the jump

        //agarrando impulso
        yield return new WaitForSeconds(jumpDuration/3);



        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration; // Normalize time (0 to 1)

            // **Horizontal movement (Linear Interpolation)**
            Vector3 horizontalPos = Vector3.Lerp(startPos, targetPos, t);

            // **Vertical movement (Parabolic Arc)**
            float heightOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight; // Sinusoidal arc

            // **Combine horizontal and vertical movement**
            currentUnit.gameObject.transform.position = new Vector3(horizontalPos.x, horizontalPos.y + heightOffset, horizontalPos.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure correct landing position
        currentUnit.gameObject.transform.position = targetPos;

        pathIndex++;
        currentUnit.currentNode = targetNode;


        if (pathIndex < path.Count)
        {
            currentUnit.currentNode = path[pathIndex];
            //StartCoroutine(MoveToTile(path[pathIndex])); //  Start the next step directly inside this coroutine
            float currentAltitude = currentUnit.currentNode.altitude;
            float targetAltitude = targetNode.altitude;

            //Debug.Log($"Altitude Difference: " + (currentAltitude - targetAltitude));

            if (Mathf.Abs(targetAltitude - currentAltitude) >= currentUnit.currentNode.tileObject.transform.localScale.y) // Detect height differences
            {
                //Debug.Log("Height difference detected!");
                StartCoroutine(PerformJump(path[pathIndex])); // Trigger jump
            }
            else
            {
                StartCoroutine(MoveToTile(path[pathIndex])); // Normal movement
            }
        }
        else
        {
            isMoving = false; //  Only reset when fully finished

            gameManager.ChangeState(GameManager.GameState.CommandSelect);


            targetNode.isWalkable = false;            
            currentUnit.currentNode = targetNode;
            targetNode.hasUnitOnTile = true;

            currentUnit.hasMoved = true;

            gameManager.clearHighlightedTiles();
        }
    }







    private IEnumerator MoveToTile(Node targetNode)
    {
        float tileHeight = targetNode.tileObject.transform.localScale.y;
        float targetY = targetNode.tileObject.transform.position.y + placementOffset;
        Vector3 targetPosition = new Vector3(targetNode.x, targetY, targetNode.y);

        moveDirection = (targetPosition - currentUnit.gameObject.transform.position).normalized;

        // Set animation parameters and take into account any kind of camera and compass rotation
        Vector2 xzDir = new Vector2((float)Math.Round(moveDirection.x), (float)Math.Round(moveDirection.z));
        Vector2 blendTreeValues = compass.convertDirectionToBlendTreeDirection(xzDir);
        //Debug.Log(xzDir.ToString());

        currentUnit.directionFacing = xzDir;

        currentUnit.mainAnimator.SetFloat("directionX", blendTreeValues.x);
        currentUnit.mainAnimator.SetFloat("directionZ", blendTreeValues.y);



        float moveDuration = 0.2f;
        float elapsedTime = 0f;
        Vector3 startPosition = currentUnit.gameObject.transform.position;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            currentUnit.gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            yield return null;
        }

        currentUnit.gameObject.transform.position = targetPosition;
        pathIndex++;

        if (pathIndex < path.Count)
        {
            currentUnit.currentNode = path[pathIndex];

            float currentAltitude = currentUnit.currentNode.altitude;
            float targetAltitude = targetNode.altitude;

            //Debug.Log($"Altitude Difference: " + (currentAltitude - targetAltitude));

            if (Mathf.Abs(targetAltitude - currentAltitude) >= currentUnit.currentNode.tileObject.transform.localScale.y) // Detect height differences
            {
                //Debug.Log("Height difference detected!");
                StartCoroutine(PerformJump(path[pathIndex])); // Trigger jump
            }
            else
            {
                StartCoroutine(MoveToTile(path[pathIndex])); // Normal movement
            }
        }
        else
        {
            isMoving = false; //  Only reset when fully finished

            gameManager.ChangeState(GameManager.GameState.CommandSelect);

            //update node walkable information
            targetNode.isWalkable = false;
            currentUnit.currentNode = targetNode;
            targetNode.hasUnitOnTile = true;

            currentUnit.hasMoved = true;


            gameManager.clearHighlightedTiles();

        }
    }

    private IEnumerator ExecuteAttack()
    {
        gameManager.clearHighlightedTiles();
        //      yield return new WaitForSeconds(1f);

        Vector2 blendTreeValues = compass.convertDirectionToBlendTreeDirection(currentUnit.directionFacing);

        currentUnit.weaponAnimator.SetFloat("directionX", blendTreeValues.x);
        currentUnit.weaponAnimator.SetFloat("directionZ", blendTreeValues.y);


        if (nextAttackIsMagic == false)
        {
            currentUnit.mainAnimator.SetTrigger("attack");
            currentUnit.weaponAnimator.SetTrigger("attack");
        }
        else
        {
            currentUnit.mainAnimator.SetTrigger("channel");
        }

        currentUnit.hasActed = true;

        //yield return new WaitForSeconds(2.5f);
        

        yield return null;
    }




    public void attack()
    {
        gameManager.ChangeState(GameManager.GameState.InAction);
        StartCoroutine(ExecuteAttack()); // Trigger jump

    }










}
