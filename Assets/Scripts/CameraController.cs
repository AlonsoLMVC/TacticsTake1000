using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 5f;
    public float rotationSpeed = 300f; // Speed for rotating around the grid

    private float minZoom = 3f;
    private float maxZoom = 20f;
    private Camera cam;

    public int gridWidth = 10;  // Assigned from GridManager
    public int gridHeight = 10; // Assigned from GridManager

    public Transform playerTransform; // Reference to the player
    private bool isRotating = false;
    private float targetAngle;
    private Vector3 gridCenter; // Stores the center of the grid

    public GameObject compass;
    private PlayerController playerController;

    private void Start()
    {
        cam = Camera.main;
        CalculateGridCenter(); // Ensure this is correctly calculated
        CenterCamera();

    }


    private void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleGridRotation();
    }

    void CalculateGridCenter()
    {
        // Ensure the center accounts for an odd/even width & height
        float centerX = (gridWidth - 1) / 2f;
        float centerZ = (gridHeight - 1) / 2f;

        gridCenter = new Vector3(centerX, 0, centerZ); // Adjusted to better align
    }

    void CenterCamera()
    {
        float heightOffset = Mathf.Max(gridWidth, gridHeight) * 1.2f;
        float distance = Mathf.Max(gridWidth, gridHeight) * 1.2f;

        // Position the camera in an isometric style
        transform.position = gridCenter + new Vector3(-distance, heightOffset, -distance);
        transform.LookAt(gridCenter);

        // Adjust orthographic size dynamically for better zoom level
        cam.orthographicSize = Mathf.Max(gridWidth, gridHeight) * 0.65f;

        // Set correct Final Fantasy Tactics camera angle (30Åã tilt, 45Åã initial rotation)
        transform.rotation = Quaternion.Euler(30, 45, 0);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 right = transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up); // Get forward without vertical component

        Vector3 moveDirection = (right * moveX + forward * moveZ).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    public void assignPlayer(GameObject playerGameObject)
    {
        playerTransform = playerGameObject.transform;
    }


    float halfwayAngle;
    bool hasTriggeredHalfway;
    void HandleGridRotation()
    {
        
       
        


        if ((Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.G)) && !isRotating)
        {
            isRotating = true;

            // Determine rotation direction: T = clockwise (right), G = counterclockwise (left)
            int direction = Input.GetKeyDown(KeyCode.T) ? 1 : -1;

            // Adjust rotation to consider the initial 45-degree starting rotation
            targetAngle = Mathf.Round((transform.eulerAngles.y - 45f) / 90f) * 90f + (90f * direction) + 45f;

            // Calculate the halfway point
            halfwayAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, 0.5f);
            hasTriggeredHalfway = false; // Reset halfway trigger
        }

        if (isRotating)
        {
            Debug.Log("Rotating");
            float step = rotationSpeed * Time.deltaTime;
            float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, step);
            transform.RotateAround(gridCenter, Vector3.up, newAngle - transform.eulerAngles.y);

            Debug.Log($"newAngle: {newAngle}, halfwayAngle: {halfwayAngle}, Difference: {Mathf.Abs(newAngle - halfwayAngle)}");

            // Check if we've reached the halfway point and trigger logic
            if (!hasTriggeredHalfway && Mathf.Abs(newAngle - halfwayAngle) < 2f)
            {
                hasTriggeredHalfway = true; // Prevent duplicate calls

                Debug.Log("we get in HERE");

                if (targetAngle > transform.eulerAngles.y) // Clockwise
                {


                    Debug.Log("halfway");

                    compass.GetComponent<Compass>().compassShiftCounterClockwise();
                    playerController = GameObject.FindAnyObjectByType<PlayerController>();
                    playerController.updateSpriteRotation();
                    


                }
                else // Counterclockwise
                {

                    Debug.Log("halfway");

                    compass.GetComponent<Compass>().compassShiftClockwise();
                    playerController = GameObject.FindAnyObjectByType<PlayerController>();
                    playerController.updateSpriteRotation();



                }
            }

            if (Mathf.Abs(newAngle - targetAngle) < 0.1f)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);
                isRotating = false;
                Debug.Log("Done rotating");
            }
        }
    }





}
