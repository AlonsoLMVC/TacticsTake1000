using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    public Image healthImage;  // Green bar: Current health
    public Image oldHealthImage;  // Red bar: Delayed damage preview

    private Coroutine transitionCoroutine;

    public void Initialize()
    {
        healthImage.fillAmount = 1f;
        oldHealthImage.fillAmount = 1f;
    }

    /// <summary>
    /// Updates the health bar visuals to reflect predicted damage.
    /// Does NOT animate the damage bar.
    /// </summary>
    public void UpdateHealth(float newHealth, float potentialDamage, float maxHealth)
    {
        newHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        float healthPercent = newHealth / maxHealth;
        float damagePreviewPercent = Mathf.Clamp((newHealth - potentialDamage) / maxHealth, 0f, 1f);

        // Show immediate health change
        healthImage.fillAmount = damagePreviewPercent;

        // Show potential damage preview instantly (red bar)
        oldHealthImage.fillAmount = healthPercent;
    }

    /// <summary>
    /// Executes the damage transition by animating the red bar to match the green bar.
    /// </summary>
    
    /// <summary>
    /// Smoothly transitions the green health bar to match the red bar over time.
    /// </summary>
    public void FinalizeHealthTransition()
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(SmoothTransitionToFinalHealth());
    }

    private IEnumerator SmoothTransitionToFinalHealth()
    {
        //yield return new WaitForSeconds(0.5f); // Optional delay before animation starts

        float elapsedTime = 0f;
        float transitionDuration = 0.4f; // Time taken for transition
        float initialFill = oldHealthImage.fillAmount;
        float targetFill = healthImage.fillAmount; // Match green bar

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration; // Normalize time between 0 and 1
            Debug.Log(initialFill + "   " + targetFill);
            oldHealthImage.fillAmount = Mathf.Lerp(initialFill, targetFill, t);
            yield return null;
        }

        oldHealthImage.fillAmount = targetFill; // Ensure it reaches the exact target
        transitionCoroutine = null; // Reset coroutine tracker
    }
}
