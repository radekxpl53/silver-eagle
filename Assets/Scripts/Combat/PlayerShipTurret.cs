using UnityEngine;

/// <summary>
/// Turret gracza — podłącz na Statek.prefab (bez kokpitu UI).
/// Ustaw tag Player na rodzicu i przypisz projectilePrefab + firePoint.
/// </summary>
[RequireComponent(typeof(Turret))]
public class PlayerShipTurret : MonoBehaviour
{
    [SerializeField] private Turret turret;
    [SerializeField] private KeyCode manualFireKey = KeyCode.None;

    void Awake()
    {
        if (turret == null) turret = GetComponent<Turret>();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.currentState != GameState.Exploration &&
            GameManager.Instance.currentState != GameState.Fighting)
            return;

        if (manualFireKey != KeyCode.None && Input.GetKeyDown(manualFireKey) && turret != null)
            turret.Fire();
    }

    public void SetTarget(Transform enemy)
    {
        if (turret != null) turret.target = enemy;
    }
}
