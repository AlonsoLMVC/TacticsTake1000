using System.Collections;
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
    public GameManager gameManager;

    // Variables to store the original camera position, rotation, and zoom
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float originalZoom;
    private bool isZoomedIn = false; // Track whether we are zoomed in

    private Coroutine zoomCoroutine; // To manage transitions


    private void Start()
    {
        cam = Camera.main;
        CalculateGridCenter(); // Ensure this is correctly calculated
        CenterCamera();

    }


    private void Update()
    {
        //HandleMovement();
        //HandleZoom();
        HandleGridRotation();
        //HandlePlayerZoom(); // New function for zooming on player
    /*
    if (Input.GetKeyDown(KeyCode.C)) // Press C to center without zooming
    {
        CenterCameraOnPlayer();
    }
    */

    }


    public void SmoothFocusOnTarget(Transform target, float duration = 0.5f)
{
    if (target == null) return;

    // Stop any ongoing transition
    if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);

    // Calculate camera position and rotation
    float heightOffset = Mathf.Max(gridWidth, gridHeight) * 1.2f;
    float distance = Mathf.Max(gridWidth, gridHeight) * 1.2f;

    Vector3 targetPosition = target.position + new Vector3(-distance, heightOffset, -distance);
    Quaternion targetRotation = Quaternion.Euler(30, 45, 0); // Standard isometric view

    // Start smooth transition
    zoomCoroutine = StartCoroutine(SmoothCameraTransition(targetPosition, targetRotation, cam.orthographicSize, duration));
}





    // New method: Centers camera on player without zooming
    public void CenterCameraOnPlayer()
    {
        if (playerTransform == null) return;

        float heightOffset = Mathf.Max(gridWidth, gridHeight) * 1.2f;
        float distance = Mathf.Max(gridWidth, gridHeight) * 1.2f;

        Vector3 targetPosition = playerTransform.position + new Vector3(-distance, heightOffset, -distance);
        Quaternion targetRotation = Quaternion.Euler(30, 45, 0);

        // Start smooth transition but keep current zoom
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(SmoothCameraTransition(targetPosition, targetRotation, cam.orthographicSize, 0.5f));
    }

    void HandlePlayerZoom()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isZoomedIn)
            {
                // Store original camera state before zooming in
                originalPosition = transform.position;
                originalRotation = transform.rotation;
                originalZoom = cam.orthographicSize;

                if (playerTransform != null)
                {
                    float heightOffset = Mathf.Max(gridWidth, gridHeight) * 0.5f; // Closer than normal
                    float distance = Mathf.Max(gridWidth, gridHeight) * 0.5f; // Adjust zoom distance

                    Vector3 targetPosition = playerTransform.position + new Vector3(-distance, heightOffset, -distance);
                    Quaternion targetRotation = Quaternion.Euler(30, 45, 0);
                    float targetZoom = Mathf.Max(gridWidth, gridHeight) * 0.3f;

                    // Start smooth transition
                    if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
                    zoomCoroutine = StartCoroutine(SmoothCameraTransition(targetPosition, targetRotation, targetZoom, 0.2f));
                }

                isZoomedIn = true;
            }
            else
            {
                // Smoothly return to original position
                if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
                zoomCoroutine = StartCoroutine(SmoothCameraTransition(originalPosition, originalRotation, originalZoom, 0.2f));

                isZoomedIn = false;
            }
        }
    }


    void HandlePlayerFocus()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isZoomedIn)
            {
                // Store original camera state before zooming in
                originalPosition = transform.position;
                originalRotation = transform.rotation;
                originalZoom = cam.orthographicSize;

                if (playerTransform != null)
                {
                    float heightOffset = Mathf.Max(gridWidth, gridHeight) * 0.5f;
                    float distance = Mathf.Max(gridWidth, gridHeight) * 0.5f;

                    Vector3 targetPosition = playerTransform.position + new Vector3(-distance, heightOffset, -distance);
                    Quaternion targetRotation = Quaternion.Euler(30, 45, 0);
                    float targetZoom = Mathf.Max(gridWidth, gridHeight) * 0.3f;

                    // Start smooth transition
                    if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
                    zoomCoroutine = StartCoroutine(SmoothCameraTransition(targetPosition, targetRotation, targetZoom, 0.5f));
                }

                isZoomedIn = true;
            }
            else
            {
                // Smoothly return to original position
                if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
                zoomCoroutine = StartCoroutine(SmoothCameraTransition(originalPosition, originalRotation, originalZoom, 0.5f));

                isZoomedIn = false;
            }
        }
    }



    // Coroutine to smoothly transition between camera positions
    IEnumerator SmoothCameraTransition(Vector3 targetPosition, Quaternion targetRotation, float targetZoom, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float startZoom = cam.orthographicSize;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            cam.orthographicSize = Mathf.Lerp(startZoom, targetZoom, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are set to avoid small floating-point errors
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        cam.orthographicSize = targetZoom;
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

        // Set correct Final Fantasy Tactics camera angle (30�� tilt, 45�� initial rotation)
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
            //Debug.Log("Rotating");
            float step = rotationSpeed * Time.deltaTime;
            float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, step);
            transform.RotateAround(gridCenter, Vector3.up, newAngle - transform.eulerAngles.y);

           // Debug.Log($"newAngle: {newAngle}, halfwayAngle: {halfwayAngle}, Difference: {Mathf.Abs(newAngle - halfwayAngle)}");

            // Check if we've reached the halfway point and trigger logic
            if (!hasTriggeredHalfway && Mathf.Abs(newAngle - halfwayAngle) < 2f)
            {
                hasTriggeredHalfway = true; // Prevent duplicate calls

               // Debug.Log("we get in HERE");

                if (targetAngle > transform.eulerAngles.y) // Clockwise
                {


                    //Debug.Log("halfway");

                    compass.GetComponent<Compass>().compassShiftCounterClockwise();  

                    foreach(Unit u in gameManager.units)
                    {
                        u.updateSpriteRotation();
                    }
                    //playerController = GameObject.FindAnyObjectByType<PlayerController>();
                    //playerController.currentUnitGameObject.GetComponent<Unit>().updateSpriteRotation();
                    


                }
                else // Counterclockwise
                {

                    //Debug.Log("halfway");

                    compass.GetComponent<Compass>().compassShiftClockwise();

                    foreach(Unit u in gameManager.units)
                    {
                        u.updateSpriteRotation();
                    }

                    //playerController = GameObject.FindAnyObjectByType<PlayerController>();
                    //playerController.currentUnit.updateSpriteRotation();



                }
            }

            if (Mathf.Abs(newAngle - targetAngle) < 0.1f)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);
                isRotating = false;
                //Debug.Log("Done rotating");
            }
        }
    }





}
