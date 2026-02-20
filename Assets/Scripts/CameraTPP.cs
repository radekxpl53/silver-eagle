using UnityEngine;
using UnityEngine.InputSystem;

public class kameraTpp : MonoBehaviour
{
    public Transform target;

    [Header("Ustawienia Orbity")]
    [SerializeField] private float distance = 15.0f;
    [SerializeField] private float sensitivity = 0.5f;
    [SerializeField] private float yMinLimit = -20f;
    [SerializeField] private float yMaxLimit = 80f;

    [SerializeField] private float mouseX = 0.0f;
    [SerializeField] private float mouseY = 0.0f;


    [SerializeField] private float currentX = 0.0f;
    [SerializeField] private float currentY = 0.0f;

    [SerializeField] private float smoothSpeed = 10f;

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

            mouseX += mouseDelta.x * sensitivity * 0.5f;
            mouseY -= mouseDelta.y * sensitivity * 0.5f;

            // Ograniczenie góra/dó³ (¿eby nie zrobiæ fiko³ka kamer¹)
            mouseY = Mathf.Clamp(mouseY, yMinLimit, yMaxLimit);

            currentX = Mathf.Lerp(currentX, mouseX, smoothSpeed * Time.deltaTime);
            currentY = Mathf.Lerp(currentY, mouseY, smoothSpeed * Time.deltaTime);
        }

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Pozycja: Punkt celu - (kierunek * dystans)
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        transform.rotation = rotation;
        transform.position = position;
    }
}
