using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShipStats))]
public class latanieTpp : MonoBehaviour
{
    [Header("PARAMETRY BAZOWE")]
    [SerializeField] private float baseMass = 100000f;
    [SerializeField] private float cargoCapacity = 80000f;
    [SerializeField] private float maxMainThrust = 800000f;
    [SerializeField] private float brakeThrust = 400000f;
    [SerializeField] private float maneuveringTorque = 250000f;

    [Header("PALIWO")]
    [SerializeField] private float emergencySpeedMultiplier = 0.3f; // 30% prędkości bez paliwa
    [SerializeField] private float normalDrainRate = 5f;            // Spalanie na sekundę

    [Header("ILOSC LADUNKU")]
    [Range(0f, 1f)]
    public float currentLoadPercent = 0f;

    [Header("INNE")]
    [SerializeField] private float liftThrust = 30000f;
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private ShipStats shipStats;
    private float previousLoadPercent = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shipStats = GetComponent<ShipStats>();

        rb.useGravity = false;
        rb.linearDamping = 0.5f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        UpdatePhysics();
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(currentLoadPercent - previousLoadPercent) > 0.001f)
        {
            UpdatePhysics();
            previousLoadPercent = currentLoadPercent;
        }

        handleMovement();
    }

    private void UpdatePhysics()
    {
        rb.mass = baseMass + cargoCapacity * currentLoadPercent;
        rb.angularDamping = 1.5f - (1.5f - 0.9f) * currentLoadPercent;
    }

    void handleMovement()
    {
        float gasInput = 0f;
        float turnInput = 0f;
        float verticalInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) gasInput = 1f;
            if (Keyboard.current.sKey.isPressed) gasInput = -1f;

            if (Keyboard.current.aKey.isPressed) turnInput = -1f;
            if (Keyboard.current.dKey.isPressed) turnInput = 1f;

            if (Keyboard.current.spaceKey.isPressed) verticalInput = 1f;
            if (Keyboard.current.leftShiftKey.isPressed) verticalInput = -1f;
        }

        bool hasFuel = shipStats.CurrentEnergy > 0f;
        bool isMoving = gasInput != 0 || turnInput != 0 || verticalInput != 0;

        float currentPerformanceMode = hasFuel ? 1f : emergencySpeedMultiplier;

        // A. PRZÓD / HAMOWANIE 
        if (gasInput != 0)
        {
            float baseThrust = (gasInput > 0) ? maxMainThrust : brakeThrust;
            rb.AddRelativeForce(Vector3.forward * gasInput * baseThrust * currentPerformanceMode);
        }

        // B. SKRĘT
        if (turnInput != 0)
        {
            rb.AddRelativeTorque(Vector3.up * turnInput * maneuveringTorque * currentPerformanceMode);
        }

        // C. WINDA
        if (verticalInput != 0)
        {
            rb.AddForce(Vector3.up * verticalInput * liftThrust * currentPerformanceMode);
        }

        // ZUŻYCIE PALIWA
        if (isMoving && hasFuel)
        {
            shipStats.UseEnergy(normalDrainRate * Time.fixedDeltaTime);
        }
    }
}