using UnityEngine;

public class CombatPromptSystem : MonoBehaviour
{
    public static CombatPromptSystem Instance { get; private set; }

    [SerializeField] private bool enemyDetected;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        EventBus.OnPlayerDetected += OnPlayerDetected;
        EventBus.OnEnemyDeath += OnEnemyDeath;
    }

    void OnDisable()
    {
        EventBus.OnPlayerDetected -= OnPlayerDetected;
        EventBus.OnEnemyDeath -= OnEnemyDeath;
    }

    private void OnPlayerDetected(Transform player)
    {
        if (enemyDetected) return;
        enemyDetected = true;

        Transform enemy = FindNearestEnemy(player != null ? player.position : Vector3.zero);
        Debug.Log("[Combat] Wróg — walcz/uciekaj");
        GameEvents.TriggerCombatPromptShown(enemy != null ? enemy : player);
        GameEvents.TriggerCombatStarted();
    }

    private void OnEnemyDeath(EnemyAI _)
    {
        if (!enemyDetected) return;
        enemyDetected = false;
        GameEvents.TriggerCombatEnded();
    }

    public void AnswerCombatPrompt(bool fight)
    {
        GameEvents.TriggerCombatPromptAnswered(fight);
        if (!fight)
        {
            enemyDetected = false;
            GameEvents.TriggerCombatEnded();
        }
    }

    private static Transform FindNearestEnemy(Vector3 from)
    {
        EnemyAI[] enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        Transform best = null;
        float bestDist = float.MaxValue;

        foreach (EnemyAI enemy in enemies)
        {
            if (enemy == null) continue;
            float d = Vector3.Distance(from, enemy.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = enemy.transform;
            }
        }

        return best;
    }
}
