using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class fppLatanie : MonoBehaviour
{
    [Header("PARAMETRY BAZOWE")]
    public float baseMass = 20000f;
    public float cargoCapacity = 15000f;
    public float maxMainThrust = 120000f;

    [Header("SI£Y OBROTU")]
    public float maneuverForce = 5000f;
    public float rollForce = 30000f;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0.8f;

        // Ukrycie kursora
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //MOC SILNIKOW SCROLEM
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.y.ReadValue() * throttleSensitivity;
            currentThrottle = Mathf.Clamp(currentThrottle + scroll, 0f, 100f);
        }

        //AKTUALIZACJA FIZYKI MASY
        float currentCargoMass = cargoCapacity * currentLoadPercent;
        rb.mass = baseMass + currentCargoMass;
        rb.angularDamping = Mathf.Lerp(1.5f, 0.9f, currentLoadPercent);
    }

    void FixedUpdate()
    {
        //CI¥G SILNIKA
        float currentThrustForce = maxMainThrust * (currentThrottle / 100f);
        rb.AddRelativeForce(Vector3.forward * currentThrustForce);

        //ZMIENNE STEROWANIA
        float mouseX = 0f;
        float mouseY = 0f;
        float rollInput = 0f;

        //ODCZYT MYSZY
        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            mouseX = delta.x * mouseSensitivity * Time.fixedDeltaTime * 50f;
            mouseY = delta.y * mouseSensitivity * Time.fixedDeltaTime * 50f;
        }

        //ODCZYT KLAWIATURY (A/D - ROLL)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) rollInput = -1f;
            if (Keyboard.current.dKey.isPressed) rollInput = 1f;
        }

        //APLIKOWANIE SI£ OBROTOWYCH
        // Pitch (X): Mysz góra/dó³ (odwrócona oœ Y myszy dla sterowania samolotowego)
        float pitchForce = -mouseY * maneuverForce;

        // Yaw (Y): Mysz lewo/prawo
        float yawForce = mouseX * maneuverForce;

        // Roll (Z): Klawisze A/D (minus, ¿eby D pochyla³o w prawo)
        float rollTorque = -rollInput * rollForce;

        Vector3 rotacja = new Vector3(pitchForce, yawForce, rollTorque);
        rb.AddRelativeTorque(rotacja);
    }
}