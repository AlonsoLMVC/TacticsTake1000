using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public GameObject[] spheres; // Assign the spheres in the Unity Editor
    public float defaultScale = 1f;
    public float enlargedScale = 2f;
    private GameObject currentEnlargedSphere;
    private bool spheresActive = true; // Tracks whether spheres are active

    void Update()
    {
        if (!spheresActive || spheres.Length == 0) return;

        GameObject closestSphere = null;
        float closestDistance = float.MaxValue;

        Vector3 mousePosition = Input.mousePosition;

        foreach (GameObject sphere in spheres)
        {
            if (!sphere.activeSelf) continue;

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(sphere.transform.position);
            float distance = Vector2.Distance(new Vector2(mousePosition.x, mousePosition.y), new Vector2(screenPosition.x, screenPosition.y));

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSphere = sphere;
            }
        }

        currentEnlargedSphere = closestSphere;

        foreach (GameObject sphere in spheres)
        {
            if (sphere == closestSphere)
            {
                sphere.transform.localScale = Vector3.one * enlargedScale;
            }
            else
            {
                sphere.transform.localScale = Vector3.one * defaultScale;
            }
        }
    }

    public Transform GetCurrentEnlargedSphere()
    {
        return currentEnlargedSphere != null ? currentEnlargedSphere.transform : null;
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
