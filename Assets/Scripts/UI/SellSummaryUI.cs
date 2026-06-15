using TMPro;
using UnityEngine;

public class SellSummaryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private GameObject panelRoot;

    void OnEnable()
    {
        GameEvents.OnResourcesSold += ShowSummary;
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    void OnDisable()
    {
        GameEvents.OnResourcesSold -= ShowSummary;
    }

    private void ShowSummary()
    {
        if (summaryText == null) return;

        float debt = EconomyManager.Instance != null ? EconomyManager.Instance.Debt : 0f;
        summaryText.text =
            $"Sprzedaż zakończona\n" +
            $"Brutto: {SellSystem.LastSaleGross:F0} cr\n" +
            $"Podatek ({SellSystem.LastSaleTaxPercent:F0}%): -{SellSystem.LastSaleTax:F0} cr\n" +
            $"Netto: +{SellSystem.LastSaleNet:F0} cr\n" +
            $"Aktualny dług: {debt:F0} cr";

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
            CancelInvoke(nameof(Hide));
            Invoke(nameof(Hide), 5f);
        }
    }

    private void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }
}
