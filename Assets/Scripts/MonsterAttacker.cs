using UnityEngine;

public class MonsterAttacker : MonoBehaviour
{
    [Header("Monster Attack Settings")]
    public float attackInterval = 2f; 
    public float attackDamage = 25f;  
    private float nextAttackTime = 0f;
    private float attackAnimationDuration = 0.5f; 

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
            // Agar ek bhi bot nahi bacha, toh monster attack rok kar chup-chaap Idle khada ho jaye
            if (monsterController != null)
            {
                monsterController.TriggerIdleAnimation();
            }
            return; // Yahin se update roko taake attack timer aage na chale
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

        GameObject randomBot = activeBots[Random.Range(0, activeBots.Length)];

        BotHealth botHealth = randomBot.GetComponent<BotHealth>();
        if (botHealth != null)
        {
            botHealth.TakeDamage(attackDamage);
            Debug.Log("Monster attacked a bot for " + attackDamage + " damage!");
        }
    }

    void ResetToIdle()
    {
        // Sirf tab idle karein agar game abhi bhi fighting state mein hai
        if (monsterController != null && gameManager != null && gameManager.currentState == GameplayHandler.GameState.Fighting)
        {
            monsterController.TriggerIdleAnimation();
        }
    }
}