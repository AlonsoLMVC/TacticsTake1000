using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public GameObject[] spheres; // Assign the spheres in the Unity Editor
    public float defaultScale = 1f;
    public float enlargedScale = 2f;
    private GameObject currentEnlargedSphere;
    private bool spheresActive = true; // Tracks whether spheres are active

    public GameObject player;

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
                player.GetComponent<PlayerController>().updateSpriteWithBlendTreeVector();
            }
            else
            {
                sphere.transform.localScale = Vector3.one * defaultScale;
                player.GetComponent<PlayerController>().updateSpriteWithBlendTreeVector();

            }
        }


        
    }

    public Transform GetCurrentEnlargedSphere()
    {
        return currentEnlargedSphere != null ? currentEnlargedSphere.transform : null;
    }




    // Logs the X, Z location of the enlarged sphere
    public Vector2 getEnlargedSpherePosition()
    {
        if (currentEnlargedSphere != null)
        {
            Vector3 pos = transform.InverseTransformPoint(currentEnlargedSphere.transform.position);
            //Debug.Log($"Enlarged Sphere Position -> X: {pos.x}, Z: {pos.z}");

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
