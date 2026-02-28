using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class fppLatanie : MonoBehaviour
{
    [Header("PARAMETRY BAZOWE")]
    public float baseMass = 100000f;          // Masa Statku
    public float cargoCapacity = 80000f;      // Pojemność Ładowni
    public float maxMainThrust = 800000f;     // Siła Ciągu Głównego

    [Header("SIŁY HAMOWANIA I MANEWRÓW")]
    public float brakeThrust = 400000f;       // Siła Hamowania
    public float maneuverForce = 250000f;     // Siła Manewrowa
    public float rollForce = 250000f;         // Roll

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
    private float previousLoadPercent = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0.5f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdatePhysics();
    }

    void Update()
    {
        if (GameManager.Instance.currentState != GameState.Exploration) return;
        // === MOC SILNIKÓW ===
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
        // === CIĄG GŁÓWNY ===
        float currentThrustForce = maxMainThrust * (currentThrottle / 100f);
        rb.AddRelativeForce(Vector3.forward * currentThrustForce);

        // === HAMOWANIE ===
        if (Keyboard.current != null && Keyboard.current.sKey.isPressed)
        {
            rb.AddRelativeForce(Vector3.forward * -brakeThrust);
        }

        // === STEROWANIE MYSZĄ + ROLL ===
        float mouseX = 0f;
        float mouseY = 0f;
        float rollInput = 0f;

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

        // Pitch
        float pitchForce = -mouseY * maneuverForce;
        // Yaw
        float yawForce = mouseX * maneuverForce;
        // Roll
        float rollTorque = -rollInput * rollForce;

        rb.AddRelativeTorque(new Vector3(pitchForce, yawForce, rollTorque));
    }
}