using UnityEngine;

public class CameraBasedAnimationScript : MonoBehaviour
{
    private Animator animator; // Animator of the child sprite
    private SpriteRenderer spriteRenderer; // SpriteRenderer of the child sprite
    private Vector3 lastScreenPosition;
    private bool lastMovingUp = true; // Tracks last direction (true = up, false = down)

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
        {
            Debug.LogError("Animator not found on child object!");
        }
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on child object!");
        }

        lastScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
    }

    private void HandleWalkingAnimation()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        float verticalMovement = screenPosition.y - lastScreenPosition.y;
        float horizontalMovement = screenPosition.x - lastScreenPosition.x;

        // Only update vertical animation direction when actual movement happens
        if (Mathf.Abs(verticalMovement) > 0.01f)
        {
            bool isMovingUp = verticalMovement > 0;
            if (isMovingUp != lastMovingUp) // Update only when direction changes
            {
                animator.SetBool("isMovingUp", isMovingUp);
                lastMovingUp = isMovingUp; // Store last movement direction
            }
        }

        // Flip the sprite based on horizontal movement
        if (Mathf.Abs(horizontalMovement) > 0.01f)
        {
            spriteRenderer.flipX = horizontalMovement < 0;
        }

        lastScreenPosition = screenPosition; // Store last position for next frame
    }
}
