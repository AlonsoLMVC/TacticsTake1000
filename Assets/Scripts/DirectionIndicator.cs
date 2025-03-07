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
        if (spheres.Length == 0) return;

        GameObject targetSphere = null;
        float bestDot = -Mathf.Infinity;

        foreach (GameObject sphere in spheres)
        {
            Vector3 localPos = transform.InverseTransformPoint(sphere.transform.position);
            Vector2 sphereDirection = new Vector2(localPos.x, localPos.z).normalized;

            float dotProduct = Vector2.Dot(direction.normalized, sphereDirection);

            if (dotProduct > bestDot)
            {
                bestDot = dotProduct;
                targetSphere = sphere;
            }
        }

        // Update enlarged sphere
        currentEnlargedSphere = targetSphere;

        
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
