using UnityEngine;
using UnityEngine.UI;

public class CombatAimHUD : MonoBehaviour
{
    private const float ReticleSize = 28f;
    private const float LeadSize = 14f;

    private RectTransform reticleRoot;
    private Image reticleH;
    private Image reticleV;
    private Image leadMarker;
    private Image lockBracket;
    private static Sprite whiteSprite;

    private void Awake()
    {
        BuildUi();
    }

    private void Start()
    {
        if (reticleRoot == null)
            BuildUi();
    }

    private void LateUpdate()
    {
        if (reticleRoot == null) return;
        if (StationProximity.RequiresCursor || CombatAimSystem.Instance == null)
        {
            reticleRoot.gameObject.SetActive(false);
            return;
        }

        GameState state = GameManager.Instance != null ? GameManager.Instance.currentState : GameState.Menu;
        bool combat = state == GameState.Exploration || state == GameState.Fighting;
        reticleRoot.gameObject.SetActive(combat);
        if (!combat) return;

        CombatAimSystem aim = CombatAimSystem.Instance;
        Color main = aim.HasLock ? new Color(1f, 0.35f, 0.2f, 0.95f) : new Color(0.85f, 0.95f, 1f, 0.9f);
        reticleH.color = main;
        reticleV.color = main;

        bool showLead = false;
        if (aim.HasAim && aim.TryProjectToScreen(aim.AimPoint, out Vector2 leadViewport))
        {
            Vector2 leadPos = ViewportToCanvas(leadViewport);
            leadMarker.rectTransform.anchoredPosition = leadPos;
            leadMarker.color = aim.HasLock
                ? new Color(1f, 0.5f, 0.15f, 0.85f)
                : new Color(0.7f, 0.8f, 1f, 0.55f);
            showLead = true;
        }

        leadMarker.gameObject.SetActive(showLead);

        bool showBracket = false;
        if (aim.HasLock && aim.LockedTarget != null &&
            aim.TryProjectToScreen(aim.LockedTarget.position, out Vector2 targetViewport))
        {
            lockBracket.rectTransform.anchoredPosition = ViewportToCanvas(targetViewport);
            lockBracket.color = new Color(1f, 0.25f, 0.15f, 0.9f);
            showBracket = true;
        }

        lockBracket.gameObject.SetActive(showBracket);
    }

    private static Vector2 ViewportToCanvas(Vector2 viewport)
    {
        RectTransform canvas = SharedUIManager.Instance != null
            ? SharedUIManager.Instance.MainCanvas.transform as RectTransform
            : null;
        if (canvas == null) return Vector2.zero;

        float w = canvas.rect.width;
        float h = canvas.rect.height;
        return new Vector2((viewport.x - 0.5f) * w, (viewport.y - 0.5f) * h);
    }

    private void BuildUi()
    {
        if (SharedUIManager.Instance == null || SharedUIManager.Instance.MainCanvas == null)
            return;

        var rootGo = new GameObject("CombatAimHUD", typeof(RectTransform));
        rootGo.transform.SetParent(SharedUIManager.Instance.MainCanvas.transform, false);
        reticleRoot = rootGo.GetComponent<RectTransform>();
        reticleRoot.anchorMin = reticleRoot.anchorMax = new Vector2(0.5f, 0.5f);
        reticleRoot.pivot = new Vector2(0.5f, 0.5f);
        reticleRoot.anchoredPosition = Vector2.zero;
        reticleRoot.sizeDelta = new Vector2(80f, 80f);

        reticleH = CreateLine("ReticleH", new Vector2(ReticleSize, 2f));
        reticleV = CreateLine("ReticleV", new Vector2(2f, ReticleSize));
        leadMarker = CreateDot("LeadMarker", LeadSize, new Color(1f, 0.6f, 0.2f, 0.8f));
        lockBracket = CreateDot("LockBracket", 36f, new Color(1f, 0.2f, 0.1f, 0.35f));
        lockBracket.type = Image.Type.Sliced;
    }

    private Image CreateLine(string name, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(reticleRoot, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        var img = go.GetComponent<Image>();
        img.sprite = GetWhiteSprite();
        img.raycastTarget = false;
        return img;
    }

    private Image CreateDot(string name, float size, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(reticleRoot, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(size, size);
        var img = go.GetComponent<Image>();
        img.sprite = GetWhiteSprite();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    private static Sprite GetWhiteSprite()
    {
        if (whiteSprite != null) return whiteSprite;
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        return whiteSprite;
    }
}
