using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f; // Bullet ki raftaar
    private Transform target;
    private float damage;

    public void Initialize(Transform targetTransform, float damageAmount)
    {
        target = targetTransform;
        damage = damageAmount;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Target ki taraf move karo (thoda center point ki taraf)
        Vector3 targetPos = target.position + new Vector3(0, 1f, 0);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Jab bullet target ke kareeb pohch jaye
        if (Vector3.Distance(transform.position, targetPos) < 0.5f)
        {
            // 1. Agar saamne Monster hai, toh usko damage do (Bot ki goli)
            MonsterController monster = target.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
            }

            // 2. Agar saamne Bot hai, toh usko damage do (Monster ki goli)
            BotHealth bot = target.GetComponent<BotHealth>();
            if (bot != null)
            {
                bot.TakeDamage(damage);
            }

            // Hit hone ke baad bullet gayab!
            Destroy(gameObject);
        }
    }
}