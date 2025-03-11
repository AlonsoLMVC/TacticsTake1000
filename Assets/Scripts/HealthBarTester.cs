using UnityEngine;

public class HealthBarTester : MonoBehaviour
{
    public HealthBar healthBar; // Assign in Inspector
    private float maxHealth = 100f;
    private float currentHealth;
    private float potentialDamage;

    private void Start()
    {
        currentHealth = maxHealth;
        potentialDamage = 0;
        healthBar.Initialize();
        InvokeRepeating(nameof(ApplyPotentialDamage), 1f, 3f); // Show potential damage every 3s
        InvokeRepeating(nameof(ApplyDamage), 2f, 3f); // Apply actual damage 1s later
    }

    private void ApplyPotentialDamage()
    {
        potentialDamage = Random.Range(10f, 30f);
        healthBar.UpdateHealth(currentHealth, potentialDamage, maxHealth);
        Debug.Log($"⚠️ Potential Damage Incoming: {potentialDamage}");
    }

    private void ApplyDamage()
    {

        healthBar.FinalizeHealthTransition(); // Now actually animate the red bar

        Debug.Log($"💥 Took {potentialDamage} damage! Current Health: {currentHealth}");

        // If health is depleted, heal after 3 seconds
        if (currentHealth <= 0)
        {
            Debug.Log("⚠️ Health Depleted! Healing in 3 seconds...");
            Invoke(nameof(Heal), 3f);
        }
    }

    private void Heal()
    {
        currentHealth = maxHealth;
        healthBar.UpdateHealth(currentHealth, 0, maxHealth);
        Debug.Log("✨ Fully healed!");
    }
}
