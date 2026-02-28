using UnityEngine;
using UnityEngine.InputSystem;

public class swichingCamera : MonoBehaviour
{
    [Header("SKRYPTY STEROWANIA")]
    [SerializeField] private latanieTpp tppMovementScript;
    [SerializeField] private fppLatanie fppMovementScript;

    [Header("KAMERY (Obiekty)")]
    [SerializeField] private GameObject tppCameraObject; // Obiekt z kamerą i skryptem kameraTpp
    [SerializeField] private GameObject fppCameraObject; // Obiekt z kamerą z perspektywy kokpitu

    [Header("USTAWIENIA POCZĄTKOWE")]
    [SerializeField] private bool startInFPP = false;

    private Rigidbody rb;
    private bool isCurrentlyFPP;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        isCurrentlyFPP = startInFPP;
        ApplyMode(isCurrentlyFPP);
    }

    void Update()
    {
        // Sprawdzamy wciśnięcie klawisza V
        if (Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame)
        {
            isCurrentlyFPP = !isCurrentlyFPP;
            ApplyMode(isCurrentlyFPP);
        }
    }

    private void ApplyMode(bool isFpp)
    {
        // 1. Przełączanie Skryptów
        tppMovementScript.enabled = !isFpp;
        fppMovementScript.enabled = isFpp;

        // 2. Przełączanie Kamer
        tppCameraObject.SetActive(!isFpp);
        fppCameraObject.SetActive(isFpp);

        // 3. Resetowanie ustawień fizyki
        if (isFpp)
        {
            // --- USTAWIENIA DLA FPP ---
            rb.constraints = RigidbodyConstraints.None; // Odblokowujemy pełen obrót
            rb.linearDamping = 0.8f;
        }
        else
        {
            // --- USTAWIENIA DLA TPP ---
            // Blokujemy obrót, żeby statek latał prosto
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.linearDamping = 2.0f;
            rb.angularDamping = 5.0f;

            // Zerujemy przechył statku po wyjściu z FPP żeby nie został zablokowany krzywo
            Vector3 currentRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);

            // wyzerowanie pędu obrotowego, żeby statkiem nie trzęsło przy przejściu
            rb.angularVelocity = Vector3.zero;
        }
    }
}
