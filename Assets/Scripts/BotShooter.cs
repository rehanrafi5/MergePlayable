using UnityEngine;

public class BotShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public float baseFireRate = 1f; // Har 1 second mein shoot
    public float baseDamage = 10f;  // Level 1 ka base damage
    private float nextFireTime = 0f;
    private float attackAnimationDuration = 0.3f; // Attack animation kitni dair chalegi

    [Header("References")]
    public MonsterController targetMonster;
    private BotIdentity botIdentity;
    private GameplayHandler gameManager; 
    private Animator botAnimator; // Bot ka animator component

    void Start()
    {
        botIdentity = GetComponent<BotIdentity>();
        gameManager = FindObjectOfType<GameplayHandler>();
        botAnimator = GetComponent<Animator>();

        if (targetMonster == null)
        {
            targetMonster = FindObjectOfType<MonsterController>();
        }

        // Shuru mein Idle animation chala do
        SetAnimationState(true, false);
    }

    void Update()
    {
        // Jab game 'Fighting' state mein ho tabhi bots shoot karenge
        if (gameManager != null && gameManager.currentState == GameplayHandler.GameState.Fighting)
        {
            if (targetMonster != null && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / baseFireRate;
            }
            else if (targetMonster == null)
            {
                targetMonster = FindObjectOfType<MonsterController>();
            }
        }
        else
        {
            // Fighting state na ho toh Idle rakho
            SetAnimationState(true, false);
        }
    }

    void Shoot()
    {
        float finalDamage = baseDamage;

        if (botIdentity != null)
        {
            finalDamage = baseDamage * botIdentity.level; 
        }

        if (targetMonster != null)
        {
            targetMonster.TakeDamage(finalDamage);

            // Attack animation trigger karo
            SetAnimationState(false, true);

            // Thori der baad wapas Idle par le aao
            Invoke("ResetToIdle", attackAnimationDuration);
        }
    }

    void ResetToIdle()
    {
        SetAnimationState(true, false);
    }

    // Helper function animator ke bool parameters control karne ke liye
    void SetAnimationState(bool idle, bool attack)
    {
        if (botAnimator != null)
        {
            botAnimator.SetBool("Idle", idle);
            botAnimator.SetBool("Attack", attack);
        }
    }
}