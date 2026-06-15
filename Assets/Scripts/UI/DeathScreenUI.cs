using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI detailText;

    private bool layoutReady;

    void Awake()
    {
        EnsureLayout();
    }

    void OnEnable()
    {
        EnsureLayout();
        RefreshDeathInfo();
        GameEvents.OnPlayerDestroyed += RefreshDeathInfo;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerDestroyed -= RefreshDeathInfo;
    }

    public void WireTexts(TextMeshProUGUI title, TextMeshProUGUI detail)
    {
        titleText = title;
        detailText = detail;
        RefreshDeathInfo();
    }

    public void EnsureLayout()
    {
        if (layoutReady && titleText != null && detailText != null)
            return;

        foreach (Transform child in transform)
        {
            if (child.name == "Text (TMP)")
                child.gameObject.SetActive(false);
        }

        titleText = FindTmp("Title") ?? CreateText("Title", new Vector2(0f, 140f), new Vector2(760f, 90f), 44f,
            TextAlignmentOptions.Center, FontStyles.Bold);

        detailText = FindTmp("Detail") ?? CreateText("Detail", new Vector2(0f, -10f), new Vector2(760f, 240f), 26f,
            TextAlignmentOptions.Center);

        Transform button = transform.Find("Button");
        if (button != null)
        {
            var rt = button.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, -190f);
                rt.sizeDelta = new Vector2(240f, 52f);
                rt.localScale = Vector3.one;
            }
        }

        layoutReady = true;
        RefreshDeathInfo();
    }

    private void RefreshDeathInfo()
    {
        if (!layoutReady)
            EnsureLayout();

        float credits = EconomyManager.Instance != null ? EconomyManager.Instance.Credits : 0f;
        float repairCost = credits * 0.3f;

        string title = LocalizationManager.Instance != null
            ? LocalizationManager.Instance.GetString("DEATH_TITLE")
            : "Zniszczenie Statku";

        string repairLabel = LocalizationManager.Instance != null
            ? LocalizationManager.Instance.GetString("DEATH_REPAIR")
            : "Koszt naprawy kadłuba: ";

        string cargoLost = LocalizationManager.Instance != null
            ? LocalizationManager.Instance.GetString("DEATH_CARGO_LOST")
            : "Utracono ~20% ładunku";

        string respawn = LocalizationManager.Instance != null
            ? LocalizationManager.Instance.GetString("DEATH_RESPAWN")
            : "Kliknij przycisk, aby wrócić do stacji";

        if (titleText != null)
            titleText.text = title;

        if (detailText != null)
        {
            detailText.text =
                "Wystrzelono z kokpitu ratunkowego.\n\n" +
                $"{repairLabel}{repairCost:F0} cr (30% kredytów)\n" +
                $"{cargoLost}\n\n{respawn}";
        }
    }

    public void OnRestartButtonClicked()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.RespawnAtBase();
    }

    private TextMeshProUGUI FindTmp(string name)
    {
        foreach (TextMeshProUGUI tmp in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (tmp.name == name)
                return tmp;
        }

        return null;
    }

    private TextMeshProUGUI CreateText(string name, Vector2 pos, Vector2 size, float fontSize,
        TextAlignmentOptions align, FontStyles style = FontStyles.Normal)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(transform, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.fontStyle = style;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        tmp.enableWordWrapping = true;
        return tmp;
    }
}
