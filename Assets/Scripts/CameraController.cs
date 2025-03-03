using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 5f;
    public float rotationAroundPlayerSpeed = 300f; // Speed for rotating around player


    private float minZoom = 3f;
    private float maxZoom = 20f;
    private Camera cam;

    public int gridWidth = 10;  // Assigned from GridManager
    public int gridHeight = 10; // Assigned from GridManager

    public Transform playerTransform; // Reference to the player
    private bool isRotating = false;
    private float targetAngle;



    private void Start()
    {
        cam = Camera.main;
        CenterCamera();
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
        HandlePlayerRotation();
    }

    void CenterCamera()
    {
        Vector3 gridCenter = new Vector3(gridWidth / 2f, 0, gridHeight / 2f); // Middle of the grid

        //float heightOffset = Mathf.Max(gridWidth, gridHeight) * 0.6f; // Raise camera based on grid size
        float heightOffset = Mathf.Max(gridWidth, gridHeight) * 1.2f; // Raise camera based on grid size
        float distance = Mathf.Max(gridWidth, gridHeight) * 1.2f; // Adjust how far back the camera is

        // New positioning for Final Fantasy Tactics style
        transform.position = gridCenter + new Vector3(-distance, heightOffset, -distance); // Offset for isometric view
        transform.LookAt(gridCenter); // Ensure the camera looks at the grid center

        // Adjust orthographic size dynamically for better zoom level
        Camera.main.orthographicSize = Mathf.Max(gridWidth, gridHeight) * 0.65f;

        //  NEW: Set the correct Final Fantasy Tactics camera angle
        transform.rotation = Quaternion.Euler(30, 45, 0); // 30Åã tilt, 45Åã rotation
    }







    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ);
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

    void HandlePlayerRotation()
    {
        if (Input.GetKeyDown(KeyCode.T) && playerTransform.gameObject != null && !isRotating)
        {
            isRotating = true;
            targetAngle = transform.eulerAngles.y + 90f; // Set target angle 90 degrees from current rotation
        }

        if (isRotating)
        {
            float step = rotationAroundPlayerSpeed * Time.deltaTime;
            float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, step);
            transform.RotateAround(playerTransform.position, Vector3.up, newAngle - transform.eulerAngles.y);

            if (Mathf.Abs(newAngle - targetAngle) < 0.1f)
            {
                isRotating = false;
            }
        }
    }
}
