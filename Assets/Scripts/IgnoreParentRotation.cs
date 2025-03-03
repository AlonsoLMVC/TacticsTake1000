using UnityEngine;

public class IgnoreParentRotation : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation; // Store initial world rotation
    }

    void LateUpdate()
    {
        transform.rotation = initialRotation; // Lock rotation every frame
    }
}
