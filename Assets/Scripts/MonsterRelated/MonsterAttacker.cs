using UnityEngine;

public class MonsterAttacker : MonoBehaviour
{
    [Header("Monster Attack Settings")]
    public float attackInterval = 2f; 
    public float attackDamage = 25f;  
    private float nextAttackTime = 0f;
    private float attackAnimationDuration = 0.5f; 

    [Header("Projectile Settings")]
    public GameObject bulletPrefab; // Monster ka fireball/bullet prefab
    public Transform firePoint;     // Kahan se fireball niklega (Jaise uske muh ya hath se)

    private GameplayHandler gameManager;
    private MonsterController monsterController;

    void Start()
    {
        gameManager = FindObjectOfType<GameplayHandler>();
        monsterController = GetComponent<MonsterController>();
    }

    void Update()
    {
        // 1. Agar game Fighting state mein nahi hai, toh attack bilkul band
        if (gameManager == null || gameManager.currentState != GameplayHandler.GameState.Fighting)
        {
            if (monsterController != null)
            {
                monsterController.TriggerIdleAnimation();
            }
            return;
        }

        // 2. Scene mein check karo ke kya koi bot zinda bhi hai ya nahi
        GameObject[] activeBots = GameObject.FindGameObjectsWithTag("Bot");
        if (activeBots.Length == 0)
        {
            if (monsterController != null)
            {
                monsterController.TriggerIdleAnimation();
            }
            return; 
        }

        // 3. Normal fighting logic agar bots mojood hain
        if (Time.time >= nextAttackTime)
        {
            AttackRandomBot(activeBots);
            nextAttackTime = Time.time + attackInterval;
        }
    }

    void AttackRandomBot(GameObject[] activeBots)
    {
        if (monsterController != null)
        {
            monsterController.TriggerAttackAnimation();
            Invoke("ResetToIdle", attackAnimationDuration);
        }

        // Random bot select karo
        GameObject randomBot = activeBots[Random.Range(0, activeBots.Length)];

        // Bullet / Fireball Spawn karo
        if (bulletPrefab != null)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            Projectile projectileScript = bulletObj.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                // Target (bot) aur damage pass kar do
                projectileScript.Initialize(randomBot.transform, attackDamage);
            }
        }
        else
        {
            // Fallback: Agar prefab na ho toh direct instant damage de do (purana tareeqa)
            BotHealth botHealth = randomBot.GetComponent<BotHealth>();
            if (botHealth != null)
            {
                botHealth.TakeDamage(attackDamage);
            }
        }
    }

    void ResetToIdle()
    {
        if (monsterController != null && gameManager != null && gameManager.currentState == GameplayHandler.GameState.Fighting)
        {
            monsterController.TriggerIdleAnimation();
        }
    }
}