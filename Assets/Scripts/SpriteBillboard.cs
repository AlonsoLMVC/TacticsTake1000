using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (cameraTransform != null)
        {
            Vector3 targetRotation = transform.eulerAngles;
            targetRotation.y = cameraTransform.eulerAngles.y; // Match Y rotation
            transform.eulerAngles = targetRotation;
        }
    }
}
