using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{



        public enum GameState
    {
        MenuNav,       // Navigating menus
        MoveSelect,    // Selecting where to move
        DirectionSelect, // Choosing facing direction
        ChooseTarget,  // Selecting an attack target
        PreviewAttack, // Previewing attack outcome
        InAction       // Executing an action
    }

    public GameState currentState;





    public int width;
    public int height;
    public float cellSize = 1f;
    public GameObject tilePrefab;

    private Node[,] grid;

    List<Node> highlightedNodes = new List<Node>();

    public GameObject characterPrefab; // Assign in the Inspector
    private GameObject playerGameObject;
    private PlayerController playerController;
    public GameObject cubePrefab;

    public GameObject UIManager;

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

        
        currentState = GameState.MenuNav;
        ChangeState(GameState.MenuNav);

    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        

        if(Input.GetMouseButtonDown(0)){

            if (currentState == GameState.MoveSelect) // Left Click for obstacles
            {
                MoveToClickedHighlightedTile();
            }
            else if(currentState == GameState.DirectionSelect){
                ChangeState(GameState.MenuNav);
            }


        }




        
        
        if (Input.GetKeyDown(KeyCode.R)) // Reset scene
        {
            ReloadScene();
        }
    }


    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return; // Prevent redundant state changes

        // Exit logic for current state
        OnExitState(currentState);

        // Set the new state
        currentState = newState;
        Debug.Log($"State changed to: {newState}");

        // Enter logic for new state
        OnEnterState(newState);
    }

    private void OnExitState(GameState state)
    {
        // Turn off UI elements when leaving a state
        switch (state)
        {
            case GameState.MenuNav:
                //menuUI.SetActive(false);
                UIManager.GetComponent<UIManager>().setActionPanelActive(false);
                UIManager.GetComponent<UIManager>().setMainPanelActive(false);
                break;
            case GameState.MoveSelect:
                //moveSelectionUI.SetActive(false);
                break;
            case GameState.DirectionSelect:
                playerController.SetDirectionIndicatorActive(false);
                //directionUI.SetActive(false);
                break;
            case GameState.ChooseTarget:
                //attackTargetUI.SetActive(false);
                break;
            case GameState.PreviewAttack:
                //attackPreviewUI.SetActive(false);
                break;
            case GameState.InAction:
                //actionExecutionUI.SetActive(false);
                break;
        }
    }

    private void OnEnterState(GameState state)
    {
        // Activate necessary UI elements when entering a new state
        switch (state)
        {
            case GameState.MenuNav:
                //menuUI.SetActive(true);
                UIManager.GetComponent<UIManager>().setActionPanelActive(true);
                UIManager.GetComponent<UIManager>().setMainPanelActive(true);
                currentState = GameState.MenuNav;
                break;
            case GameState.MoveSelect:
                //moveSelectionUI.SetActive(true);
                currentState = GameState.MoveSelect;

                break;
            case GameState.DirectionSelect:
                //directionUI.SetActive(true);
                playerController.SetDirectionIndicatorActive(true);
                currentState = GameState.DirectionSelect;


                break;
            case GameState.ChooseTarget:
                //attackTargetUI.SetActive(true);
                currentState = GameState.ChooseTarget;

                break;
            case GameState.PreviewAttack:
                //attackPreviewUI.SetActive(true);
                currentState = GameState.PreviewAttack;

                break;
            case GameState.InAction:
                //actionExecutionUI.SetActive(true);
                currentState = GameState.InAction;

                //ExecuteAction();
                break;
        }
    }



    private void ExecuteAction()
    {
        Debug.Log("Executing action...");

        // You can add animations, attack calculations, and transition to the next state
        Invoke(nameof(FinishAction), 1.5f); // Simulate action duration
    }

    private void FinishAction()
    {
        Debug.Log("Action completed! Returning to menu navigation.");
        ChangeState(GameState.MenuNav);
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
        float placementOffset = (tileHeight / 2f) + (characterHeight / 2f); //  Adjust for tile & character height

        Vector3 spawnPosition = new Vector3(spawnTile.x, spawnTile.tileObject.transform.position.y +  placementOffset, spawnTile.y); // XZ plane with correct Y height

        // Instantiate the character
        playerGameObject = Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
        playerController = playerGameObject.GetComponent<PlayerController>();

        Debug.Log("Spawn tile is " + spawnTile.x + ", " + spawnTile.y);
        playerController.currentNode = spawnTile;
        playerController.placementOffset = placementOffset;

        Camera.main.GetComponent<CameraController>().assignPlayer(playerGameObject);
    }






    /*
     * this method uses scaled cubes to generate the field, which is making certain calculations more difficult.
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
    */


    void GenerateGrid()
    {
        grid = new Node[width, height];

        float offsetX = Random.Range(0f, 100f);
        float offsetY = Random.Range(0f, 100f);

        float cubeHeight = 0.5f; // Individual cube prefab height

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseValue = Mathf.PerlinNoise((x + offsetX) * 0.1f, (y + offsetY) * 0.1f);
                int altitude = Mathf.RoundToInt(noiseValue * 10); // Determines number of stacked cubes

                GameObject topCube = null;

                for (int i = 0; i < altitude; i++)
                {
                    float yPos = (i * cubeHeight) + (cubeHeight / 2f); // Centers cube on Y

                    Vector3 position = new Vector3(x, yPos, y);
                    GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);

                    cube.name = $"Cube {x},{y},{i}";

                    // Store the top cube only
                    if (i == altitude - 1)
                    {
                        topCube = cube;
                    }
                }

                // Add only the top cube to the grid
                if (topCube != null)
                {
                    grid[x, y] = new Node(x, y, true, topCube, altitude);
                    
                }
            }
        }
    }


    public void OnMoveButtonClicked()
    {
        highlightSurroundingTiles();
        ChangeState(GameState.MoveSelect);


    }



    void MoveToClickedHighlightedTile()
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
                    

                    if(node.isHighlighted){
                        MoveCharacterToNode(node);

                    }
                    

                    break;

                }
            }
        }
    }


    public void highlightSurroundingTiles()
    {
        Debug.Log("click");
        PlayerController cc = playerGameObject.GetComponent<PlayerController>();

        List<Node> list = new List<Node>();
        list.Add(cc.currentNode);
        //Debug.Log(cc.currentNode);
        list = getSurroundingNodes(cc.move, list);


        foreach (Node highlightNode in list)
        {
            highlightedNodes.Add(highlightNode);
            highlightNode.ToggleHighlight();
        }
    }

    public void clearHighlightedTiles(){

        foreach (Node node in highlightedNodes){
            node.ToggleHighlight();
        }

        highlightedNodes.Clear();

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



    void MoveCharacterToNode(Node targetNode)
    {
        
        if (playerController == null || playerController.IsMoving) return; // Prevent movement mid-action
        GameObject clickedObject = targetNode.tileObject;

        /*
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Use Raycast to detect the clicked tile
        {
            GameObject clickedObject = hit.collider.gameObject;
            

            foreach (Node node in grid)
            {
            */
                //if (node.tileObject == clickedObject && node.isWalkable)
                if(targetNode.isWalkable)
                {
                    Vector3 characterPos = playerGameObject.transform.position;

                    // Ensure movement is on the correct Y level (top of the tile)
                    float tileHeight = targetNode.tileObject.transform.localScale.y;
                    float targetY = targetNode.tileObject.transform.position.y + (tileHeight / 2f) + 0.5f;

                    List<Node> path = pathfinding.FindPath(
                        new Vector2Int(Mathf.RoundToInt(characterPos.x), Mathf.RoundToInt(characterPos.z)),
                        new Vector2Int(targetNode.x, targetNode.y)
                    );

                    if (path != null)
                    {
                        playerController.SetPath(path);
                    }
                    
                }
            //}
        //}
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


    List<Node> getSurroundingNodes(int moves, List<Node> tilesToBeChecked)
    {
        List<Node> surroundingTiles = new List<Node>(tilesToBeChecked); // Store all found tiles
        List<Node> edgeTiles = new List<Node>(tilesToBeChecked); // Tiles to be checked in the next iteration

        while (moves > 0)
        {
            List<Node> newEdgeTiles = new List<Node>();

            foreach (Node tile in edgeTiles)
            {
                List<Node> neighbors = GetNeighbors(tile);

                foreach (Node neighbor in neighbors)
                {
                    if (!surroundingTiles.Contains(neighbor) && neighbor != null)
                    {
                        surroundingTiles.Add(neighbor);
                        newEdgeTiles.Add(neighbor);
                    }
                }
            }

            edgeTiles = newEdgeTiles; // Update edge tiles for the next iteration
            moves--;
        }

        return surroundingTiles;
    }





}

