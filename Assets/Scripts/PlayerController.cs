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

    public Node currentNode; //  Stores the node the character is currently standing on

    public int move = 4;

    public float placementOffset;
    public DirectionIndicator directionIndicator;

    private Vector3 moveDirection;

    public Animator mainAnimator;
    public Animator weaponAnimator;

    private Vector2 directionFacing;

    private Compass compass;

    private GameManager gameManager;



    private void Start()
    {
        mainAnimator = transform.Find("Sprite").GetComponent<Animator>(); // Ensure the Animator is on a child GameObject
        weaponAnimator = transform.Find("Weapon Sprite").GetComponent<Animator>();
        compass = GameObject.FindAnyObjectByType<Compass>();
        gameManager = GameObject.FindAnyObjectByType<GameManager>();

        // Set default animation parameters
        mainAnimator.SetFloat("directionX", 0);
        mainAnimator.SetFloat("directionZ", 1);

        directionFacing = new Vector2(0, 1);
    }


    private void Update()
    {
        if (isMoving && path != null && pathIndex < path.Count)
        {
            MoveAlongPath();
        }
        // Print enlarged sphere position when Spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            updateSpriteWithBlendTreeVector();
        }
    }


    public void setDirectionFacing(Vector2 newDirection)
    {
        directionFacing = newDirection;
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
        if (directionIndicator != null)
        {
            directionIndicator.gameObject.SetActive(isActive);
        }
    }

    




    private void MoveAlongPath()
    {
        if (path == null || path.Count == 0)
        {
            isMoving = false;
            return;
        }


        if (!isMoving && gameManager.currentState == GameManager.GameState.MoveSelect) // Only start if not already moving
        {
            
            gameManager.ChangeState(GameManager.GameState.InAction);
            isMoving = true;

            

            Vector3 currentPosition = transform.position; 
            Node targetNode = path[0];

            float currentAltitude = currentNode.altitude;
            float targetAltitude = targetNode.altitude;

            //Debug.Log($"Altitude Difference: " + (currentAltitude - targetAltitude));

            if (Mathf.Abs(targetAltitude - currentAltitude) >= currentNode.tileObject.transform.localScale.y) // Detect height differences
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


    public void updateSpriteRotation()
    {
        Debug.Log("Trying to update sprite rotation");
        Vector2 blendTreeValues = compass.convertDirectionToBlendTreeDirection(directionFacing);

        mainAnimator.SetFloat("directionX", blendTreeValues.x);
        mainAnimator.SetFloat("directionZ", blendTreeValues.y);
    }

    public void updateSpriteWithBlendTreeVector()
    {
        Vector2 rawValues = GetComponentInChildren<DirectionIndicator>().getEnlargedSpherePosition();

        Vector2 blendTreeValues = compass.convertDirectionToBlendTreeDirection(rawValues);

        
        mainAnimator.SetFloat("directionX", blendTreeValues.x);
        mainAnimator.SetFloat("directionZ", blendTreeValues.y);

        directionFacing = rawValues;

    }


    private IEnumerator PerformJump(Node targetNode)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(targetNode.tileObject.transform.position.x, targetNode.tileObject.transform.position.y + placementOffset, targetNode.tileObject.transform.position.z);

        // Update move direction
        moveDirection = (targetPos - startPos).normalized;

        // Set animation parameters and take into account any kind of camera and compass rotation
        Vector2 xzDir = new Vector2((float)Math.Round(moveDirection.x), (float)Math.Round(moveDirection.z));
        Vector2 blendTreeValues = compass.convertDirectionToBlendTreeDirection(xzDir);
        Debug.Log(xzDir.ToString());

        directionFacing = xzDir;

        mainAnimator.SetFloat("directionX", blendTreeValues.x);
        mainAnimator.SetFloat("directionZ", blendTreeValues.y);


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
            transform.position = new Vector3(horizontalPos.x, horizontalPos.y + heightOffset, horizontalPos.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure correct landing position
        transform.position = targetPos;

        pathIndex++;
        currentNode = targetNode;

        if (pathIndex < path.Count)
        {
            currentNode = path[pathIndex];
            //StartCoroutine(MoveToTile(path[pathIndex])); //  Start the next step directly inside this coroutine
            float currentAltitude = currentNode.altitude;
            float targetAltitude = targetNode.altitude;

            //Debug.Log($"Altitude Difference: " + (currentAltitude - targetAltitude));

            if (Mathf.Abs(targetAltitude - currentAltitude) >= currentNode.tileObject.transform.localScale.y) // Detect height differences
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

            gameManager.ChangeState(GameManager.GameState.DirectionSelect);

            currentNode = targetNode;
            gameManager.clearHighlightedTiles();
        }
    }







    private IEnumerator MoveToTile(Node targetNode)
    {
        float tileHeight = targetNode.tileObject.transform.localScale.y;
        float targetY = targetNode.tileObject.transform.position.y + placementOffset;
        Vector3 targetPosition = new Vector3(targetNode.x, targetY, targetNode.y);

        moveDirection = (targetPosition - transform.position).normalized;

        // Set animation parameters and take into account any kind of camera and compass rotation
        Vector2 xzDir = new Vector2((float)Math.Round(moveDirection.x), (float)Math.Round(moveDirection.z));
        Vector2 blendTreeValues = compass.convertDirectionToBlendTreeDirection(xzDir);
        //Debug.Log(xzDir.ToString());

        directionFacing = xzDir;

        mainAnimator.SetFloat("directionX", blendTreeValues.x);
        mainAnimator.SetFloat("directionZ", blendTreeValues.y);



        float moveDuration = 0.2f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            yield return null;
        }

        transform.position = targetPosition;
        pathIndex++;

        if (pathIndex < path.Count)
        {
            currentNode = path[pathIndex];

            float currentAltitude = currentNode.altitude;
            float targetAltitude = targetNode.altitude;

            //Debug.Log($"Altitude Difference: " + (currentAltitude - targetAltitude));

            if (Mathf.Abs(targetAltitude - currentAltitude) >= currentNode.tileObject.transform.localScale.y) // Detect height differences
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
            currentNode = targetNode;

            gameManager.ChangeState(GameManager.GameState.DirectionSelect);

            GameObject.FindAnyObjectByType<GameManager>().clearHighlightedTiles();

        }
    }

    private IEnumerator ExecuteAttack()
    {
        gameManager.clearHighlightedTiles();
        //      yield return new WaitForSeconds(1f);

        Vector2 blendTreeValues = compass.convertDirectionToBlendTreeDirection(directionFacing);

        weaponAnimator.SetFloat("directionX", blendTreeValues.x);
        weaponAnimator.SetFloat("directionZ", blendTreeValues.y);

        mainAnimator.SetTrigger("attack");
        weaponAnimator.SetTrigger("attack");


        //yield return new WaitForSeconds(2.5f);
        

        return null;
    }



    public void attack()
    {
        gameManager.ChangeState(GameManager.GameState.InAction);
        StartCoroutine(ExecuteAttack()); // Trigger jump

    }

    public void endAttack()
    {
        StartCoroutine(ExecuteAttack()); // Trigger jump

        //gameManager.ChangeState(GameManager.GameState.DirectionSelect);
    }







}
