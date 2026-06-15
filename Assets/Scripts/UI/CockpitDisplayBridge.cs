using UnityEngine;
using TMPro;

public class CockpitDisplayBridge : MonoBehaviour
{
    private TextMeshProUGUI hp;
    private TextMeshProUGUI energy;
    private TextMeshProUGUI cargo;
    private TextMeshProUGUI credits;
    private ShipStats stats;

    public void Bind(CockpitDisplayManager mgr, TextMeshProUGUI hpText, TextMeshProUGUI energyText,
        TextMeshProUGUI cargoText, TextMeshProUGUI creditsText, TextMeshProUGUI notif)
    {
        hp = hpText;
        energy = energyText;
        cargo = cargoText;
        credits = creditsText;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            stats = player.GetComponent<ShipStats>();

        GameEvents.OnCreditsChanged += OnCredits;
        GameEvents.OnHullDamaged += (d, _) => mgr?.ShowNotification($"HULL -{d:F0}", Color.red);
        Refresh();
    }

    public void BindBriefing(CockpitDisplayManager mgr, TextMeshProUGUI sectorName, TextMeshProUGUI sectorTerritory,
        TextMeshProUGUI sectorJurisdiction, TextMeshProUGUI sectorProfile, TextMeshProUGUI sectorRisk,
        TextMeshProUGUI sectorOre, TextMeshProUGUI crewNote, TextMeshProUGUI crtLog)
    {
        mgr.BindBriefingFields(sectorName, sectorTerritory, sectorJurisdiction, sectorProfile,
            sectorRisk, sectorOre, crewNote, crtLog);
    }

    void Update() => Refresh();

    private void OnCredits(float c) => Refresh();

    private void Refresh()
    {
        if (stats != null)
        {
            if (hp != null) hp.text = $"HP {stats.CurrentHP:F0}/{stats.GetMaxHP():F0}";
            if (energy != null) energy.text = $"ENG {stats.CurrentEnergy:F0}/{stats.GetMaxEnergy():F0}";
            if (cargo != null) cargo.text = $"CRG {stats.CurrentCargo:F0}/{stats.GetMaxCargo():F0}";
        }

        if (credits != null && EconomyManager.Instance != null)
            credits.text = $"CR {EconomyManager.Instance.Credits:F0}";
    }
}
