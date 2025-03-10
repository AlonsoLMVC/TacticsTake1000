using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

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

    public GameObject unitPrefab; // Assign in the Inspector
    public PlayerController playerController;
    public GameObject cubePrefab;

    public GameObject uiManagerObject;
    public GameObject compassGameObject;

    public List<Unit> units;

    private void Start()
    {
        units = new List<Unit>();

        GenerateGrid();
        


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

        playerController.currentUnit = units[0];
        


        // Pass grid size to CameraController
        CameraController cameraController = FindObjectOfType<CameraController>();
        if (cameraController != null)
        {
            cameraController.gridWidth = width;
            cameraController.gridHeight = height;
        }

        
        currentState = GameState.MenuNav;
        ChangeState(GameState.MenuNav);

        StartCoroutine(ExecuteAfterDelay());

    }

    IEnumerator ExecuteAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        foreach(Unit u in units){
            u.directionFacing = new Vector2(0,1);
            u.updateSpriteRotation();
        }


        playerController.setDirectionFacing(new Vector2(0, 1));
        Debug.Log("Executed after 2 seconds");
    }


    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (currentState == GameState.ChooseTarget || currentState == GameState.MoveSelect || currentState == GameState.DirectionSelect)
        {
            //ChangeState(GameState.InAction);
            vocalTiles();
            //AttackClickedHighlightTile();
        }

        if (Input.GetMouseButtonDown(0)){

            if (currentState == GameState.MoveSelect) // Left Click for obstacles
            {
                MoveToClickedHighlightedTile();
            }
            else if(currentState == GameState.DirectionSelect){
                ChangeState(GameState.MenuNav);
            }
            else if(currentState == GameState.ChooseTarget)
            {
                //ChangeState(GameState.InAction);
                
                AttackClickedHighlightTile();
            }


        }




        
        
        if (Input.GetKeyDown(KeyCode.R)) // Reset scene
        {
            ReloadScene();
        }
    }

    public void vocalTiles()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            // Check if the hovered object is a tile
            foreach (Node node in grid)
            {
                if (node.tileObject == hoveredObject)
                {
                    //Debug.Log("Hovered node is " + node.x + ", " + node.y);
                    //Debug.Log("Current node is " + playerController.currentNode);

                    Vector2 newDirectionToFace = compassGameObject.GetComponent<Compass>()
                        .GetClosestGridDirection(playerController.currentUnit.currentNode.getGridCoordinates(), node.getGridCoordinates());

                    
                    playerController.setDirectionFacing(newDirectionToFace);
                    playerController.setIndicatorDirectionFacing(newDirectionToFace);



                    break; // Exit loop once we find the hovered tile
                }
            }
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
                uiManagerObject.GetComponent<UIManager>().setActionPanelActive(false);
                uiManagerObject.GetComponent<UIManager>().setMainPanelActive(false);
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
                uiManagerObject.GetComponent<UIManager>().setActionPanelActive(false);
                uiManagerObject.GetComponent<UIManager>().setMainPanelActive(true);
                currentState = GameState.MenuNav;
                break;
            case GameState.MoveSelect:
                //moveSelectionUI.SetActive(true);
                currentState = GameState.MoveSelect;

                break;
            case GameState.DirectionSelect:
                //directionUI.SetActive(true);
                playerController.SetDirectionIndicatorActive(true);
                playerController.currentUnit.gameObject.GetComponent<Unit>().directionIndicator.ToggleSpheres(true);
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




    /*
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
        playerController.currentUnitGameObject = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);  

        Debug.Log("Spawn tile is " + spawnTile.x + ", " + spawnTile.y);
        playerController.currentNode = spawnTile;
        playerController.placementOffset = placementOffset;

        Camera.main.GetComponent<CameraController>().assignPlayer(playerController.currentUnitGameObject );

        playerController.assignStartingUnit(playerController.currentUnitGameObject );
    }
    */

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
        Node spawnTile = walkableTiles[Random.Range(0, walkableTiles.Count)];

        float tileHeight = spawnTile.tileObject.transform.localScale.y; // Get the cube's height
        float characterHeight = 1f; // Change this based on your character's height
        float placementOffset = (tileHeight / 2f) + (characterHeight / 2f); //  Adjust for tile & character height

        Vector3 spawnPosition = new Vector3(spawnTile.x, spawnTile.tileObject.transform.position.y + placementOffset, spawnTile.y); // XZ plane with correct Y height

        // Instantiate the character
        Unit newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity).GetComponent<Unit>();

        newUnit.setJobandAllegianceAndInitialize(job, isAllied);

        Debug.Log("Spawn tile is " + spawnTile.x + ", " + spawnTile.y);
        newUnit.currentNode = spawnTile;
        playerController.placementOffset = placementOffset;

        Camera.main.GetComponent<CameraController>().assignPlayer(newUnit.gameObject);

        playerController.assignStartingUnit(playerController.currentUnit);

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
        Debug.Log("Move Button Clicked");

        highlightSurroundingTiles(playerController.currentUnit.gameObject.GetComponent<Unit>().MoveRange);
        ChangeState(GameState.MoveSelect);


    }

    public void OnActButtonClicked()
    {
        Debug.Log("Act Button Clicked");

        uiManagerObject.GetComponent<UIManager>().setActionPanelActive(true);


    }


    public void OnAttackButtonClicked()
    {
        Debug.Log("Attack Button Clicked");


        //honestly for now we are going to use just the surrounding tiles for possible attack tiles
        highlightSurroundingTiles(1);

        ChangeState(GameState.ChooseTarget);

        playerController.nextAttackIsMagic = false;

    }

    public void OnMagicAttackButtonClicked()
    {
        Debug.Log("Magic Attack Button Clicked");


        //honestly for now we are going to use just the surrounding tiles for possible attack tiles
        highlightSurroundingTiles(3);

        ChangeState(GameState.ChooseTarget);

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
                    

                    if(node.isHighlighted){
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

            // Check if the clicked object is a tile
            foreach (Node node in grid)
            {
                if (node.tileObject == clickedObject)
                {


                    if (node.isHighlighted)
                    {
                        playerController.attack();

                    }


                    break;

                }
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

        
        if(targetNode.isWalkable)
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


    public void endAttack()
    {

        ChangeState(GameManager.GameState.DirectionSelect);
    }


}

