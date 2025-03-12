using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailsPanelUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI jobText;
    [SerializeField] private Image jobIconImage;
    [SerializeField] private TextMeshProUGUI currentHealthText;
    [SerializeField] private TextMeshProUGUI maxHealthText;
    [SerializeField] private HealthBar healthBar;

    /// <summary>
    /// Updates the details panel UI with unit information.
    /// </summary>
    public void UpdateDetails(string unitName, string job, Sprite jobIcon, float currentHealth, float maxHealth)
    {
        if (nameText != null)
            nameText.text = unitName;

        if (jobText != null)
            jobText.text = job;

        if (jobIconImage != null)
            jobIconImage.sprite = jobIcon;

        if (currentHealthText != null)
            currentHealthText.text = $"{Mathf.CeilToInt(currentHealth)}";

        if (maxHealthText != null)
            maxHealthText.text = $"{Mathf.CeilToInt(maxHealth)}";

        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth, 0, maxHealth); // No predicted damage
    }

    /// <summary>
    /// Updates the health bar to reflect predicted damage.
    /// </summary>
    public void UpdateHealthWithDamage(float currentHealth, float incomingDamage, float maxHealth)
    {
        if (currentHealthText != null)
            currentHealthText.text = $"{Mathf.CeilToInt(Mathf.Max(0, currentHealth - incomingDamage))}";

        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth, incomingDamage, maxHealth);
    }

    /// <summary>
    /// Finalizes health update after damage is confirmed.
    /// </summary>
    public void ApplyFinalHealth()
    {
        if (healthBar != null)
            healthBar.FinalizeHealthTransition();
    }
}
