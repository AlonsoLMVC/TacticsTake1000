using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class GameManager : MonoBehaviour
{



    public enum GameState
    {
        DestinationSelect,    // Selecting where to move
        CommandSelect,
        TargetSelect,  // Selecting an attack target
        InAction,   // Executing an action
        StandbyDirectionSelect, // Choosing facing direction
    }

    internal void HandleUnitMouseEnter(Unit unit)
    {
        throw new System.NotImplementedException();
    }

    public GameState currentState;





    public int width;
    public int height;
    public float cellSize = 1f;
    public GameObject tilePrefab;

    private Node[,] grid;

    List<Node> highlightedNodes = new List<Node>();

    public GameObject unitPrefab; // Assign in the Inspector
    public PlayerController playerController;
    public GameObject cubePrefab;
    public GameObject fillerCubePrefab;

    public UIManager uiManager;
    public static Compass Compass;

    public List<Unit> units;

    private void Start()
    {
        units = new List<Unit>();

        GenerateGrid();
        
        Compass = GameObject.FindAnyObjectByType<Compass>();

        

        units.Add(SpawnUnitWithJobAndAllegiance("Archer", true));
        units.Add(SpawnUnitWithJobAndAllegiance("Black Mage", true));
        units.Add(SpawnUnitWithJobAndAllegiance("Blue Mage", true));
        units.Add(SpawnUnitWithJobAndAllegiance("Fighter", true));
        units.Add(SpawnUnitWithJobAndAllegiance("Hunter", true));
        units.Add(SpawnUnitWithJobAndAllegiance("Illusionist", true));
        units.Add(SpawnUnitWithJobAndAllegiance("Ninja", true));
        units.Add(SpawnUnitWithJobAndAllegiance("Paladin", true));
        units.Add(SpawnUnitWithJobAndAllegiance("Soldier", true));
        units.Add(SpawnUnitWithJobAndAllegiance("Thief", true));
        units.Add(SpawnUnitWithJobAndAllegiance("White Mage", true));

        playerController.currentUnit = units[4];



        // Pass grid size to CameraController
        CameraController cameraController = FindObjectOfType<CameraController>();
        if (cameraController != null)
        {
            cameraController.gridWidth = width;
            cameraController.gridHeight = height;
        }



        StartCoroutine(ExecuteAfterDelay());




        /*
         * 
         * NEW FEATURE ALERT
         * 
         * 
         */

        InitializePathLineRenderer();


    }
    private LineRenderer pathLineRenderer;
    private void InitializePathLineRenderer()
    {
        GameObject lineObject = new GameObject("PathLine");
        pathLineRenderer = lineObject.AddComponent<LineRenderer>();

        pathLineRenderer.startWidth = 0.1f;
        pathLineRenderer.endWidth = 0.1f;
        pathLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        pathLineRenderer.positionCount = 0;
    }

    private void UpdatePathVisualization()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            foreach (Node node in grid)
            {
                if (node.tileObject == hoveredObject && node.isHighlighted)
                {
                    List<Node> path = pathfinding.FindPath(
                        new Vector2Int(playerController.currentUnit.currentNode.x, playerController.currentUnit.currentNode.y),
                        new Vector2Int(node.x, node.y)
                    );

                    if (path != null && path.Count > 0)
                    {
                        DrawPathLine(path);
                    }
                    else
                    {
                        ClearPathLine();
                    }
                    return; // Exit early since we found a valid node
                }
            }
        }

        // Clear path if no valid tile is found
        ClearPathLine();
    }

    private void DrawPathLine(List<Node> path)
    {
        if (path == null || path.Count == 0) return;

        List<Vector3> linePositions = new List<Vector3>();

        // Add the player's current node to the beginning of the path
        List<Node> fullPath = new List<Node> { playerController.currentUnit.currentNode };
        fullPath.AddRange(path);

        for (int i = 0; i < fullPath.Count; i++)
        {
            Node currentNode = fullPath[i];
            Vector3 currentPos = currentNode.tileObject.transform.position;
            currentPos.y += (currentNode.tileObject.transform.localScale.y / 2) + 0.1f; // Ensure the line is on top

            if (i == 0)
            {
                linePositions.Add(currentPos);
                continue; // First node is just added, no transitions needed
            }

            Node prevNode = fullPath[i - 1];
            Vector3 prevPos = prevNode.tileObject.transform.position;
            prevPos.y += (prevNode.tileObject.transform.localScale.y / 2) + 0.1f; // Ensure previous node is correct

            // Detect height difference using altitude
            if (Mathf.Abs(currentNode.altitude - prevNode.altitude) >= prevNode.tileObject.transform.localScale.y)
            {
                bool goingUp = currentNode.altitude > prevNode.altitude;

                // Dynamically determine where the transition happens based on tile size
                float transitionOffset = Mathf.Clamp((prevNode.tileObject.transform.localScale.x / 3f), 0.45f, 0.55f);
                if (!goingUp)
                    transitionOffset = 1f - transitionOffset; // Flip transition point for downward movement

                // Step 1: Move horizontally before or after the middle of the tile
                Vector3 midPoint1 = new Vector3(
                    Mathf.Lerp(prevPos.x, currentPos.x, transitionOffset),
                    prevPos.y, // Stay at the first tile's height
                    Mathf.Lerp(prevPos.z, currentPos.z, transitionOffset)
                );
                linePositions.Add(midPoint1);

                // Step 2: Move vertically to the new height
                Vector3 midPoint2 = new Vector3(
                    midPoint1.x,
                    currentPos.y, // Move up/down to the second tile's height
                    midPoint1.z
                );
                linePositions.Add(midPoint2);
            }

            // Step 3: Move horizontally to the next tile
            linePositions.Add(currentPos);
        }

        // Apply positions to the LineRenderer
        pathLineRenderer.positionCount = linePositions.Count;
        pathLineRenderer.SetPositions(linePositions.ToArray());
    }











    private void ClearPathLine()
    {
        pathLineRenderer.positionCount = 0;
    }




    /*
         * 
         * NEW FEATURE ALERT
         * 
         * 
         */





    IEnumerator ExecuteAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log(playerController == null);
        Debug.Log(playerController.currentUnit == null);
        playerController.switchUnit(playerController.currentUnit);

        foreach (Unit u in units)
        {
            u.directionFacing = new Vector2(0, 1);
            u.updateSpriteRotation();
        }


        playerController.currentUnit.setDirectionFacing(new Vector2(0, 1));
        Debug.Log("Executed after 2 seconds");

        ChangeState(GameState.DestinationSelect);
    }
    
    public void HandleUnitMouseExit(Unit hoveredUnit)
    {
        Debug.Log("Mouse exited: " + gameObject.name);
        // Add logic (e.g., remove highlight, reset color, etc.)
        hoveredUnit.currentNode.SetSelectionIndicatorVisibility(false);
        hoveredUnit.SetSelectionArrowVisibility(false);
        uiManager.SetFloatingPanelActive(false);
    }
    
    public void HandleNodeMouseEnter(Node hoveredNode)
    {
        switch (currentState)
        {

            case GameState.DestinationSelect:
                hoveredNode.SetSelectionIndicatorVisibility(true); 
                
                if (hoveredNode.hasUnitOnTile) 
                {
                    Debug.Log("there is a unit on  this tile");

                    foreach(Unit u in units)
                    {
                        if(u.currentNode == this)
                        {
                            u.SetSelectionArrowVisibility(true);
                            uiManager.SetFloatingPanelActive(true);
                            uiManager.floatingPanel.UpdateFloatingPanel(u.Name, u.Level, null, u.currentHP, u.maxHP);

                    
                        }
                    }
            
                }
                else
                {
                    Debug.Log("there is NOT a unit on  this tile");

                    hoveredNode.SetSelectionArrowVisibility(true);
            
            
                }
                
                UpdatePathVisualization(); // Runs separately from other logic
        
                

                break;
            case GameState.CommandSelect:
                playerController.currentHoveredNode = hoveredNode;

                faceHoveredDirection();
                

                //directionUI.SetActive(false);
                break;
            case GameState.TargetSelect:
                playerController.currentHoveredNode = hoveredNode;

                faceHoveredDirection();
                

                break;
            case GameState.InAction:

                //actionExecutionUI.SetActive(false);
                break;
            case GameState.StandbyDirectionSelect:
                
                faceHoveredDirection();
                break;
        }
        
        
        
        



        
    }
    
    public void HandleNodeMouseExit(Node hoveredNode)
    {
        hoveredNode.SetSelectionIndicatorVisibility(false);

        
        hoveredNode.SetSelectionArrowVisibility(false);

        if (hoveredNode.hasUnitOnTile)
        {
            uiManager.SetFloatingPanelActive(false);

        }
        //god this is silly
        foreach (Unit u in units)
        {
            
            u.SetSelectionArrowVisibility(false);

            
        }
    }
    
    


    public void faceHoveredDirection()
    {
        
        if (playerController == null)
        {
            Debug.LogError("playerController is NULL!");
            return;
        }

        if (playerController.currentUnit == null)
        {
            Debug.LogError("playerController.currentUnit is NULL!");
            return;
        }

        if (playerController.currentUnit.currentNode == null)
        {
            Debug.LogError("playerController.currentUnit.currentNode is NULL!");
            return;
        }

        if (playerController.currentHoveredNode == null)
        {
            Debug.LogError("playerController.currentHoveredNode is NULL!");
            return;
        }

        if (Compass == null)
        {
            Debug.LogError("Compass is NULL!");
            return;
        }

        Debug.Log("Current node coordinates: " + playerController.currentUnit.currentNode.getGridCoordinates());
        Debug.Log("Hovered node coordinates: " + playerController.currentHoveredNode.getGridCoordinates());

        Vector2 newDirectionToFace = Compass
            .GetClosestGridDirection(playerController.currentUnit.currentNode.getGridCoordinates(), playerController.currentHoveredNode.getGridCoordinates());

        playerController.currentUnit.setDirectionFacing(newDirectionToFace);
        playerController.currentUnit.setIndicatorDirectionFacing(newDirectionToFace);



                    
        
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
        switch (currentState)
        {

            case GameState.DestinationSelect:
                //moveSelectionUI.SetActive(false);
                clearHighlightedTiles();

                break;
            case GameState.CommandSelect:
                

                //directionUI.SetActive(false);
                break;
            case GameState.TargetSelect:

                //attackTargetUI.SetActive(false);
                break;
            case GameState.InAction:

                //actionExecutionUI.SetActive(false);
                break;
            case GameState.StandbyDirectionSelect:

                playerController.SetDirectionIndicatorActive(false);
                //directionUI.SetActive(false);
                break;
        }

    }

    private void OnEnterState(GameState state)
    {
        // Activate necessary UI elements when entering a new state
        switch (currentState)
        {

            case GameState.DestinationSelect:
                //moveSelectionUI.SetActive(true);


                highlightedNodes = GetReachableTiles(playerController.currentUnit.currentNode, playerController.currentUnit.MoveRange);
                foreach (Node tile in highlightedNodes)
                {
                    tile.SetHighlightVisibility(true);
                }


                uiManager.SetUIState(GameState.DestinationSelect);
                
                playerController.currentUnit.currentNode.SetSelectionIndicatorModeAlly();
                playerController.currentUnit.currentNode.SetSelectionIndicatorVisibility(true);
                
                currentState = GameState.DestinationSelect;

                break;
            case GameState.CommandSelect:
                //attackPreviewUI.SetActive(true);
                uiManager.SetUIState(GameState.CommandSelect);
                currentState = GameState.CommandSelect;

                break;
            case GameState.TargetSelect:
                //attackTargetUI.SetActive(true);
                uiManager.SetUIState(GameState.TargetSelect);
                currentState = GameState.TargetSelect;

                break;
            case GameState.InAction:
                //actionExecutionUI.SetActive(true);
                uiManager.SetUIState(GameState.InAction);
                currentState = GameState.InAction;

                //ExecuteAction();
                break;
            case GameState.StandbyDirectionSelect:
                //directionUI.SetActive(true);
                uiManager.SetUIState(GameState.StandbyDirectionSelect);
                playerController.SetDirectionIndicatorActive(true);
                playerController.currentUnit.gameObject.GetComponent<Unit>().directionIndicator.ToggleSpheres(true);
                currentState = GameState.StandbyDirectionSelect;


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
        //ChangeState(GameState.MenuNav);
    }




    Unit SpawnUnitWithJobAndAllegiance(string job, bool isAllied)
    {
        List<Node> walkableTiles = new List<Node>();
        foreach (Node node in grid)
        {
            if (node.isWalkable)
                walkableTiles.Add(node);
        }

        if (walkableTiles.Count == 0) return null;

        // Pick a random walkable tile
        Node spawnNode = walkableTiles[Random.Range(0, walkableTiles.Count)];

        float tileHeight = spawnNode.tileObject.transform.localScale.y; // Get the cube's height
        float characterHeight = 1f; // Change this based on your character's height
        float placementOffset = (tileHeight / 2f) + (characterHeight / 2f); //  Adjust for tile & character height

        Vector3 spawnPosition = new Vector3(spawnNode.x, spawnNode.tileObject.transform.position.y + placementOffset, spawnNode.y); // XZ plane with correct Y height

        // Instantiate the character
        Unit newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity).GetComponent<Unit>();

        newUnit.setJobandAllegianceAndInitialize(job, isAllied);
        newUnit.uiManager = uiManager;


        //Debug.Log("Spawn tile is " + spawnNode.x + ", " + spawnNode.y);
        newUnit.currentNode = spawnNode;
        playerController.placementOffset = placementOffset;

        Camera.main.GetComponent<CameraController>().assignPlayer(newUnit.gameObject);


        spawnNode.isWalkable = false;
        spawnNode.hasUnitOnTile = true;


        return newUnit;

    }







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
                    GameObject cube;

                    if (i == altitude - 1)
                    {
                        cube = Instantiate(cubePrefab, position, Quaternion.identity);
                        topCube = cube;
                        cube.GetComponent<Node>().setValues(x, y, true, topCube, altitude);
                        cube.GetComponent<Node>().gameManager = this;
                        cube.GetComponent<Node>().playerController = playerController;
                        grid[x, y] = cube.GetComponent<Node>();


                    }
                    else
                    {
                        cube = Instantiate(fillerCubePrefab, position, Quaternion.identity);
                    }





                    cube.name = $"Cube {x},{y},{i}";

                    // Store the top cube only

                }
                /*
                // Add only the top cube to the grid
                if (topCube != null)
                {
                    grid[x, y] = new Node(x, y, true, topCube, altitude);
                    
                }
                */
            }
        }
    }


    public void OnMoveButtonClicked()
    {
        Debug.Log("Move Button Clicked");

        highlightSurroundingTiles(playerController.currentUnit.gameObject.GetComponent<Unit>().MoveRange);
        ChangeState(GameState.DestinationSelect);


    }


    public void OnAttackButtonClicked()
    {
        Debug.Log("Attack Button Clicked");


        //honestly for now we are going to use just the surrounding tiles for possible attack tiles
        highlightSurroundingTiles(1);

        ChangeState(GameState.TargetSelect);

        playerController.nextAttackIsMagic = false;

    }

    public void OnMagicAttackButtonClicked()
    {
        Debug.Log("Magic Attack Button Clicked");


        //honestly for now we are going to use just the surrounding tiles for possible attack tiles
        highlightSurroundingTiles(3);

        ChangeState(GameState.TargetSelect);

        playerController.nextAttackIsMagic = true;

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


                    if (node.isHighlighted)
                    {
                        MoveCharacterToNode(node);

                    }


                    break;

                }
            }
        }
    }


    void AttackClickedHighlightTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            Debug.Log("Clicked: " + clickedObject.name);

            // Check if it's a Tile
            foreach (Node node in grid)
            {
                if (node.tileObject == clickedObject)
                {
                    if (node.isHighlighted)
                    {
                        playerController.attack();
                    }
                    return; // Stop checking once we find a match
                }
            }

            // Check if it's part of a Unit
            Unit clickedUnit = hit.collider.GetComponentInParent<Unit>();
            if (clickedUnit != null)
            {
                Debug.Log("Clicked a unit: " + clickedUnit.name);
                playerController.attack();
                return; // Prevent unnecessary extra checks
            }
        }
    }





    public void highlightSurroundingTiles(int move)
    {
        Debug.Log("click");
        PlayerController pc = FindAnyObjectByType<PlayerController>();

        List<Node> list = new List<Node>();
        list.Add(pc.currentUnit.currentNode);
        //Debug.Log(cc.currentNode);
        list = getAreaAroundNodes(move, list);


        foreach (Node highlightNode in list)
        {
            //Don't highlight the node the player is on
            if (highlightNode == pc.currentUnit.currentNode) continue;

            highlightedNodes.Add(highlightNode);
            highlightNode.SetHighlightVisibility(true);
            highlightNode.isHighlighted = true;

        }
    }

    public void clearHighlightedTiles()
    {

        foreach (Node node in highlightedNodes)
        {
            node.SetHighlightVisibility(false);
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


        if (targetNode.isWalkable)
        {
            Vector3 characterPos = playerController.currentUnit.gameObject.transform.position;

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


    List<Node> getAreaAroundNodes(int moves, List<Node> tilesToBeChecked)
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


    public List<Node> GetReachableTiles(Node startNode, int moves)
    {
        List<Node> reachableTiles = new List<Node>();
        Dictionary<Node, int> moveCostMap = new Dictionary<Node, int>(); // Track remaining moves per node

        Queue<(Node node, int remainingMoves)> frontier = new Queue<(Node, int)>();
        frontier.Enqueue((startNode, moves));
        moveCostMap[startNode] = moves;

        while (frontier.Count > 0)
        {
            var (currentNode, remainingMoves) = frontier.Dequeue();

            if (remainingMoves <= 0) continue; // Stop if no moves left

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable) continue; // Skip unwalkable tiles

                int costToEnter = 1; // Modify if different terrain has different costs
                int newRemainingMoves = remainingMoves - costToEnter;

                if (!moveCostMap.ContainsKey(neighbor) || newRemainingMoves > moveCostMap[neighbor])
                {
                    moveCostMap[neighbor] = newRemainingMoves;
                    reachableTiles.Add(neighbor);
                    frontier.Enqueue((neighbor, newRemainingMoves));
                }
            }
        }

        return reachableTiles;
    }



    public void endAttack()
    {

        ChangeState(GameManager.GameState.StandbyDirectionSelect);
    }


}

