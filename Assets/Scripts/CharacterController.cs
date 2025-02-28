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
        if (newPath == null || newPath.Count == 0 || isMoving) return;

        path = newPath;
        pathIndex = 0;
        isMoving = true;
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
        if (path == null || pathIndex >= path.Count)
        {
            isMoving = false;
            return;
        }

        Node targetNode = path[pathIndex];

        //  Adjust the target Y position to be on top of the tile
        float tileHeight = targetNode.tileObject.transform.localScale.y;
        float targetY = targetNode.tileObject.transform.position.y + (tileHeight / 2f) + 0.5f;

        Vector3 targetPosition = new Vector3(targetNode.x, targetY, targetNode.y); // Ensure correct height

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        //  Rotate character to face movement direction
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            pathIndex++;
            if (pathIndex >= path.Count)
            {
                isMoving = false; //  Stop movement when reaching destination
            }
        }
    }

}
