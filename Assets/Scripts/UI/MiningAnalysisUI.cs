using TMPro;
using UnityEngine;

public class MiningAnalysisUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI analysisText;
    [SerializeField] private float displayDuration = 4f;

    void Awake()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    void OnEnable() => GameEvents.OnMiningAnalysisReady += ShowAnalysis;
    void OnDisable() => GameEvents.OnMiningAnalysisReady -= ShowAnalysis;

    private void ShowAnalysis(SectorDefinition sector, MiningThreatLevel threat, string composition, float avgTemp)
    {
        string band = avgTemp < 1500f ? "LOW" : avgTemp < 2200f ? "MID" : "HIGH";
        string sectorName = sector != null ? sector.sectorName : "Nieznany";

        if (analysisText != null)
        {
            analysisText.text =
                $"ANALIZA PRÓBKI — {sectorName}\n" +
                $"Zagrożenie: {threat}\n" +
                $"Skład: {composition}\n" +
                $"Śr. temp: {avgTemp:F0}°C ({band})";
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
            CancelInvoke(nameof(Hide));
            Invoke(nameof(Hide), displayDuration);
        }
    }

    private void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }
}
