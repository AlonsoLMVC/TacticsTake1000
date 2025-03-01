using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private List<Node> path;
    private int pathIndex;
    private bool isMoving = false;

    public bool IsMoving => isMoving;

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


    private void Update()
    {
        if (isMoving && path != null && pathIndex < path.Count)
        {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        if (path == null || path.Count < 2) // Ensure there are at least two nodes to compare
        {
            isMoving = false;
            return;
        }

        if (!isMoving) // Only start if not already moving
        {
            isMoving = true;

            Node currentNode = path[0]; // First node in path
            Node targetNode = path[1]; // Second node in path (the first movement step)

            float currentHeight = currentNode.tileObject.transform.position.y + (currentNode.tileObject.transform.localScale.y / 2f);
            float targetHeight = targetNode.tileObject.transform.position.y + (targetNode.tileObject.transform.localScale.y / 2f);

            Debug.Log($"Current Node ({currentNode.x}, {currentNode.y}) - Height: {currentHeight}");
            Debug.Log($"Target Node ({targetNode.x}, {targetNode.y}) - Height: {targetHeight}");
            Debug.Log($"Height Difference: {targetHeight - currentHeight}");

            if (Mathf.Abs(targetHeight - currentHeight) > 0.1f) // Detect height differences
            {
                Debug.Log("Height difference detected!");
                StartCoroutine(PerformJump(targetNode, currentHeight, targetHeight)); // Trigger jump
            }
            else
            {
                StartCoroutine(MoveToTile(targetNode)); // Normal movement
            }
        }
    }





    private IEnumerator PerformJump(Node targetNode, float startHeight, float targetHeight)
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        float tileHeight = targetNode.tileObject.transform.localScale.y;
        float targetY = targetNode.tileObject.transform.position.y + (tileHeight / 2f) + 0.5f;

        Vector3 targetPosition = new Vector3(targetNode.x, targetY, targetNode.y);

        float jumpPeakHeight = Mathf.Max(startHeight, targetHeight) + 1.5f;
        float jumpDuration = 0.6f;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            if (startHeight < targetHeight) // Jumping UP
            {
                float height = Mathf.Lerp(startHeight, jumpPeakHeight, t * 1.2f);
                Vector3 midPoint = new Vector3(startPosition.x, height, startPosition.z);
                transform.position = Vector3.Lerp(startPosition, midPoint, t * 0.6f);
            }
            else // Jumping DOWN
            {
                Vector3 arcPoint = new Vector3(targetPosition.x, jumpPeakHeight, targetPosition.z);
                transform.position = Vector3.Lerp(startPosition, arcPoint, t * 0.5f);
                transform.position = new Vector3(transform.position.x, Mathf.Lerp(jumpPeakHeight, targetY, t), transform.position.z);
            }

            yield return null;
        }

        transform.position = targetPosition; // Ensure correct landing position
        pathIndex++;

        if (pathIndex < path.Count)
        {
            yield return new WaitForSeconds(0.1f); // Prevent immediate re-triggering
            StartCoroutine(MoveToTile(path[pathIndex])); //  Chain the next movement step
        }
        else
        {
            isMoving = false;
        }
    }




    private IEnumerator MoveToTile(Node targetNode)
    {
        float tileHeight = targetNode.tileObject.transform.localScale.y;
        float targetY = targetNode.tileObject.transform.position.y + (tileHeight / 2f) + 0.5f;
        Vector3 targetPosition = new Vector3(targetNode.x, targetY, targetNode.y);

        float moveDuration = 0.3f;
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
            StartCoroutine(MoveToTile(path[pathIndex])); //  Start the next step directly inside this coroutine
        }
        else
        {
            isMoving = false; //  Only reset when fully finished
        }
    }













}
