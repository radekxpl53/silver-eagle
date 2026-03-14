using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(ShipStats))]
public class latanieTpp : MonoBehaviour
{
    [Header("PARAMETRY BAZOWE")]
    [SerializeField] private float baseMass = 100000f;
    [SerializeField] private float cargoCapacity = 80000f;
    [SerializeField] private float maxMainThrust = 800000f;
    [SerializeField] private float brakeThrust = 400000f;
    [SerializeField] private float maneuveringTorque = 250000f;

    [Header("WIZUALNY PRZECHYŁ (Tylko Wygląd)")]
    [Tooltip("Przeciągnij tu obiekt podrzędny (dziecko), który zawiera sam model 3D statku.")]
    [SerializeField] private Transform shipVisualModel;
    [SerializeField] private float maxRollAngle = 15f;
    [SerializeField] private float rollSpeed = 45f;      // Jak szybko statek zmienia przechył (stopnie na sekundę)
    [SerializeField] private float rollSmoothSpeed = 5f; // Miękkość wyhamowywania przechyłu

    [Header("PALIWO")]
    [SerializeField] private float emergencySpeedMultiplier = 0.3f;
    [SerializeField] private float normalDrainRate = 5f;

    [Header("ILOSC LADUNKU (Tylko Podgląd)")]
    public float currentLoadPercent = 0f;

    [Header("INNE")]
    [SerializeField] private float liftThrust = 30000f;
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private ShipStats shipStats;
    private float previousLoadPercent = -1f;

    // Zmienne do wizualnego przechyłu
    private float currentVisualRoll = 0f;
    private float targetVisualRoll = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shipStats = GetComponent<ShipStats>();

        rb.useGravity = false;

        // Blokada fizycznego obrotu X i Z, fizycznie statek zawsze jest płaski
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        UpdatePhysics();
    }

    void Update()
    {
        float maxCargo = shipStats.GetMaxCargo();
        currentLoadPercent = maxCargo > 0 ? shipStats.CurrentCargo / maxCargo : 0f;
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
        rb.mass = baseMass + (cargoCapacity * currentLoadPercent);
        rb.angularDamping = Mathf.Lerp(1.5f, 0.9f, currentLoadPercent);
        rb.linearDamping = Mathf.Lerp(0.5f, 0.05f, currentLoadPercent);
    }

    void handleMovement()
    {
        float gasInput = 0f;
        float turnInput = 0f;
        float verticalInput = 0f;
        float rollInput = 0f;

        if (Keyboard.current != null && (GameManager.Instance.currentState == GameState.Exploration || GameManager.Instance.currentState == GameState.Fighting))
        {
            if (Keyboard.current.wKey.isPressed) gasInput = 1f;
            if (Keyboard.current.sKey.isPressed) gasInput = -1f;

            if (Keyboard.current.aKey.isPressed) turnInput = -1f;
            if (Keyboard.current.dKey.isPressed) turnInput = 1f;

            if (Keyboard.current.spaceKey.isPressed) verticalInput = 1f;
            if (Keyboard.current.leftShiftKey.isPressed) verticalInput = -1f;

            if (Keyboard.current.qKey.isPressed) rollInput = 1f;
            if (Keyboard.current.eKey.isPressed) rollInput = -1f;
        }

        bool hasFuel = shipStats.CurrentEnergy > 0f;
        bool isMoving = gasInput != 0 || turnInput != 0 || verticalInput != 0 || rollInput != 0;

        float currentPerformanceMode = hasFuel ? 1f : emergencySpeedMultiplier;

        // A. PRZÓD / HAMOWANIE 
        if (gasInput != 0)
        {
            float baseThrust = (gasInput > 0) ? maxMainThrust : brakeThrust;
            rb.AddRelativeForce(Vector3.forward * gasInput * baseThrust * currentPerformanceMode);
        }

        // B. SKRĘT (Yaw) - używamy AddTorque (globalnej osi Y), by skręcał zawsze idealnie płasko
        if (turnInput != 0)
        {
            rb.AddTorque(Vector3.up * turnInput * maneuveringTorque * currentPerformanceMode);
        }

        // C. WINDA
        if (verticalInput != 0)
        {
            rb.AddForce(Vector3.up * verticalInput * liftThrust * currentPerformanceMode);
        }

        // D. WIZUALNY PRZECHYŁ (Q/E) - zapamiętuje pozycję
        if (rollInput != 0)
        {
            // Zmieniamy docelowy kąt tylko wtedy, gdy gracz wciska Q lub E
            targetVisualRoll += rollInput * rollSpeed * Time.fixedDeltaTime;

            // Pilnujemy, by nie przekroczyć twardego limitu 15 stopni
            targetVisualRoll = Mathf.Clamp(targetVisualRoll, -maxRollAngle, maxRollAngle);
        }

        // Płynnie przechodzimy do docelowego kąta (daje to efekt łagodnego startu/stopu)
        currentVisualRoll = Mathf.Lerp(currentVisualRoll, targetVisualRoll, Time.fixedDeltaTime * rollSmoothSpeed);

        if (shipVisualModel != null)
        {
            // Aplikujemy obrót TYLKO do przypisanego modelu 3D
            shipVisualModel.localRotation = Quaternion.Euler(0f, 0f, currentVisualRoll);
        }

        // ZUŻYCIE PALIWA
        if (isMoving && hasFuel)
        {
            shipStats.UseEnergy(normalDrainRate * Time.fixedDeltaTime);
        }
    }
}