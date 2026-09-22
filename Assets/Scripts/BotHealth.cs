using UnityEngine;
using UnityEngine.UI; // UI Image ke liye zaroori hai

public class BotHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 50f;
    public float currentHealth;

    [Header("UI & Effects")]
    public Image healthBarUI;         // Bot ke canvas ka fill amount wala image
    public GameObject deathEffect;    // Explosion ya poof particle effect prefab

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthBarUI != null)
        {
            // Health bar fill amount update karega (0 se 1 ke darmiyan)
            healthBarUI.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Bot destroyed by monster!");

        // Agar death particle effect lagaya hua hai toh usko spawn karo
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Bot ko scene se destroy kar do
        Destroy(gameObject);
    }
}