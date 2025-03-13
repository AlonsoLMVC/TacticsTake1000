using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public GameObject[] spheres; // Assign the spheres in the Unity Editor
    public float defaultScale = 1f;
    public float enlargedScale = 2f;
    private GameObject currentEnlargedSphere;
    private bool spheresActive = true; // Tracks whether spheres are active

    public GameObject player;

    // Dictionary to map directions to spheres (optional if you want a predefined mapping)
    private Vector2[] directions = {
        new Vector2(1, 0),  // NE
        new Vector2(0, -1), // SE
        new Vector2(-1, 0), // SW
        new Vector2(0, 1)   // NW
    };

    public void SetEnlargedSphere(Vector2 direction)
    {
        // Convert the input direction to match the expected coordinate system
        direction = new Vector2(direction.x, -direction.y).normalized; // Invert Y-axis

        float closestDot = -1f; // Lowest possible dot product
        GameObject closestSphere = null;

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2 sphereDirection = directions[i].normalized;

            // Use dot product to find the closest matching direction
            float dot = Vector2.Dot(direction, sphereDirection);

            if (dot > closestDot)
            {
                closestDot = dot;
                closestSphere = spheres[i];
            }
        }

        // If we found a matching sphere
        if (closestSphere != null)
        {
            // Reset the previous enlarged sphere
            if (currentEnlargedSphere != null)
            {
                currentEnlargedSphere.transform.localScale = Vector3.one * defaultScale;
            }

            // Enlarge the new sphere
            closestSphere.transform.localScale = Vector3.one * enlargedScale;
            currentEnlargedSphere = closestSphere;
        }
    }


    // Get the currently enlarged sphere
    public Transform GetCurrentEnlargedSphere()
    {
        return currentEnlargedSphere != null ? currentEnlargedSphere.transform : null;
    }

    // Get the X, Z position of the enlarged sphere
    public Vector2 GetEnlargedSpherePosition()
    {
        if (currentEnlargedSphere != null)
        {
            Vector3 pos = transform.InverseTransformPoint(currentEnlargedSphere.transform.position);
            return new Vector2(pos.x, pos.z);
        }
        else
        {
            Debug.Log("No sphere is currently enlarged.");
            return Vector2.zero;
        }
    }

    // Toggles the spheres' active state
    public void ToggleSpheres(bool isActive)
    {
        spheresActive = isActive;
        foreach (GameObject sphere in spheres)
        {
            sphere.SetActive(isActive);
        }
    }
}
