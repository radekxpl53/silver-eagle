using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(200)]
public class GameplayUIBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoAttach()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "GameManager") return;

        Canvas canvas = FindMainGameplayCanvas();
        if (canvas != null && canvas.GetComponent<GameplayUIBootstrap>() == null)
            canvas.gameObject.AddComponent<GameplayUIBootstrap>();
    }

    static Canvas FindMainGameplayCanvas()
    {
        GameObject named = GameObject.Find("Canvas");
        if (named != null)
        {
            Canvas canvas = named.GetComponent<Canvas>();
            if (canvas != null) return canvas;
        }

        foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (canvas.gameObject.name == "Canvas")
                return canvas;
        }

        return Object.FindFirstObjectByType<Canvas>();
    }

    private GameObject rowPrefab;
    private Transform stationUiRoot;
    private bool shopWasVisible;
    private bool missionsWasVisible;

    void Awake()
    {
        rowPrefab = CreateRowPrefab(transform);
        stationUiRoot = CreateStationUiRoot();
        DisableLegacyShipStatusHud();
        EnsureGameplaySystems();
        EnsureEventSystem();
        BuildAllPanels();
        WireExistingScreens();
    }

    static Transform CreateStationUiRoot()
    {
        var go = new GameObject("StationOverlayCanvas", typeof(RectTransform));
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1024f);
        scaler.matchWidthOrHeight = 1f;

        go.AddComponent<GraphicRaycaster>();
        return go.transform;
    }

    private void DisableLegacyShipStatusHud()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "ShipStatus")
                child.gameObject.SetActive(false);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            UI_ShipStatus legacyHud = player.GetComponent<UI_ShipStatus>();
            if (legacyHud != null)
                legacyHud.enabled = false;
        }
    }

    void Update()
    {
        ToggleStationPanels();
    }

    void LateUpdate()
    {
        StationProximity.ApplyCursor();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        ShipController ship = player.GetComponent<ShipController>();
        if (ship != null)
            ship.isInteractingWithUI = StationUiInput.BlocksWeaponInput;
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null)
            return;

        var go = new GameObject("EventSystem");
        go.AddComponent<UnityEngine.EventSystems.EventSystem>();
        go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
    }

    private void EnsureGameplaySystems()
    {
        if (FindFirstObjectByType<CombatPromptSystem>() == null)
        {
            var go = new GameObject("CombatPromptSystem");
            go.AddComponent<CombatPromptSystem>();
        }

        if (ShopSystem.Instance == null)
        {
            var go = new GameObject("ShopSystem");
            go.AddComponent<ShopSystem>();
        }

        if (FactionMissionSystem.Instance == null)
        {
            var go = new GameObject("FactionMissionSystem");
            go.AddComponent<FactionMissionSystem>();
        }

        if (RepairSupportSystem.Instance == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player.GetComponent<RepairSupportSystem>() == null)
                player.AddComponent<RepairSupportSystem>();
        }

        GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null && playerGo.GetComponent<FastTravelSystem>() == null)
            playerGo.AddComponent<FastTravelSystem>();

        EnsureNarrativeSystems();
    }

    private void EnsureNarrativeSystems()
    {
        if (FindFirstObjectByType<NarrativeDirector>() == null)
        {
            var go = new GameObject("NarrativeDirector");
            go.AddComponent<NarrativeDirector>();
        }

        if (FindFirstObjectByType<SectorTerritoryRules>() == null)
        {
            var go = new GameObject("SectorTerritoryRules");
            go.AddComponent<SectorTerritoryRules>();
        }
    }

    private void BuildAllPanels()
    {
        Transform root = transform;

        BuildCombatPrompt(root);
        BuildMiningAnalysis(root);
        BuildSellSummary(root);
        BuildShopPanel(stationUiRoot);
        BuildMissionPanel(stationUiRoot);
    }

    private void WireExistingScreens()
    {
        DeathScreenUI death = FindFirstObjectByType<DeathScreenUI>(FindObjectsInactive.Include);
        if (death != null)
            death.EnsureLayout();
    }

    private void ToggleStationPanels()
    {
        Transform shop = stationUiRoot != null ? stationUiRoot.Find("ShopPanel") : null;
        Transform missions = stationUiRoot != null ? stationUiRoot.Find("MissionPanel") : null;

        bool showShop = StationProximity.ShopOpen;
        bool showMissions = StationProximity.MissionsOpen;

        if (shop != null)
        {
            if (shop.gameObject.activeSelf != showShop)
                shop.gameObject.SetActive(showShop);
            if (showShop && !shopWasVisible)
            {
                ShopUI ui = shop.GetComponent<ShopUI>();
                if (ui != null) ui.Refresh();
            }
            shopWasVisible = showShop;
        }

        if (missions != null)
        {
            if (missions.gameObject.activeSelf != showMissions)
                missions.gameObject.SetActive(showMissions);
            if (showMissions && !missionsWasVisible)
            {
                MissionUI ui = missions.GetComponent<MissionUI>();
                if (ui != null) ui.Refresh();
            }
            missionsWasVisible = showMissions;
        }
    }

    private void BuildCombatPrompt(Transform root)
    {
        var panel = CreatePanel(root, "CombatPromptPanel", new Vector2(0.5f, 0.35f), new Vector2(0.5f, 0.35f),
            Vector2.zero, new Vector2(420, 160), new Color(0.05f, 0.05f, 0.12f, 0.9f));
        panel.gameObject.SetActive(false);

        var prompt = CreateTmp(panel, "Prompt", Vector2.zero, 28, "Wróg — walcz / uciekaj", TextAlignmentOptions.Center);
        var fightBtn = CreateButton(panel, "FightBtn", new Vector2(-90, -45), "WALCZ", new Vector2(140, 44));
        var fleeBtn = CreateButton(panel, "FleeBtn", new Vector2(90, -45), "UCIEKAJ", new Vector2(140, 44));

        var ui = panel.gameObject.AddComponent<CombatPromptUI>();
        SetPrivateField(ui, "panelRoot", panel.gameObject);
        SetPrivateField(ui, "promptText", prompt);
        SetPrivateField(ui, "fightButton", fightBtn);
        SetPrivateField(ui, "fleeButton", fleeBtn);
    }

    private void BuildMiningAnalysis(Transform root)
    {
        var panel = CreatePanel(root, "MiningAnalysisPanel", new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f),
            Vector2.zero, new Vector2(520, 200), new Color(0, 0.1f, 0.15f, 0.92f));
        panel.gameObject.SetActive(false);
        var text = CreateTmp(panel, "Analysis", Vector2.zero, 24, "", TextAlignmentOptions.Center, new Vector2(500, 180));
        var ui = panel.gameObject.AddComponent<MiningAnalysisUI>();
        SetPrivateField(ui, "panelRoot", panel.gameObject);
        SetPrivateField(ui, "analysisText", text);
    }

    private void BuildSellSummary(Transform root)
    {
        var panel = CreatePanel(root, "SellSummaryPanel", new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f),
            Vector2.zero, new Vector2(460, 180), new Color(0.02f, 0.12f, 0.05f, 0.9f));
        panel.gameObject.SetActive(false);
        var text = CreateTmp(panel, "Summary", Vector2.zero, 22, "", TextAlignmentOptions.Center, new Vector2(440, 160));
        var ui = panel.gameObject.AddComponent<SellSummaryUI>();
        SetPrivateField(ui, "panelRoot", panel.gameObject);
        SetPrivateField(ui, "summaryText", text);
    }

    private void BuildShopPanel(Transform root)
    {
        if (root.Find("ShopPanel") != null) return;

        var panel = CreateEdgePanel(root, "ShopPanel", rightSide: true, 320, 420, 16f,
            new Color(0.08f, 0.08f, 0.14f, 0.88f));
        panel.gameObject.SetActive(false);
        EnsureStationPanelCanvas(panel);

        CreateTmp(panel, "ShopTitle", new Vector2(0, 185), 26, "SKLEP", TextAlignmentOptions.Center, new Vector2(280, 36));
        var status = CreateTmp(panel, "ShopStatus", new Vector2(0, -195), 16, "", TextAlignmentOptions.Center, new Vector2(280, 28));
        var scroll = CreateScrollList(panel, "ShopList", new Vector2(0, -15), new Vector2(300, 330));

        var ui = panel.gameObject.AddComponent<ShopUI>();
        SetPrivateField(ui, "listRoot", scroll);
        SetPrivateField(ui, "rowPrefab", rowPrefab);
        SetPrivateField(ui, "statusText", status);
    }

    private void BuildMissionPanel(Transform root)
    {
        if (root.Find("MissionPanel") != null) return;

        var panel = CreateEdgePanel(root, "MissionPanel", rightSide: false, 320, 420, 16f,
            new Color(0.1f, 0.08f, 0.05f, 0.88f));
        panel.gameObject.SetActive(false);
        EnsureStationPanelCanvas(panel);

        CreateTmp(panel, "MissionTitle", new Vector2(0, 185), 26, "MISJE", TextAlignmentOptions.Center, new Vector2(280, 36));
        var status = CreateTmp(panel, "MissionStatus", new Vector2(0, -195), 16, "", TextAlignmentOptions.Center, new Vector2(280, 28));
        var scroll = CreateScrollList(panel, "MissionList", new Vector2(0, -15), new Vector2(300, 330));

        var ui = panel.gameObject.AddComponent<MissionUI>();
        SetPrivateField(ui, "listRoot", scroll);
        SetPrivateField(ui, "rowPrefab", rowPrefab);
        SetPrivateField(ui, "statusText", status);
    }

    // --- UI helpers ---

    static RectTransform CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pos, Vector2 size, Color bg)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        go.GetComponent<Image>().color = bg;
        return rt;
    }

    static RectTransform CreateEdgePanel(Transform parent, string name, bool rightSide, float width, float height,
        float margin, Color bg)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        if (rightSide)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(1f, 0.5f);
            rt.pivot = new Vector2(1f, 0.5f);
            rt.anchoredPosition = new Vector2(-margin, 0f);
        }
        else
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(margin, 0f);
        }

        rt.sizeDelta = new Vector2(width, height);
        go.GetComponent<Image>().color = bg;
        return rt;
    }

    static void EnsureStationPanelCanvas(RectTransform panel)
    {
        var bg = panel.GetComponent<Image>();
        if (bg != null) bg.raycastTarget = true;
    }

    static TextMeshProUGUI CreateTmp(Transform parent, string name, Vector2 pos, float fontSize, string text,
        TextAlignmentOptions align, Vector2? size = null)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size ?? new Vector2(300, 40);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        return tmp;
    }

    static Button CreateButton(Transform parent, string name, Vector2 pos, string label, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        go.GetComponent<Image>().color = new Color(0.2f, 0.45f, 0.75f, 1f);
        CreateTmp(go.transform, "Label", Vector2.zero, 20, label, TextAlignmentOptions.Center, size);
        return go.GetComponent<Button>();
    }

    static Transform CreateScrollList(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        var scrollGo = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Mask), typeof(ScrollRect));
        scrollGo.transform.SetParent(parent, false);
        var scrollRt = scrollGo.GetComponent<RectTransform>();
        scrollRt.anchorMin = scrollRt.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRt.pivot = new Vector2(0.5f, 0.5f);
        scrollRt.anchoredPosition = pos;
        scrollRt.sizeDelta = size;
        var scrollBg = scrollGo.GetComponent<Image>();
        scrollBg.color = new Color(0, 0, 0, 0.25f);
        scrollBg.raycastTarget = true;

        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(scrollGo.transform, false);
        var crt = content.GetComponent<RectTransform>();
        crt.anchorMin = new Vector2(0, 1);
        crt.anchorMax = new Vector2(1, 1);
        crt.pivot = new Vector2(0.5f, 1);
        crt.anchoredPosition = Vector2.zero;
        crt.sizeDelta = new Vector2(0, 0);
        var vlg = content.GetComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.spacing = 8;
        vlg.padding = new RectOffset(6, 6, 6, 6);
        content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scroll = scrollGo.GetComponent<ScrollRect>();
        scroll.content = crt;
        scroll.viewport = scrollRt;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 20f;
        return content.transform;
    }

    static GameObject CreateRowPrefab(Transform parent)
    {
        var row = new GameObject("ShopRowPrefab", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement), typeof(StationRowClick));
        row.transform.SetParent(parent, false);
        row.GetComponent<Image>().color = new Color(0.15f, 0.2f, 0.3f, 0.9f);
        row.GetComponent<Image>().raycastTarget = true;
        row.GetComponent<LayoutElement>().preferredHeight = 104;
        var button = row.GetComponent<Button>();
        var colors = button.colors;
        colors.highlightedColor = new Color(0.25f, 0.4f, 0.6f, 1f);
        colors.pressedColor = new Color(0.1f, 0.3f, 0.5f, 1f);
        button.colors = colors;
        var rt = row.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(280, 104);
        var label = CreateTmp(row.transform, "Label", Vector2.zero, 14, "Item", TextAlignmentOptions.TopLeft, new Vector2(260, 96));
        var labelRt = label.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = new Vector2(8f, 4f);
        labelRt.offsetMax = new Vector2(-8f, -4f);
        label.enableWordWrapping = true;
        label.overflowMode = TextOverflowModes.Ellipsis;
        row.SetActive(false);
        return row;
    }

    static TextMeshProUGUI FindTmp(Transform root, string name)
    {
        foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (tmp.name.Contains(name)) return tmp;
        }
        return null;
    }

    static void SetPrivateField(object target, string field, object value)
    {
        var fieldInfo = target.GetType().GetField(field,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        fieldInfo?.SetValue(target, value);
    }
}
