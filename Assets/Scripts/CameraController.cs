using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 5f;
    public float rotationSpeed = 100f;

    private float minZoom = 3f;
    private float maxZoom = 20f;
    private Camera cam;

    public int gridWidth = 10;  // Assigned from GridManager
    public int gridHeight = 10; // Assigned from GridManager

    private void Start()
    {
        cam = Camera.main;
        CenterCamera();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
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

    void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            float rotateX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.RotateAround(Vector3.zero, Vector3.up, rotateX);
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}
