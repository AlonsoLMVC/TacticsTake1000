using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingPanelUIManager : MonoBehaviour
{
    [SerializeField] private Image jobIconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextMeshProUGUI currentHealthText;
    [SerializeField] private TextMeshProUGUI finalHealthText;
    [SerializeField] private GameObject triangle;
    /// <summary>
    /// Updates the floating panel UI with unit information.
    /// </summary>
    public void UpdateFloatingPanel(string unitName, int level, Sprite jobIcon, float currentHealth, float maxHealth)
    {
        if (nameText != null)
            nameText.text = unitName;

        if (levelText != null)
            levelText.text = "Lv. " + level;

        if (jobIconImage != null)
            jobIconImage.sprite = jobIcon;

        if (currentHealthText != null)
            currentHealthText.text = $"{Mathf.CeilToInt(currentHealth)}";

        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth, 0, maxHealth); // No predicted damage

        triangle.SetActive(false);
        finalHealthText.gameObject.SetActive(false);
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
