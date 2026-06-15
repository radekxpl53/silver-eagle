using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private Transform listRoot;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private TextMeshProUGUI statusText;

    void OnEnable()
    {
        Refresh();
        SetStatus("Kliknij misję, aby ją przyjąć. Potem oddaj surowce w warsztacie.");
    }

    public void Refresh()
    {
        if (listRoot == null || FactionMissionSystem.Instance == null) return;

        foreach (Transform child in listRoot)
            Destroy(child.gameObject);

        foreach (var mission in FactionMissionSystem.Instance.GetActiveMissions())
            CreateRow(mission);

        if (listRoot.childCount == 0)
            SetStatus("Brak aktywnych misji.");
    }

    private void CreateRow(FactionMissionDefinition mission)
    {
        GameObject row = rowPrefab != null
            ? Instantiate(rowPrefab, listRoot)
            : new GameObject(mission.displayName, typeof(RectTransform), typeof(Image), typeof(StationRowClick));

        if (row.transform.parent != listRoot)
            row.transform.SetParent(listRoot, false);

        row.SetActive(true);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerInventory inv = player != null ? player.GetComponent<PlayerInventory>() : null;
        FactionMissionSystem sys = FactionMissionSystem.Instance;

        string resName = mission.targetResource != null ? mission.targetResource.Name : "?";
        bool accepted = sys.IsAccepted(mission);
        int owned = sys.GetOwnedAmount(mission, inv);
        string action = !accepted
            ? "[Kliknij: PRZYJMIJ]"
            : owned >= mission.requiredAmount
                ? "[Kliknij: ODDAJ]"
                : $"[Zebrane: {owned}/{mission.requiredAmount}]";

        var label = row.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = $"{mission.displayName} {action}\n" +
                         $"Dostarcz: {mission.requiredAmount}x {resName} → {mission.creditReward:F0} cr\n" +
                         mission.description;
        }

        var rowImage = row.GetComponent<Image>();
        if (rowImage != null)
        {
            rowImage.raycastTarget = true;
            if (!accepted) rowImage.color = new Color(0.12f, 0.18f, 0.28f, 0.92f);
            else if (owned >= mission.requiredAmount) rowImage.color = new Color(0.12f, 0.28f, 0.14f, 0.92f);
            else rowImage.color = new Color(0.22f, 0.18f, 0.1f, 0.92f);
        }

        FactionMissionDefinition captured = mission;
        var rowClick = row.GetComponent<StationRowClick>();
        if (rowClick == null) rowClick = row.AddComponent<StationRowClick>();
        rowClick.Bind(() => OnMissionClicked(captured));

        var button = row.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnMissionClicked(captured));
        }
    }

    private void OnMissionClicked(FactionMissionDefinition mission)
    {
        if (mission == null || FactionMissionSystem.Instance == null)
        {
            SetStatus("Błąd: system misji niedostępny.");
            return;
        }

        if (!FactionMissionSystem.Instance.IsAccepted(mission))
        {
            if (FactionMissionSystem.Instance.TryAcceptMission(mission))
                SetStatus($"Przyjęto misję: {mission.displayName}");
            else
                SetStatus("Nie udało się przyjąć misji.");
            Refresh();
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            SetStatus("Nie znaleziono statku gracza.");
            return;
        }

        PlayerInventory inv = player.GetComponent<PlayerInventory>();
        if (inv == null)
        {
            SetStatus("Brak ekwipunku na statku.");
            return;
        }

        bool ok = FactionMissionSystem.Instance.TryCompleteMission(mission, inv);
        if (ok)
            SetStatus($"Ukończono: {mission.displayName} (+{mission.creditReward:F0} cr)");
        else
            SetStatus($"Brak surowców: {mission.targetResource?.Name ?? "?"} ({FactionMissionSystem.Instance.GetOwnedAmount(mission, inv)}/{mission.requiredAmount})");

        Refresh();
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }
}
