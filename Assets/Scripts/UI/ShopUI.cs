using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Transform listRoot;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private TextMeshProUGUI statusText;

    private UpgradeDefinition[] catalog;

    void OnEnable()
    {
        Refresh();
        GameEvents.OnCreditsChanged += OnCreditsChanged;
        GameEvents.OnUpgradePurchased += OnUpgradePurchased;
    }

    void OnDisable()
    {
        GameEvents.OnCreditsChanged -= OnCreditsChanged;
        GameEvents.OnUpgradePurchased -= OnUpgradePurchased;
    }

    private void OnCreditsChanged(float _) => Refresh();
    private void OnUpgradePurchased(string _) => Refresh();

    public void Refresh()
    {
        if (listRoot == null) return;

        foreach (Transform child in listRoot)
            Destroy(child.gameObject);

        catalog = Resources.LoadAll<UpgradeDefinition>("Upgrades");
        if (catalog == null || catalog.Length == 0)
        {
            SetStatus("Brak oferty w sklepie.");
            return;
        }

        foreach (var upgrade in catalog)
        {
            if (upgrade == null) continue;
            CreateRow(upgrade);
        }
    }

    private void CreateRow(UpgradeDefinition upgrade)
    {
        GameObject row = rowPrefab != null
            ? Instantiate(rowPrefab, listRoot)
            : new GameObject(upgrade.displayName, typeof(RectTransform));

        if (row.transform.parent != listRoot)
            row.transform.SetParent(listRoot, false);

        row.SetActive(true);

        var label = row.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            bool owned = ShopSystem.Instance != null && ShopSystem.Instance.IsAlreadyOwned(upgrade);
            bool afford = ShopSystem.Instance != null && ShopSystem.Instance.CanAfford(upgrade);
            string state = owned ? "[KUPIONE]" : afford ? "" : "[ZA DROGO]";
            label.text = $"{upgrade.displayName} — {upgrade.creditCost:F0} cr {state}\n{upgrade.description}";
        }

        var rowImage = row.GetComponent<Image>();
        if (rowImage != null)
        {
            rowImage.raycastTarget = true;
            bool owned = ShopSystem.Instance != null && ShopSystem.Instance.IsAlreadyOwned(upgrade);
            bool afford = ShopSystem.Instance != null && ShopSystem.Instance.CanAfford(upgrade);
            if (owned) rowImage.color = new Color(0.1f, 0.16f, 0.1f, 0.92f);
            else if (!afford) rowImage.color = new Color(0.22f, 0.12f, 0.12f, 0.92f);
            else rowImage.color = new Color(0.15f, 0.2f, 0.3f, 0.92f);
        }

        UpgradeDefinition capturedUpgrade = upgrade;
        var rowClick = row.GetComponent<StationRowClick>();
        if (rowClick == null) rowClick = row.AddComponent<StationRowClick>();
        rowClick.Bind(() => TryBuy(capturedUpgrade));
    }

    private void TryBuy(UpgradeDefinition upgrade)
    {
        if (ShopSystem.Instance == null) return;

        if (ShopSystem.Instance.IsAlreadyOwned(upgrade))
        {
            SetStatus("Już posiadasz ten upgrade.");
            return;
        }

        if (!ShopSystem.Instance.CanAfford(upgrade))
        {
            SetStatus("Za mało kredytów.");
            return;
        }

        bool ok = ShopSystem.Instance.TryPurchase(upgrade);
        SetStatus(ok ? $"Kupiono: {upgrade.displayName}" : $"Nie udało się kupić: {upgrade.displayName}");
        Refresh();
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }
}
