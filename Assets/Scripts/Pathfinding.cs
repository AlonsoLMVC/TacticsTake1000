using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public GameManager gridManager;
    private PriorityQueue<Node> openList;
    private HashSet<Node> closedList;

    public List<Node> FindPath(Vector2Int start, Vector2Int goal)
    {
        Node startNode = gridManager.GetNode(start.x, start.y);
        Node goalNode = gridManager.GetNode(goal.x, goal.y);
        /*
        if (!startNode.isWalkable || !goalNode.isWalkable)
            return null; // No path if start or goal is blocked
        */
        if (!goalNode.isWalkable)
            return null; // No path if goal is blocked

        openList = new PriorityQueue<Node>((a, b) => a.fCost.CompareTo(b.fCost));
        closedList = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, goalNode);
        startNode.altitudeCost = 0;
        openList.Enqueue(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.Dequeue();

            if (currentNode == goalNode)
                return RetracePath(startNode, goalNode);

            closedList.Add(currentNode);

            foreach (Node neighbor in gridManager.GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedList.Contains(neighbor))
                    continue;

                int altitudeDifference = Mathf.Abs(currentNode.altitude - neighbor.altitude);

                int altitudePenalty = altitudeDifference * 10; // Increase weight

                int newCost = currentNode.gCost + GetDistance(currentNode, neighbor) + altitudePenalty;

                if (newCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetDistance(neighbor, goalNode);
                    neighbor.altitudeCost = altitudePenalty; // NEW: Assign altitude penalty
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Enqueue(neighbor);
                    else
                        openList.UpdateItem(neighbor);
                }
            }
        }

        return null; // No path found
    }


    private List<Node> RetracePath(Node startNode, Node goalNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = goalNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.x - b.x);
        int distZ = Mathf.Abs(a.y - b.y); //  Z instead of Y

        return 10 * (distX + distZ);
    }

}
