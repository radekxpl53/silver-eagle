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
        Debug.Log("[Combat] Wróg — walcz/uciekaj");
        GameEvents.TriggerCombatPromptShown(player);
        GameEvents.TriggerCombatStarted();
    }

    private void OnEnemyDeath(EnemyAI _)
    {
        if (!enemyDetected) return;
        enemyDetected = false;
        GameEvents.TriggerCombatEnded();
    }

    /// <summary>Stub dla UI (Prompt B): fight=true pozostaje w walce, false kończy prompt.</summary>
    public void AnswerCombatPrompt(bool fight)
    {
        GameEvents.TriggerCombatPromptAnswered(fight);
        if (!fight)
        {
            enemyDetected = false;
            GameEvents.TriggerCombatEnded();
        }
    }
}
