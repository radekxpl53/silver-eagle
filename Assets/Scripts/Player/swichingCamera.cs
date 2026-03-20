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
        tppMovementScript.enabled = !isFpp;
        fppMovementScript.enabled = isFpp;

        tppCameraObject.SetActive(!isFpp);
        fppCameraObject.SetActive(isFpp);

        if (isFpp)
        {
            rb.constraints = RigidbodyConstraints.None;
            // Usunięte linearDamping
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            // Usunięte linearDamping i angularDamping

            Vector3 currentRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
            rb.angularVelocity = Vector3.zero;
        }
    }
}
