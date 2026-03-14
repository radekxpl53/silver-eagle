using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(ShipStats))]
public class fppLatanie : MonoBehaviour
{
    [Header("PARAMETRY BAZOWE")]
    public float baseMass = 100000f;
    public float cargoCapacity = 80000f;
    public float maxMainThrust = 800000f;

    [Header("SIŁY HAMOWANIA I MANEWRÓW")]
    public float brakeThrust = 400000f;
    public float maneuverForce = 250000f;
    public float rollForce = 250000f;

    [Header("PALIWO")]
    public float emergencySpeedMultiplier = 0.3f;
    public float maxDrainRate = 1f; // Maksymalne spalanie przy 100% przepustnicy

    [Header("OSTRZEŻENIE O PALIWIE")]
    public float lowFuelThreshold = 40f;
    private bool lowFuelWarningTriggered = false;

    [Header("ILOSC LADUNKU")]
    [Range(0f, 1f)]
    public float currentLoadPercent = 0f;

    [Header("STEROWANIE")]
    public float throttleSensitivity = 0.05f;
    public float mouseSensitivity = 0.5f;

    [Header("PODGLAD MOCY SILNIKOW")]
    [Range(0f, 100f)]
    public float currentThrottle = 0f;

    public Transform cameraTransform;

    private Rigidbody rb;
    private ShipStats shipStats;
    private float previousLoadPercent = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shipStats = GetComponent<ShipStats>();

        rb.useGravity = false;
        rb.linearDamping = 0.5f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdatePhysics();
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.y.ReadValue() * throttleSensitivity;
            currentThrottle = Mathf.Clamp(currentThrottle + scroll, 0f, 100f);
        }

        if (Mathf.Abs(currentLoadPercent - previousLoadPercent) > 0.001f)
        {
            UpdatePhysics();
            previousLoadPercent = currentLoadPercent;
        }
    }

    private void UpdatePhysics()
    {
        rb.mass = baseMass + cargoCapacity * currentLoadPercent;
        rb.angularDamping = Mathf.Lerp(1.5f, 0.9f, currentLoadPercent);
    }

    void FixedUpdate()
    {
        bool hasFuel = shipStats.CurrentEnergy > 0f;
        float currentPerformanceMode = hasFuel ? 1f : emergencySpeedMultiplier;

        float currentThrustForce = maxMainThrust * (currentThrottle / 100f) * currentPerformanceMode;
        rb.AddRelativeForce(Vector3.forward * currentThrustForce);

        if (Keyboard.current != null && Keyboard.current.sKey.isPressed)
        {
            rb.AddRelativeForce(Vector3.forward * -brakeThrust * currentPerformanceMode);
        }

        float mouseX = 0f, mouseY = 0f, rollInput = 0f;

        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            mouseX = delta.x * mouseSensitivity * Time.fixedDeltaTime * 50f;
            mouseY = delta.y * mouseSensitivity * Time.fixedDeltaTime * 50f;
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) rollInput = -1f;
            if (Keyboard.current.dKey.isPressed) rollInput = 1f;
        }

        float pitchForce = -mouseY * maneuverForce * currentPerformanceMode;
        float yawForce = mouseX * maneuverForce * currentPerformanceMode;
        float rollTorque = -rollInput * rollForce * currentPerformanceMode;

        rb.AddRelativeTorque(new Vector3(pitchForce, yawForce, rollTorque));

        // ZUŻYCIE PALIWA PROPORCJONALNIE DO MOCY SILNIKA
        if (currentThrottle > 0f && hasFuel)
        {
            float throttlePercent = currentThrottle / 100f;
            shipStats.UseEnergy(maxDrainRate * throttlePercent * Time.fixedDeltaTime);
        }

        CheckFuelWarning();
    }
    private void CheckFuelWarning()
    {
        if (shipStats.CurrentEnergy <= lowFuelThreshold && !lowFuelWarningTriggered && shipStats.CurrentEnergy > 0)
        {
            lowFuelWarningTriggered = true;
            Debug.LogWarning("<color=red><b>[FPP] UWAGA: Niski poziom paliwa!</b></color>");
        }
        else if (shipStats.CurrentEnergy > lowFuelThreshold && lowFuelWarningTriggered)
        {
            lowFuelWarningTriggered = false;
        }
    }
}