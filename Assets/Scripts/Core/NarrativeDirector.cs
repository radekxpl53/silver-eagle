using UnityEngine;

public class NarrativeDirector : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnSectorEntered += HandleSectorEntered;
        GameEvents.OnCombatStarted += HandleCombatStarted;
        GameEvents.OnHullDamaged += HandleHullDamaged;
        GameEvents.OnDebtChanged += HandleDebtChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnSectorEntered -= HandleSectorEntered;
        GameEvents.OnCombatStarted -= HandleCombatStarted;
        GameEvents.OnHullDamaged -= HandleHullDamaged;
        GameEvents.OnDebtChanged -= HandleDebtChanged;
    }

    private void HandleSectorEntered(Vector2Int grid, SectorDefinition sector)
    {
        if (sector == null) return;

        string note = sector.crewNote;
        if (string.IsNullOrWhiteSpace(note))
        {
            // Pick a random crew member and get sector enter bark
            CrewMember member = (CrewMember)Random.Range(0, 4);
            note = CrewBarks.GetBark(member, BarkEvent.SectorEnter);
            sector.crewNote = note; // Cache it
        }

        if (CockpitDisplayManager.Instance != null)
        {
            CockpitDisplayManager.Instance.ShowNotification(note, Color.yellow);
        }
    }

    private void HandleCombatStarted()
    {
        TriggerCrewBark(BarkEvent.Combat, Color.red);
    }

    private void HandleHullDamaged(float damage, Vector3 hitPoint)
    {
        // Low fuel or combat hull damage barks
        TriggerCrewBark(BarkEvent.Combat, Color.red);
    }

    private void HandleDebtChanged(float debt)
    {
        if (debt > 0f)
        {
            TriggerCrewBark(BarkEvent.Debt, Color.magenta);
        }
    }

    private void TriggerCrewBark(BarkEvent barkEvent, Color color)
    {
        CrewMember member = (CrewMember)Random.Range(0, 4);
        string bark = CrewBarks.GetBark(member, barkEvent);
        if (CockpitDisplayManager.Instance != null)
        {
            CockpitDisplayManager.Instance.ShowNotification(bark, color);
        }
    }
}
