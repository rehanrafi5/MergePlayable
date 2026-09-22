using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarUI; 
    public GameObject destructionEffect; 

    [Header("Animator Settings")]
    public Animator monsterAnimator; // Yahan monster ka animator component aayega

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Agar animator assign nahi hai toh khud dhoond lega
        if (monsterAnimator == null)
        {
            monsterAnimator = GetComponent<Animator>();
        }

        // Game shuru hote hi Idle animation chala do
        SetAnimationState(true, false, false);
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

    // Jab monster attack karega tab yeh function call hoga
    public void TriggerAttackAnimation()
    {
        if (currentHealth > 0)
        {
            SetAnimationState(false, true, false);
        }
    }

    // Jab monster idle ho jaye
    public void TriggerIdleAnimation()
    {
        if (currentHealth > 0)
        {
            SetAnimationState(true, false, false);
        }
    }

    void UpdateHealthUI()
    {
        if (healthBarUI != null)
        {
            healthBarUI.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Monster Defeated! Level Clear!");

        // Death animation trigger karo
        SetAnimationState(false, false, true);
        
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        // Thori der delay ke baad object disable ya destroy kar sakte hain taake death animation poori play ho sake
        gameObject.SetActive(false);
    }

    // Helper function animator ke bool parameters control karne ke liye
    void SetAnimationState(bool idle, bool attack, bool death)
    {
        if (monsterAnimator != null)
        {
            monsterAnimator.SetBool("Idle", idle);
            monsterAnimator.SetBool("Attack", attack);
            monsterAnimator.SetBool("Death", death);
        }
    }
}