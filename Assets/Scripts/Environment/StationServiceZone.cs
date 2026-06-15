using UnityEngine;

public class StationServiceZone : MonoBehaviour
{
    public enum ServiceType { Shop, Missions, Repair }

    [SerializeField] private ServiceType serviceType = ServiceType.Shop;
    [SerializeField] private float fallbackRadius = 12f;

    public void Configure(ServiceType type) => serviceType = type;

    void Awake()
    {
        EnsureTriggerCollider();
    }

    private void EnsureTriggerCollider()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            box.isTrigger = true;
            return;
        }

        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere != null)
        {
            sphere.isTrigger = true;
            return;
        }

        sphere = gameObject.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        sphere.radius = fallbackRadius;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StationProximity.SetActive(serviceType, true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StationProximity.SetActive(serviceType, false);
    }
}

public static class StationProximity
{
    public static bool ShopOpen { get; private set; }
    public static bool MissionsOpen { get; private set; }
    public static bool RepairOpen { get; private set; }

    public static bool RequiresCursor => ShopOpen || MissionsOpen || RepairOpen;

    public static void SetActive(StationServiceZone.ServiceType type, bool active)
    {
        switch (type)
        {
            case StationServiceZone.ServiceType.Shop:
                ShopOpen = active;
                break;
            case StationServiceZone.ServiceType.Missions:
                MissionsOpen = active;
                break;
            case StationServiceZone.ServiceType.Repair:
                RepairOpen = active;
                MissionsOpen = active;
                break;
        }

        ApplyCursor();
    }

    public static void ApplyCursor()
    {
        if (GameManager.Instance == null) return;
        if (InventoryToggle.IsOpen || MapToggle.IsOpen) return;

        GameState state = GameManager.Instance.currentState;
        if (state == GameState.GameOver || state == GameState.Menu || state == GameState.Console)
            return;

        if (RequiresCursor && state == GameState.Exploration)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (state == GameState.Exploration)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
