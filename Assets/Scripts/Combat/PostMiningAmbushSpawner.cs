using UnityEngine;

public class PostMiningAmbushSpawner : MonoBehaviour
{
    [SerializeField] private float ambushChance = 0.1f;

    void OnEnable() => GameEvents.OnMiningComplete += TryAmbush;
    void OnDisable() => GameEvents.OnMiningComplete -= TryAmbush;

    private void TryAmbush()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Exploration)
            return;

        CustomSectorSpawner spawner = FindFirstObjectByType<CustomSectorSpawner>();
        if (spawner == null) return;

        if (spawner.TrySpawnAmbushAtOppositeEdge(ambushChance))
        {
            Debug.Log("[Ambush] Wróg zrespiowany na przeciwnym końcu sektora po wydobyciu.");
            GameManager.Instance?.ChangeState(GameState.Fighting);
        }
    }
}
