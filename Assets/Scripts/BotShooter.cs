using UnityEngine;

public class BotShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public float baseFireRate = 1f; 
    public float baseDamage = 10f;  
    private float nextFireTime = 0f;
    private float attackAnimationDuration = 0.3f; 

    [Header("References & Effects")]
    public MonsterController targetMonster;
    public GameObject bulletPrefab; // Yahan apni Bullet ya Fireball ka prefab lagana hai
    public Transform firePoint;     // Jahan se bullet niklegi (agar nahi di toh bot ke center se niklegi)
    
    private BotIdentity botIdentity;
    private GameplayHandler gameManager; 
    private Animator botAnimator; 

    void Start()
    {
        botIdentity = GetComponent<BotIdentity>();
        gameManager = FindObjectOfType<GameplayHandler>();
        botAnimator = GetComponent<Animator>();

        if (targetMonster == null)
        {
            targetMonster = FindObjectOfType<MonsterController>();
        }

        SetAnimationState(true, false);
    }

    void Update()
    {
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

        // FIX: Yahan se ghalat 'long.MinValue' wala check hata diya hai
        if (targetMonster != null)
        {
            // Attack animation trigger karo
            SetAnimationState(false, true);
            Invoke("ResetToIdle", attackAnimationDuration);

            // Bullet / Fireball Spawn karo
            if (bulletPrefab != null)
            {
                Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
                GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

                Projectile projectileScript = bulletObj.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.Initialize(targetMonster.transform, finalDamage);
                }
            }
            else
            {
                // Fallback: Agar prefab na ho toh direct damage lag jaye
                targetMonster.TakeDamage(finalDamage);
            }
        }
    }

    void ResetToIdle()
    {
        SetAnimationState(true, false);
    }

    void SetAnimationState(bool idle, bool attack)
    {
        if (botAnimator != null)
        {
            botAnimator.SetBool("Idle", idle);
            botAnimator.SetBool("Attack", attack);
        }
    }
}