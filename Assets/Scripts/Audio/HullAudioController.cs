using UnityEngine;
using FMODUnity;

public class HullAudioController : MonoBehaviour
{
    [SerializeField] private EventReference hullDamageEvent;
    
    private const string HULL_STRESS_PARAM = "HullStress";
    private float currentStress = 0f;
    [SerializeField] private float stressDecayRate = 20f; // Decay per second

    private void OnEnable()
    {
        GameEvents.OnHullDamaged += HandleHullDamaged;
    }

    private void OnDisable()
    {
        GameEvents.OnHullDamaged -= HandleHullDamaged;
    }

    private void HandleHullDamaged(float damage, Vector3 hitPoint)
    {
        // Play damage one-shot
        if (!hullDamageEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(hullDamageEvent, hitPoint);
        }

        // Spike stress based on damage magnitude
        currentStress = Mathf.Clamp(currentStress + damage * 2f, 0f, 100f);
        UpdateFmodStress();
    }

    private void Update()
    {
        if (currentStress > 0f)
        {
            currentStress = Mathf.Max(0f, currentStress - stressDecayRate * Time.deltaTime);
            UpdateFmodStress();
        }
    }

    private void UpdateFmodStress()
    {
        FMOD.RESULT result = RuntimeManager.StudioSystem.setParameterByName(HULL_STRESS_PARAM, currentStress);
        if (result != FMOD.RESULT.OK)
        {
            // Fail silently or log once
        }
    }
}
