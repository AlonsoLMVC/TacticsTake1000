using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public GameObject tilePrefab;

    private Node[,] grid;

    public GameObject characterPrefab; // Assign in the Inspector
    private GameObject characterInstance;
    private CharacterController characterScript;

    private void Start()
    {
        GenerateGrid();
        SpawnCharacter();

        // Pass grid size to CameraController
        CameraController cameraController = FindObjectOfType<CameraController>();
        if (cameraController != null)
        {
            cameraController.gridWidth = width;
            cameraController.gridHeight = height;
        }

    }

    void SpawnCharacter()
    {
        List<Node> walkableTiles = new List<Node>();
        foreach (Node node in grid)
        {
            if (node.isWalkable)
                walkableTiles.Add(node);
        }

        if (walkableTiles.Count == 0) return;

        // Pick a random walkable tile
        Node spawnTile = walkableTiles[Random.Range(0, walkableTiles.Count)];

        float tileHeight = spawnTile.tileObject.transform.localScale.y; // Get the cube's height
        float characterHeight = 1f; // Change this based on your character's height
        float spawnY = spawnTile.tileObject.transform.position.y + (tileHeight / 2f) + (characterHeight / 2f); //  Adjust for tile & character height

        Vector3 spawnPosition = new Vector3(spawnTile.x, spawnY, spawnTile.y); // XZ plane with correct Y height

        // Instantiate the character
        characterInstance = Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
        characterScript = characterInstance.GetComponent<CharacterController>();
    }







    void GenerateGrid()
    {
        grid = new Node[width, height];

        float offsetX = Random.Range(0f, 1000f);
        float offsetY = Random.Range(0f, 1000f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseValue = Mathf.PerlinNoise((x + offsetX) * 0.1f, (y + offsetY) * 0.1f);
                int altitude = Mathf.RoundToInt(noiseValue * 10); // 0 to 10 altitude

                float tileHeight = altitude * 0.5f + 0.1f; // Scale based on altitude
                float tilePositionY = tileHeight / 2f; // Raise the cube so the base is at y=0

                Vector3 position = new Vector3(x, tilePositionY, y); // XZ plane, Y based on height
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.transform.position = position;
                tile.transform.localScale = new Vector3(1, tileHeight, 1); // Proper height scaling

                tile.name = $"Tile {x},{y}";

                grid[x, y] = new Node(x, y, true, tile, altitude);
            }
        }
    }







    void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            // Check if the clicked object is a tile
            foreach (Node node in grid)
            {
                if (node.tileObject == clickedObject)
                {
                    node.ToggleWalkability();
                    break;
                }
            }
        }
    }


    public Node GetNode(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return grid[x, y];
        return null;
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        int[,] directions = new int[,]
        {
        { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } // Up, Right, Down, Left
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int checkX = node.x + directions[i, 0];
            int checkY = node.y + directions[i, 1];

            Node neighbor = GetNode(checkX, checkY);
            if (neighbor != null)
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    public Pathfinding pathfinding;
    private Node startNode, goalNode;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click for obstacles
        {
            HandleClick();
        }
        else if (Input.GetMouseButtonDown(1)) // Right Click to move
        {
            HandleCharacterMovement();
        }
        else if (Input.GetKeyDown(KeyCode.R)) // Reset scene
        {
            ReloadScene();
        }
    }

    void HandleCharacterMovement()
    {
        
        if (characterScript == null || characterScript.IsMoving) return; // Prevent movement mid-action

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Use Raycast to detect the clicked tile
        {
            GameObject clickedObject = hit.collider.gameObject;

            foreach (Node node in grid)
            {
                if (node.tileObject == clickedObject && node.isWalkable)
                {
                    Vector3 characterPos = characterInstance.transform.position;

                    // Ensure movement is on the correct Y level (top of the tile)
                    float tileHeight = node.tileObject.transform.localScale.y;
                    float targetY = node.tileObject.transform.position.y + (tileHeight / 2f) + 0.5f;

                    List<Node> path = pathfinding.FindPath(
                        new Vector2Int(Mathf.RoundToInt(characterPos.x), Mathf.RoundToInt(characterPos.z)),
                        new Vector2Int(node.x, node.y)
                    );

                    if (path != null)
                    {
                        characterScript.SetPath(path);
                    }
                    break;
                }
            }
        }
    }



    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



    void HandlePathfinding()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.y / cellSize);

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            Node clickedNode = grid[x, y];

            if (startNode == null)
            {
                startNode = clickedNode;
                clickedNode.tileObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else if (goalNode == null)
            {
                goalNode = clickedNode;
                clickedNode.tileObject.GetComponent<SpriteRenderer>().color = Color.red;

                List<Node> path = pathfinding.FindPath(new Vector2Int(startNode.x, startNode.y), new Vector2Int(goalNode.x, goalNode.y));

                if (path != null)
                {
                    foreach (Node node in path)
                    {
                        node.tileObject.GetComponent<SpriteRenderer>().color = Color.blue;
                    }
                }
            }
            else
            {
                ResetPath();
            }
        }
    }

    void ResetPath()
    {
        foreach (Node node in grid)
        {
            if (node.isWalkable)
                node.tileObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        startNode = null;
        goalNode = null;
    }


}
