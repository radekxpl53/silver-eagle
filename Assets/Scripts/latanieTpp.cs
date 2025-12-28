using UnityEngine;
using UnityEngine.InputSystem;

public class latanieTpp : MonoBehaviour
{

    [Header("PARAMETRY BAZOWE")]
    [SerializeField] private float baseMass = 20000f;
    [SerializeField] private float cargoCapacity = 15000f;
    [SerializeField] private float maxMainThrust = 120000f;

    [Header("ILOSC LADUNKU")]
    [Range(0f, 1f)]
    public float currentLoadPercent = 0f;

    [Header("moc silnikow")]
    [SerializeField]private float forwardSpeed = 50f;
    [SerializeField] private float turnSpeed = 25f;
    [SerializeField] private float liftSpeed = 30f;

    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        rb.linearDamping = 2.0f;
        rb.angularDamping = 5.0f;

        //blokowanie obrocenia na boki
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        handleMovment();
    }

    void handleMovment()
    {
        // 1. ODCZYT KLAWIISZY (WASD)
        float gasInput = 0f;  // O� Z (Prz�d/Ty�)
        float turnInput = 0f; // O� Y (Lewo/Prawo)

        if (Keyboard.current != null)
        {
            // W / S - Gaz do przodu/ty�u
            if (Keyboard.current.wKey.isPressed) gasInput = 1f;
            if (Keyboard.current.sKey.isPressed) gasInput = -1f;

            // A / D - Skr�t (Obr�t statku)
            if (Keyboard.current.aKey.isPressed) turnInput = -1f;
            if (Keyboard.current.dKey.isPressed) turnInput = 1f;
        }

        // 2. ODCZYT G�RA / Dӣ (Winda)
        float verticalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.isPressed) verticalInput = 1f;
            if (Keyboard.current.leftShiftKey.isPressed) verticalInput = -1f;
        }

        // --- FIZYKA LOTU ---

        // A. RUCH DO PRZODU (Zawsze zgodnie z dziobem statku)
        // AddRelativeForce u�ywa lokalnych osi statku
        if (gasInput != 0)
        {
            rb.AddRelativeForce(Vector3.forward * gasInput * forwardSpeed);
        }

        // B. SKR�CANIE (Obr�t wok� osi Y)
        // AddRelativeTorque obraca statek
        if (turnInput != 0)
        {
            rb.AddRelativeTorque(Vector3.up * turnInput * turnSpeed);
        }

        // C. WINDA (G�ra / D�)
        // U�ywamy Vector3.up (Globalne), �eby statek wznosi� si� "do nieba", a nie "nad sufit kokpitu"
        if (verticalInput != 0)
        {
            rb.AddForce(Vector3.up * verticalInput * liftSpeed);
        }
    }
}
