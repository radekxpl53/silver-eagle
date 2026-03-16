using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DrillMinigame : MonoBehaviour
{
    [Header("UI Panels & Refs")]
    public GameObject miningCanvas;
    public Slider progressSlider;
    public Slider heatSlider;
    public Image heatFillImage; // Pasek zmieniający kolor
    public RectTransform optimalZoneMarker; // Wizualna strefa na sliderze

    [Header("Drill Settings")]
    public float heatIncreaseSpeed = 40f;
    public float coolDownSpeed = 25f;
    public float maxHeat = 150f;
    public float criticalErrorRate = 0.15f;

    [Header("Asteroid Data (Calculated)")]
    private float targetOptimalTemp;
    private float currentTolerance;
    private float currentMaxScale;
    
    [Header("Current State")]
    private float currentHeat = 0f;
    private float currentProgress = 0f;
    private float instability = 0f;
    private float yieldMultiplier = 1f;
    private bool isMining = false;
    private bool isOverheated = false;

    [Header("Audio")]
    [SerializeField] private EventReference successSfx;
    private EventInstance laserCollecting;

    private bool isPressingAction => Keyboard.current.spaceKey.isPressed || Pointer.current.press.isPressed;

    void Start()
    {
        laserCollecting = RuntimeManager.CreateInstance(FMODEvents.instance.laserCollecting);
        
        if (MiningData.currentAsteroidObject != null)
        {
            StartMinigame();
        }
        else
        {
            Debug.LogWarning("Brak danych asteroidy! Powrót.");
            SceneManager.LoadScene("GameManager");
        }
    }

    public void StartMinigame()
    {
        isMining = true;
        miningCanvas.SetActive(true);
        
        Asteroid asteroidScript = MiningData.currentAsteroidObject.GetComponent<Asteroid>();
        if (asteroidScript != null)
        {
            targetOptimalTemp = asteroidScript.CalculateTemperature();
            currentTolerance = asteroidScript.ToleranceTemperature();
            
            currentMaxScale = targetOptimalTemp + (currentTolerance * 5f);
            
            heatIncreaseSpeed = currentMaxScale / 3f; 
            coolDownSpeed = heatIncreaseSpeed * 0.75f;
            
            SetupOptimalZoneUI();
        }

        currentProgress = 0.1f; // Startowy postęp
        instability = 0f;
        yieldMultiplier = 1f;
    }

    void Update()
    {
        if (!isMining) return;

        if (isOverheated)
        {
            HandleOverheat();
        }
        else
        {
            HandleDrilling();
        }

        UpdateUI();
        UpdateAudio();
    }

    void HandleDrilling()
    {
        if (isPressingAction)
        {
            currentHeat += heatIncreaseSpeed * Time.deltaTime;

            // Logika różnicy temperatur
            float diff = Mathf.Abs(currentHeat - targetOptimalTemp);

            if (diff <= currentTolerance)
            {
                // Gracz w strefie - bonus do postępu, spadek niestabilności
                currentProgress += 0.15f * Time.deltaTime;
                instability = Mathf.Max(0, instability - 0.05f * Time.deltaTime);
            }
            else
            {
                // Gracz poza strefą - wolny postęp, wzrost niestabilności
                currentProgress += 0.03f * Time.deltaTime;
                float errorScale = (diff - currentTolerance) / 20f;
                instability += criticalErrorRate * errorScale * Time.deltaTime;
            }
        }
        else
        {
            currentHeat -= coolDownSpeed * Time.deltaTime;
        }

        currentHeat = Mathf.Clamp(currentHeat, 0, maxHeat);

        // Sprawdzanie warunków końcowych
        if (instability >= 0.9f || currentHeat >= maxHeat)
        {
            ThermalShock();
        }
        else if (currentProgress >= 1f)
        {
            EndGame("WYDOBYTO!");
        }
    }

    void HandleOverheat()
    {
        currentHeat -= coolDownSpeed * 1.5f * Time.deltaTime;
        if (currentHeat <= 0)
        {
            currentHeat = 0;
            isOverheated = false;
        }
    }

    void UpdateAudio()
    {
        if (isPressingAction && !isOverheated)
        {
            PLAYBACK_STATE state;
            laserCollecting.getPlaybackState(out state);
            if (state != PLAYBACK_STATE.PLAYING) laserCollecting.start();

            // Zmiana Pitch w zależności od temperatury (GTA Style)
            float pitch = 0.7f + (currentHeat / maxHeat) * 0.8f;
            laserCollecting.setPitch(pitch);
        }
        else
        {
            laserCollecting.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    void UpdateUI()
    {
        progressSlider.value = currentProgress;
        heatSlider.value = currentHeat;

        // Kolor paska temperatury
        float heatPercent = currentHeat / maxHeat;
        heatFillImage.color = Color.Lerp(Color.cyan, Color.red, heatPercent);

        // Efekt drżenia przy wysokiej niestabilności
        if (instability > 0.5f)
        {
            float shake = (instability - 0.5f) * 10f;
            miningCanvas.transform.localPosition = Random.insideUnitSphere * shake;
        }
    }

    void SetupOptimalZoneUI()
    {
        float sliderWidth = heatSlider.GetComponent<RectTransform>().rect.width;
        float normalizedPos = targetOptimalTemp / currentMaxScale;
        float normalizedWidth = (currentTolerance * 2) / currentMaxScale;
        float xPos = (normalizedPos * sliderWidth) - (sliderWidth / 2f);
        optimalZoneMarker.anchoredPosition = new Vector2(xPos, 0);
        optimalZoneMarker.sizeDelta = new Vector2(normalizedWidth * sliderWidth, optimalZoneMarker.sizeDelta.y);
    }

    void ThermalShock()
    {
        Debug.Log("THERMAL SHOCK!");
        EndGame("ASTEROIDA ROZWALONA");
    }

    void EndGame(string message)
    {
        Debug.Log(message);
        isMining = false;
        
        if (laserCollecting.isValid())
        {
            laserCollecting.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            RuntimeManager.PlayOneShot(successSfx, transform.position);
        }

        if (message == "WYDOBYTO!") {
            PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
            
            if (inventory != null && MiningData.currentAsteroidLoot != null) 
            {
                foreach (ResourceStack stack in MiningData.currentAsteroidLoot) 
                {
                    // Obliczamy ile faktycznie udało się odzyskać (zaokrąglamy w górę)
                    int finalAmount = Mathf.CeilToInt(stack.amount * yieldMultiplier);
                    
                    if (finalAmount > 0)
                    {
                        inventory.AddResource(stack.definition, finalAmount);
                        Debug.Log($"Dodano do ekwipunku: {stack.definition.Name} x{finalAmount} (Efektywność: {yieldMultiplier*100}%)");
                    }
                    
                }
                MiningData.currentAsteroidLoot.Clear();
                if (MiningData.currentManager != null) {
                    Debug.Log("DEBUG: Informuję przekaźnik o wydobyciu");
                    MiningData.currentManager.OnObjectInteracted(MiningData.currentArea, MiningData.currentBelt);
                }
                Destroy(MiningData.currentAsteroidObject.gameObject);
                Debug.Log("Obiekt asteroidy usunięty z głównej sceny");
            }
        }

        // Wywalamy dane z przekaźnika
        MiningData.currentAsteroidObject = null;
        MiningData.currentAsteroidLoot = null;
        MiningData.currentManager = null;
        MiningData.currentArea = null;
        MiningData.currentBelt = null;

        // Przełącamy sinlgetona na eksporacje i wywalamy scene z miningu
        GameManager.Instance.ChangeState(GameState.Exploration);
        SceneManager.UnloadSceneAsync("MiningScene");
    }



    void OnDestroy()
    {
        if (laserCollecting.isValid())
        {
            laserCollecting.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            laserCollecting.release();
        }
    }
}