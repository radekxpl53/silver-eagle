using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [SerializeField] private float credits;
    [SerializeField] private float debt;

    public float Credits => credits;
    public float Debt => debt;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetCredits(float amount)
    {
        credits = Mathf.Max(0f, amount);
        GameEvents.TriggerCreditsChanged(credits);
    }

    public void AddCredits(float amount)
    {
        if (amount <= 0f) return;
        credits += amount;
        GameEvents.TriggerCreditsChanged(credits);
    }

    public bool SpendCredits(float amount)
    {
        if (amount <= 0f) return true;
        if (credits < amount) return false;
        credits -= amount;
        GameEvents.TriggerCreditsChanged(credits);
        return true;
    }

    public void AddDebt(float amount)
    {
        if (amount <= 0f) return;
        debt += amount;
        GameEvents.TriggerDebtChanged(debt);
    }

    public bool PayDebt(float amount)
    {
        if (amount <= 0f) return true;
        if (debt < amount || credits < amount) return false;
        credits -= amount;
        debt -= amount;
        GameEvents.TriggerCreditsChanged(credits);
        GameEvents.TriggerDebtChanged(debt);
        return true;
    }

    public void SetDebt(float amount)
    {
        debt = Mathf.Max(0f, amount);
        GameEvents.TriggerDebtChanged(debt);
    }
}
