using UnityEngine;

public class UprightBillboard : MonoBehaviour
{
    public float fixedXRotation = 45f; // Ensures the sprite always stays rotated correctly

    void LateUpdate()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 lookDirection = (transform.position - cameraPosition).normalized;
        lookDirection.y = 0; // Keeps the sprite upright (no tilting)

        transform.rotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Euler(fixedXRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
