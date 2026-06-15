using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatHUD : MonoBehaviour
{
    private const float BarWidth = 320f;
    private const float BarHeight = 28f;

    private static Sprite solidWhiteSprite;

    private static Sprite GetSolidWhiteSprite()
    {
        if (solidWhiteSprite == null)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            solidWhiteSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }
        return solidWhiteSprite;
    }

    private ShipStats playerStats;
    private Image playerHealthFill;
    private Image energyFill;
    private Image cargoFill;
    private Image enemyHealthFill;
    private TextMeshProUGUI playerHPText;
    private TextMeshProUGUI energyText;
    private TextMeshProUGUI cargoText;
    private TextMeshProUGUI enemyHPText;
    private GameObject enemyHealthRoot;

    private HeavyKineticLauncher playerLauncher;
    private Image weaponReloadFill;
    private TextMeshProUGUI weaponReloadText;

    private float nextEnemySearchTime;
    private ShipStats cachedEnemyStats;

    private float currentPlayerHpDisplayed;
    private float playerHpVelocity;
    private float currentEnemyHpDisplayed;
    private float enemyHpVelocity;

    private void Awake()
    {
        playerStats = GetComponent<ShipStats>();
        BuildUi();
    }

    private void LateUpdate()
    {
        UpdatePlayerHealth();
        UpdateEnergy();
        UpdateCargo();
        UpdateEnemyHealth();
        UpdateWeaponReload();
    }

    private void UpdatePlayerHealth()
    {
        if (playerStats == null)
            playerStats = GetComponent<ShipStats>();

        SetHealthBar(playerStats, playerHealthFill, playerHPText, ref currentPlayerHpDisplayed, ref playerHpVelocity);
    }

    private void UpdateEnergy()
    {
        if (playerStats == null || energyFill == null || energyText == null)
            return;

        float max = playerStats.GetMaxEnergy();
        float current = playerStats.CurrentEnergy;
        float pct = max > 0f ? current / max : 0f;
        energyFill.fillAmount = pct;
        energyFill.color = Color.Lerp(Color.red, Color.cyan, pct);
        energyText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    private void UpdateCargo()
    {
        if (playerStats == null || cargoFill == null || cargoText == null)
            return;

        float max = playerStats.GetMaxCargo();
        float current = playerStats.CurrentCargo;
        float pct = max > 0f ? current / max : 0f;
        cargoFill.fillAmount = pct;
        cargoFill.color = Color.Lerp(new Color(0.4f, 0.25f, 0.1f), new Color(0.9f, 0.7f, 0.2f), pct);
        cargoText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    private void UpdateEnemyHealth()
    {
        if (Time.time >= nextEnemySearchTime)
        {
            cachedEnemyStats = FindNearestEnemyStats();
            nextEnemySearchTime = Time.time + 0.25f;
        }

        bool hasEnemy = cachedEnemyStats != null && cachedEnemyStats.GetMaxHP() > 0f;

        if (enemyHealthRoot != null)
            enemyHealthRoot.SetActive(hasEnemy);

        if (hasEnemy)
            SetHealthBar(cachedEnemyStats, enemyHealthFill, enemyHPText, ref currentEnemyHpDisplayed, ref enemyHpVelocity);
    }

    private void UpdateWeaponReload()
    {
        if (playerLauncher == null)
            playerLauncher = GetComponent<HeavyKineticLauncher>();

        if (playerLauncher == null || weaponReloadFill == null)
            return;

        bool hideForStationUi = StationProximity.ShopOpen || StationProximity.RepairOpen;
        if (weaponReloadFill.transform.parent != null)
            weaponReloadFill.transform.parent.gameObject.SetActive(!hideForStationUi);

        if (hideForStationUi)
            return;

        float progress = playerLauncher.GetReloadProgress();
        weaponReloadFill.fillAmount = progress;

        if (progress >= 1f)
        {
            weaponReloadFill.color = new Color(0.1f, 0.9f, 0.1f, 0.85f);
            weaponReloadText.text = "READY";
        }
        else
        {
            weaponReloadFill.color = new Color(1f, 1f, 1f, 0.35f);
            weaponReloadText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
        }
    }

    private ShipStats FindNearestEnemyStats()
    {
        if (GameManager.Instance == null || GameManager.Instance.activeEnemies == null)
            return null;

        float bestDistanceSqr = float.MaxValue;
        ShipStats bestStats = null;

        foreach (EnemyAI enemy in GameManager.Instance.activeEnemies)
        {
            if (enemy == null)
                continue;

            ShipStats stats = enemy.GetComponent<ShipStats>();
            if (stats == null || stats.IsDestroyed)
                continue;

            float distanceSqr = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr < bestDistanceSqr)
            {
                bestDistanceSqr = distanceSqr;
                bestStats = stats;
            }
        }

        return bestStats;
    }

    private void SetHealthBar(ShipStats stats, Image fill, TextMeshProUGUI text, ref float currentDisplayedHp, ref float hpVelocity)
    {
        if (fill == null || text == null)
            return;

        if (stats == null || stats.GetMaxHP() <= 0f)
        {
            fill.fillAmount = 0f;
            text.text = "0 / 0";
            return;
        }

        float maxHp = stats.GetMaxHP();
        float targetHp = Mathf.Clamp(stats.CurrentHP, 0f, maxHp);
        currentDisplayedHp = Mathf.SmoothDamp(currentDisplayedHp, targetHp, ref hpVelocity, 0.15f);

        float hpPercent = currentDisplayedHp / maxHp;
        fill.fillAmount = hpPercent;
        fill.color = GetHealthColor(hpPercent);
        text.text = $"{Mathf.CeilToInt(currentDisplayedHp)} / {Mathf.CeilToInt(maxHp)}";
    }

    private static Color GetHealthColor(float hpPercent)
    {
        if (hpPercent > 0.5f)
            return Color.Lerp(Color.yellow, Color.green, (hpPercent - 0.5f) * 2f);

        return Color.Lerp(Color.red, Color.yellow, hpPercent * 2f);
    }

    private void BuildUi()
    {
        if (SharedUIManager.Instance == null || SharedUIManager.Instance.MainCanvas == null)
            return;

        Transform canvas = SharedUIManager.Instance.MainCanvas.transform;

        CreateHealthBar("PlayerHealth", canvas, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(40f, 40f),
            "PLAYER HP", out playerHealthFill, out playerHPText, out _);

        CreateHealthBar("EnergyBar", canvas, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(40f, 80f),
            "FUEL", out energyFill, out energyText, out _);

        CreateHealthBar("CargoBar", canvas, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(40f, 120f),
            "CARGO", out cargoFill, out cargoText, out _);

        CreateHealthBar("EnemyHealth", canvas, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -40f),
            "ENEMY HP", out enemyHealthFill, out enemyHPText, out enemyHealthRoot);

        CreateHealthBar("WeaponReload", canvas, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-40f, 40f),
            "WEAPON RELOAD", out weaponReloadFill, out weaponReloadText, out _);
    }

    private static void CreateHealthBar(
        string name,
        Transform parent,
        Vector2 anchor,
        Vector2 pivot,
        Vector2 anchoredPosition,
        string label,
        out Image fillImage,
        out TextMeshProUGUI valueText,
        out GameObject rootGo)
    {
        rootGo = new GameObject(name, typeof(RectTransform), typeof(Image));
        rootGo.transform.SetParent(parent, false);

        var root = rootGo.GetComponent<RectTransform>();
        root.anchorMin = anchor;
        root.anchorMax = anchor;
        root.pivot = pivot;
        root.anchoredPosition = anchoredPosition;
        root.sizeDelta = new Vector2(BarWidth, BarHeight);

        var background = rootGo.GetComponent<Image>();
        background.sprite = GetSolidWhiteSprite();
        background.color = new Color(0f, 0f, 0f, 0.65f);
        background.raycastTarget = false;

        var fillGo = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillGo.transform.SetParent(rootGo.transform, false);

        var fillRect = fillGo.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(2f, 2f);
        fillRect.offsetMax = new Vector2(-2f, -2f);

        fillImage = fillGo.GetComponent<Image>();
        fillImage.sprite = GetSolidWhiteSprite();
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.color = Color.green;
        fillImage.raycastTarget = false;

        valueText = CreateText("Value", rootGo.transform, Vector2.zero, Vector2.one, Vector2.zero, 18f);
        valueText.text = "0 / 0";

        TextMeshProUGUI labelText = CreateText("Label", rootGo.transform, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, 20f), 16f);
        labelText.text = label;
        labelText.alignment = TextAlignmentOptions.Left;
        labelText.rectTransform.sizeDelta = new Vector2(0f, 20f);
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 anchoredPosition, float fontSize)
    {
        var textGo = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(parent, false);

        var rect = textGo.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = anchoredPosition;

        var text = textGo.GetComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;

        return text;
    }
}
