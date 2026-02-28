using UnityEngine;
using UnityEngine.InputSystem;

public class latanieTpp : MonoBehaviour
{
    [Header("PARAMETRY BAZOWE")]
    [SerializeField] private float baseMass = 100000f;           // Masa Statku
    [SerializeField] private float cargoCapacity = 80000f;       // Pojemność Ładowni
    [SerializeField] private float maxMainThrust = 800000f;      // Siła Ciągu Głównego
    [SerializeField] private float brakeThrust = 400000f;        // Siła Hamowania
    [SerializeField] private float maneuveringTorque = 250000f;  // Siła Manewrowa

    [Header("ILOSC LADUNKU")]
    [Range(0f, 1f)]
    public float currentLoadPercent = 0f;

    [Header("INNE")]
    [SerializeField] private float liftThrust = 30000f;
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private float previousLoadPercent = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0.5f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        UpdatePhysics();
    }

    void FixedUpdate()
    {
        // aktualizacja masy i dragów gdy zmienimy suwak w trakcie gry
        if (Mathf.Abs(currentLoadPercent - previousLoadPercent) > 0.001f)
        {
            UpdatePhysics();
            previousLoadPercent = currentLoadPercent;
        }

        handleMovment();
    }

    private void UpdatePhysics()
    {
        float totalMass = baseMass + cargoCapacity * currentLoadPercent;
        rb.mass = totalMass;

        float angularDrag = 1.5f - (1.5f - 0.9f) * currentLoadPercent;
        rb.angularDamping = angularDrag;
    }

    void handleMovment()
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

        // A. PRZÓD / HAMOWANIE 
        if (gasInput != 0)
        {
            float currentThrust = (gasInput > 0) ? maxMainThrust : brakeThrust;
            rb.AddRelativeForce(Vector3.forward * gasInput * currentThrust);
        }

        // B. SKRĘT
        if (turnInput != 0)
        {
            rb.AddRelativeTorque(Vector3.up * turnInput * maneuveringTorque);
        }

        // C. WINDA
        if (verticalInput != 0)
        {
            rb.AddForce(Vector3.up * verticalInput * liftThrust);
        }
    }
}