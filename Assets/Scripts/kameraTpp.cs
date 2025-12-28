using UnityEngine;
using UnityEngine.InputSystem;

public class kameraTpp : MonoBehaviour
{
    public Transform target;

    [Header("Ustawienia Orbity")]
    public float distance = 15.0f;
    public float sensitivity = 0.5f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    private float mouseX = 0.0f;
    private float mouseY = 0.0f;


    private float currentX = 0.0f;
    private float currentY = 0.0f;

    private float smoothSpeed = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            // Nowy system zwraca pixele, skalujemy lekko
            mouseX += mouseDelta.x * sensitivity * 0.5f;
            mouseY -= mouseDelta.y * sensitivity * 0.5f;

            // Ograniczenie góra/dó³ (¿eby nie zrobiæ fiko³ka kamer¹)
            mouseY = Mathf.Clamp(mouseY, yMinLimit, yMaxLimit);

            currentX = Mathf.Lerp(currentX, mouseX, smoothSpeed * Time.deltaTime);
            currentY = Mathf.Lerp(currentY, mouseY, smoothSpeed * Time.deltaTime);
        }

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Pozycja: Punkt celu - (kierunek * dystans)
        // Dodajemy Vector3.up * 2, ¿eby kamera patrzy³a nieco nad œrodek statku
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        transform.rotation = rotation;
        transform.position = position;
    }
}
