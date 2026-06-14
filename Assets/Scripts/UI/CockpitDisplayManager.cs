using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CockpitDisplayManager : MonoBehaviour, IDiegeticDisplay
{
    public static CockpitDisplayManager Instance { get; private set; }

    [Header("Ship Stats Displays")]
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private Slider energySlider;
    [SerializeField] private TextMeshProUGUI cargoText;
    [SerializeField] private Slider cargoSlider;

    [Header("Sector Info Panel")]
    [SerializeField] private TextMeshProUGUI sectorNameText;
    [SerializeField] private TextMeshProUGUI sectorTerritoryText;
    [SerializeField] private TextMeshProUGUI sectorJurisdictionText;
    [SerializeField] private TextMeshProUGUI sectorProfileText;
    [SerializeField] private TextMeshProUGUI sectorRiskText;
    [SerializeField] private TextMeshProUGUI sectorOreText;
    [SerializeField] private TextMeshProUGUI crewNoteText;

    [Header("CRT Log Display")]
    [SerializeField] private TextMeshProUGUI crtLogText;

    [Header("Notifications")]
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private Image notificationPanel;
    [SerializeField] private Color normalNotificationColor = Color.cyan;
    [SerializeField] private Color alarmNotificationColor = Color.red;

    private Coroutine notificationCoroutine;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnSectorEntered += HandleSectorEntered;
        GameEvents.OnCreditsChanged += SetCredits;
        GameEvents.OnHullDamaged += HandleHullDamaged;
    }

    private void OnDisable()
    {
        GameEvents.OnSectorEntered -= HandleSectorEntered;
        GameEvents.OnCreditsChanged -= SetCredits;
        GameEvents.OnHullDamaged -= HandleHullDamaged;
    }

    private void HandleSectorEntered(Vector2Int grid, SectorDefinition sector)
    {
        if (sector != null)
        {
            ShowSectorBriefing(sector);
            ShowCRTLog(sector.crtLogEntries);
            ShowNotification($"Entered Sector: {sector.sectorName}", normalNotificationColor);
        }
    }

    private void HandleHullDamaged(float damage, Vector3 hitPoint)
    {
        ShowNotification($"WARNING: HULL DAMAGED! -{damage:F0} HP", alarmNotificationColor);
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashScreenRed());
    }

    private IEnumerator FlashScreenRed()
    {
        if (notificationPanel != null)
        {
            float elapsed = 0f;
            float duration = 0.5f;
            Color originalColor = notificationPanel.color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.PingPong(elapsed * 4f, 1f);
                notificationPanel.color = Color.Lerp(originalColor, new Color(1f, 0f, 0f, 0.4f), t);
                yield return null;
            }
            notificationPanel.color = originalColor;
        }
    }

    public void SetCredits(float credits)
    {
        if (creditsText != null)
        {
            creditsText.text = $"Credits: {credits:N0}";
        }
    }

    public void SetHP(float current, float max)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = max;
            hpSlider.value = current;
        }
        if (hpText != null)
        {
            hpText.text = $"HP: {current:F0} / {max:F0}";
        }
    }

    public void SetEnergy(float current, float max)
    {
        if (energySlider != null)
        {
            energySlider.maxValue = max;
            energySlider.value = current;
        }
        if (energyText != null)
        {
            energyText.text = $"Energy: {current:F0} / {max:F0}";
        }
    }

    public void SetCargo(float current, float max)
    {
        if (cargoSlider != null)
        {
            cargoSlider.maxValue = max;
            cargoSlider.value = current;
        }
        if (cargoText != null)
        {
            cargoText.text = $"Cargo: {current:F0} / {max:F0}";
        }
    }

    public void ShowSectorBriefing(SectorDefinition sector)
    {
        if (sector == null) return;

        if (sectorNameText != null) sectorNameText.text = sector.sectorName;
        if (sectorTerritoryText != null) sectorTerritoryText.text = $"Territory: {sector.territory}";
        if (sectorJurisdictionText != null) sectorJurisdictionText.text = $"Jurisdiction: {sector.jurisdictionText}";
        if (sectorProfileText != null) sectorProfileText.text = sector.profileText;
        if (sectorRiskText != null) sectorRiskText.text = $"Risk Level: {sector.riskLevel}/4 ({sector.miningThreatLevel})";
        if (sectorOreText != null) sectorOreText.text = $"Ore Forecast: {sector.oreForecastText}";
        if (crewNoteText != null) crewNoteText.text = sector.crewNote;
    }

    public void ShowCRTLog(string[] entries)
    {
        if (crtLogText != null && entries != null)
        {
            crtLogText.text = string.Join("\n", entries);
        }
    }

    public void ShowNotification(string message, Color color)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.color = color;
        }

        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }
        notificationCoroutine = StartCoroutine(ClearNotificationAfterDelay(4f));
    }

    private IEnumerator ClearNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (notificationText != null)
        {
            notificationText.text = "";
        }
    }
}
