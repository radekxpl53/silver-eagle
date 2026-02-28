using UnityEngine;

public class damageColision : MonoBehaviour
{
    [Header("--- KALIBRACJA ZDERZEŃ (Dla masy 100t) ---")]
    public float damageThreshold = 20000f;
    public float damageMultiplier = 0.005f;

    [Header("--- EFEKTY ---")]
    public ParticleSystem impactParticles;

    void OnCollisionEnter(Collision collision)
    {
        // 1. OBLICZANIE SIŁY UDERZENIA
        float impactForce = collision.impulse.magnitude;

        //Debug.Log($"<color=orange>TEST KOLIZJI:</color> Dotknięto {collision.gameObject.name}, siła: {impactForce}");

        if (impactForce < damageThreshold) return;

        // 2. OBLICZANIE OBRAŻEŃ
        float damageToTake = (impactForce - damageThreshold) * damageMultiplier;
        damageToTake = Mathf.Max(0, damageToTake);

        // 3. PRZEKAZANIE OBRAŻEŃ
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ApplyDamage(damageToTake);
        }
        else
        {
            Debug.LogError("Brak GameManagera w scenie!");
        }

        // 4. DEBUGOWANIE I EFEKTY
        Debug.Log($"<color=red>KOLIZJA!</color> Obiekt: {collision.gameObject.name} | Siła (Impuls): {impactForce:F0} | Przekazane obrażenia: {damageToTake:F0}");

        if (impactParticles != null)
        {
            impactParticles.transform.position = collision.contacts[0].point;
            impactParticles.Play();
        }
    }
}