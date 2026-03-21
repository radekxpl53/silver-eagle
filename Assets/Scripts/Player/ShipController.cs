using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(ShipStats))]
public class ShipController : MonoBehaviour
{
    [Header("KAMERY")]
    [SerializeField] private bool isFPPMode = true;
    [SerializeField] private GameObject tppCameraObject;
    [SerializeField] private GameObject fppCameraObject;

    [Header("TRYB LOTU")]
    [SerializeField] private bool flightAssist = true;
    [SerializeField] private float autoBrakeStrength = 2.0f;

    [Header("STEROWANIE")]
    [SerializeField] private float fppMouseSensitivity = 0.5f;
    [SerializeField] private float verticalAccelerationTime = 0.75f;

    [Header("WIZUALNY PRZECHYL")]
    [SerializeField] private Transform shipVisualModel;
    [SerializeField] private float maxRollAngle = 15f;
    [SerializeField] private float rollSmoothSpeed = 5f;

    private Rigidbody rb;
    private ShipStats stats;

    private float currentVerticalThrust = 0f;
    private float verticalVelocityRef = 0f;

    private float currentVisualRoll = 0f;
    private float previousLoadPercent = -1f;
    private bool lowFuelWarningTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<ShipStats>();

        rb.useGravity = false;

        ApplyCameraMode();
        UpdatePhysics();
    }

    void Update()
    {
        // 1. Zmiana kamery
        if (Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame)
        {
            isFPPMode = !isFPPMode;
            ApplyCameraMode();
            UpdatePhysics();
        }

        // 2. Zmiana trybu lotu
        if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame)
        {
            flightAssist = !flightAssist;
            UpdatePhysics();
        }

        if (GameManager.Instance != null && (GameManager.Instance.currentState == GameState.Exploration || GameManager.Instance.currentState == GameState.Fighting || GameManager.Instance.currentState == GameState.Mining))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        // 3. Obsługa wagi i ładunku 
        float currentLoadPercent = stats.GetMaxCargo() > 0 ? stats.CurrentCargo / stats.GetMaxCargo() : 0f;
        if (Mathf.Abs(currentLoadPercent - previousLoadPercent) > 0.001f)
        {
            UpdatePhysics();
            previousLoadPercent = currentLoadPercent;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.currentState != GameState.Exploration &&
            GameManager.Instance.currentState != GameState.Fighting)
        {
            return;
        }

        HandleMovement();
        CheckFuelWarning();
        ApplyDirectionalDamping();
    }

    private void ApplyCameraMode()
    {
        if (tppCameraObject != null) tppCameraObject.SetActive(!isFPPMode);
        if (fppCameraObject != null) fppCameraObject.SetActive(isFPPMode);

        if (!isFPPMode)
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void UpdatePhysics()
    {
        rb.mass = stats.GetTotalMass();

        float loadRatio = stats.GetMaxCargo() > 0 ? stats.CurrentCargo / stats.GetMaxCargo() : 0f;

        rb.angularDamping = Mathf.Lerp(1.5f, 0.9f, loadRatio);
        rb.linearDamping = 0f;

        if (isFPPMode)
            rb.constraints = RigidbodyConstraints.None;
        else
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Asystent lotu tylko dla lotu naprzod
    private void ApplyDirectionalDamping()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);

        float loadRatio = stats.GetMaxCargo() > 0 ? stats.CurrentCargo / stats.GetMaxCargo() : 0f;
        float loadMultiplier = Mathf.Lerp(1f, 0.1f, loadRatio);
        float currentDrag = autoBrakeStrength * loadMultiplier;

        float dragX = localVelocity.x * currentDrag;
        float dragY = localVelocity.y * currentDrag;

        float dragZ = flightAssist ? (localVelocity.z * currentDrag) : 0f;

        rb.AddRelativeForce(new Vector3(-dragX, -dragY, -dragZ), ForceMode.Acceleration);
    }

    void HandleMovement()
    {
        float gasInput = 0f;
        float turnInput = 0f;
        float rollInput = 0f;
        float verticalInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) gasInput = 1f;
            if (Keyboard.current.sKey.isPressed) gasInput = -1f;

            if (Keyboard.current.aKey.isPressed) turnInput = -1f;
            if (Keyboard.current.dKey.isPressed) turnInput = 1f;

            if (Keyboard.current.qKey.isPressed) rollInput = 1f;
            if (Keyboard.current.eKey.isPressed) rollInput = -1f;

            if (Keyboard.current.spaceKey.isPressed) verticalInput = 1f;
            if (Keyboard.current.leftShiftKey.isPressed) verticalInput = -1f;
        }

        bool hasFuel = stats.CurrentEnergy > 0f;
        float currentPerformance = hasFuel ? 1f : stats.EmergencySpeedMultiplier;

        if (gasInput != 0)
        {
            float baseThrust = (gasInput > 0) ? stats.MaxMainThrust : stats.BrakeThrust;
            rb.AddRelativeForce(Vector3.forward * gasInput * baseThrust * currentPerformance);
        }

        float targetVerticalThrust = verticalInput * stats.LiftThrust * currentPerformance;
        currentVerticalThrust = Mathf.SmoothDamp(currentVerticalThrust, targetVerticalThrust, ref verticalVelocityRef, verticalAccelerationTime);

        if (Mathf.Abs(currentVerticalThrust) > 10f)
        {
            rb.AddRelativeForce(Vector3.up * currentVerticalThrust);
        }

        if (isFPPMode)
        {
            float mouseX = 0f, mouseY = 0f;
            if (Mouse.current != null)
            {
                Vector2 delta = Mouse.current.delta.ReadValue();
                mouseX = delta.x * fppMouseSensitivity * Time.fixedDeltaTime * 50f;
                mouseY = delta.y * fppMouseSensitivity * Time.fixedDeltaTime * 50f;
            }

            float pitchForce = -mouseY * stats.ManeuverForce * currentPerformance;
            float yawForce = mouseX * stats.ManeuverForce * currentPerformance;
            float rollTorque = -rollInput * stats.RollForce * currentPerformance;

            rb.AddRelativeTorque(new Vector3(pitchForce, yawForce, rollTorque));
        }
        else
        {
            if (turnInput != 0)
            {
                rb.AddTorque(Vector3.up * turnInput * stats.ManeuverForce * currentPerformance);
            }

            float targetRoll = -rollInput * maxRollAngle;
            currentVisualRoll = Mathf.Lerp(currentVisualRoll, targetRoll, Time.fixedDeltaTime * rollSmoothSpeed);

            if (shipVisualModel != null)
            {
                shipVisualModel.localRotation = Quaternion.Euler(0f, 0f, currentVisualRoll);
            }
        }

        bool isMoving = gasInput != 0 || turnInput != 0 || verticalInput != 0 || rollInput != 0 || (isFPPMode && Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f);
        if (isMoving && hasFuel)
        {
            stats.UseEnergy(stats.NormalDrainRate * Time.fixedDeltaTime);
        }
    }

    private void CheckFuelWarning()
    {
        if (stats.CurrentEnergy <= stats.LowFuelThreshold && !lowFuelWarningTriggered && stats.CurrentEnergy > 0)
        {
            lowFuelWarningTriggered = true;
            Debug.LogWarning("<color=red><b>UWAGA: Niski poziom paliwa!</b></color>");
        }
        else if (stats.CurrentEnergy > stats.LowFuelThreshold && lowFuelWarningTriggered)
        {
            lowFuelWarningTriggered = false;
        }
    }
}