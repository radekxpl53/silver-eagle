using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<Vector2Int, SectorDefinition> OnSectorEntered;
    public static event Action<float> OnCreditsChanged;
    public static event Action<float> OnDebtChanged;
    public static event Action<string> OnUpgradePurchased;
    public static event Action OnCombatStarted;
    public static event Action OnCombatEnded;
    public static event Action<float, Vector3> OnHullDamaged;
    public static event Action OnPlayerDestroyed;
    public static event Action<EnemyAI> OnEnemyKilled;

    public static void TriggerSectorEntered(Vector2Int grid, SectorDefinition def) =>
        OnSectorEntered?.Invoke(grid, def);

    public static void TriggerCreditsChanged(float credits) =>
        OnCreditsChanged?.Invoke(credits);

    public static void TriggerDebtChanged(float debt) =>
        OnDebtChanged?.Invoke(debt);

    public static void TriggerUpgradePurchased(string upgradeId) =>
        OnUpgradePurchased?.Invoke(upgradeId);

    public static void TriggerCombatStarted() =>
        OnCombatStarted?.Invoke();

    public static void TriggerCombatEnded() =>
        OnCombatEnded?.Invoke();

    public static void TriggerHullDamaged(float damage, Vector3 hitPoint) =>
        OnHullDamaged?.Invoke(damage, hitPoint);

    public static void TriggerPlayerDestroyed() =>
        OnPlayerDestroyed?.Invoke();

    public static void TriggerEnemyKilled(EnemyAI enemy) =>
        OnEnemyKilled?.Invoke(enemy);
}
