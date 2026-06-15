using UnityEngine;

public class CustomSectorSpawner : MonoBehaviour
{
    public AIArchetype[] availableArchetypes;
    public GameObject enemyPrefab;

    [Header("Pojedynczy wróg — tylko ambush po wydobyciu")]
    public int maxActiveEnemies = 1;
    public float minSpawnDistanceFromPlayer = 80f;

    private GameObject ResolveEnemyPrefab()
    {
        if (enemyPrefab != null) return enemyPrefab;
        return Resources.Load<GameObject>("AI/EnemyWroga");
    }

    private int CountActiveEnemies()
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.activeEnemies.Count;

        return FindObjectsByType<EnemyAI>(FindObjectsSortMode.None).Length;
    }

    /// <summary>10% szans po zebraniu surowca — spawn na przeciwnym końcu sektora.</summary>
    public bool TrySpawnAmbushAtOppositeEdge(float chance = 0.1f)
    {
        if (Random.value > chance) return false;
        if (CountActiveEnemies() >= maxActiveEnemies) return false;

        GameObject prefab = ResolveEnemyPrefab();
        if (prefab == null) return false;

        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        Vector3 playerPos = player != null ? player.position : Vector3.zero;

        Vector3 spawnPosition = GetOppositeEdgePosition(playerPos);
        SpawnEnemyAt(prefab, spawnPosition, playerPos, availableArchetypes);
        return true;
    }

    public static GameObject SpawnEnemyAt(GameObject prefab, Vector3 spawnPosition, Vector3 lookTarget, AIArchetype[] archetypes)
    {
        if (prefab == null) return null;

        GameObject newEnemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
        newEnemy.AddComponent<AISpawnedMarker>();

        Vector3 lookDir = lookTarget - spawnPosition;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.01f)
            newEnemy.transform.rotation = Quaternion.LookRotation(lookDir.normalized);

        AIArchetype chosen = archetypes != null && archetypes.Length > 0
            ? archetypes[Random.Range(0, archetypes.Length)]
            : null;

        EnemyAI aiComp = newEnemy.GetComponent<EnemyAI>();
        if (aiComp != null)
        {
            aiComp.ApplyArchetype(chosen);
            aiComp.AssignPatrolWaypoints();
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterEnemy(aiComp);
        }

        return newEnemy;
    }

    public static Vector3 GetOppositeEdgePosition(Vector3 playerPos)
    {
        if (ChunkManager.Instance == null)
            return playerPos + Vector3.forward * 150f;

        Vector3 center = ChunkManager.Instance.GetSectorWorldCenter();
        float half = ChunkManager.Instance.GetSectorHalfExtent() * 0.88f;

        Vector3 toPlayer = playerPos - center;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 1f)
            toPlayer = Vector3.forward;

        Vector3 spawn = center - toPlayer.normalized * half;
        spawn += new Vector3(Random.Range(-25f, 25f), 0f, Random.Range(-25f, 25f));
        spawn.y = playerPos.y;
        return ChunkManager.Instance.ClampToSector(spawn, null, 12f);
    }
}
