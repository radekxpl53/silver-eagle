using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-90)]
public class MainMenuUIBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoAttach()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu") return;
        MainMenuManager menu = Object.FindFirstObjectByType<MainMenuManager>();
        if (menu != null && menu.GetComponent<MainMenuUIBootstrap>() == null)
            menu.gameObject.AddComponent<MainMenuUIBootstrap>();
    }

    void Awake()
    {
        if (LocalizationManager.Instance == null)
        {
            var loc = new GameObject("LocalizationManager");
            loc.AddComponent<LocalizationManager>();
        }

        BuildConfirmDialog();
        BuildCreditsPanel();
    }

    private void BuildConfirmDialog()
    {
        MainMenuManager menu = GetComponent<MainMenuManager>();
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null || menu == null) return;

        var panel = CreatePanel(canvas.transform, "ConfirmOverwritePanel", Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.zero, new Color(0, 0, 0, 0.82f));
        panel.gameObject.SetActive(false);

        string q = LocalizationManager.Instance != null
            ? LocalizationManager.Instance.GetString("MENU_CONFIRM_OVERWRITE")
            : "Nadpisać zapis?";

        CreateTmp(panel, "Question", new Vector2(0, 40), 30, q, TextAlignmentOptions.Center, new Vector2(700, 80));
        var yes = CreateButton(panel, "Yes", new Vector2(-100, -50), "TAK", new Vector2(160, 50));
        var no = CreateButton(panel, "No", new Vector2(100, -50), "NIE", new Vector2(160, 50));

        SetField(menu, "confirmOverwritePanel", panel.gameObject);
        SetField(menu, "confirmYesButton", yes);
        SetField(menu, "confirmNoButton", no);
    }

    private void BuildCreditsPanel()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        var panel = CreatePanel(canvas.transform, "CreditsPanel", Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.zero, new Color(0.02f, 0.02f, 0.08f, 0.95f));
        panel.gameObject.SetActive(false);

        var body = CreateTmp(panel, "CreditsBody", Vector2.zero, 24, "", TextAlignmentOptions.Center, new Vector2(800, 500));
        panel.gameObject.AddComponent<CreditsScreen>();
        var credits = panel.GetComponent<CreditsScreen>();
        credits.SetBody(body);

        var back = CreateButton(panel, "Back", new Vector2(0, -280), "WRÓĆ", new Vector2(180, 48));
        back.onClick.AddListener(() => panel.gameObject.SetActive(false));

        // Podłącz przycisk Credits jeśli istnieje
        foreach (var btn in canvas.GetComponentsInChildren<Button>(true))
        {
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null && (tmp.text.Contains("Autor") || tmp.text.Contains("Credit")))
            {
                btn.onClick.AddListener(() => panel.gameObject.SetActive(true));
                break;
            }
        }
    }

    static RectTransform CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pos, Vector2 size, Color bg)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        go.GetComponent<Image>().color = bg;
        return rt;
    }

    static TextMeshProUGUI CreateTmp(Transform parent, string name, Vector2 pos, float fontSize, string text,
        TextAlignmentOptions align, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.color = Color.white;
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
        go.GetComponent<Image>().color = new Color(0.25f, 0.4f, 0.7f, 1f);
        CreateTmp(go.transform, "Label", Vector2.zero, 22, label, TextAlignmentOptions.Center, size);
        return go.GetComponent<Button>();
    }

    static void SetField(object target, string field, object value)
    {
        var f = target.GetType().GetField(field,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        f?.SetValue(target, value);
    }
}
