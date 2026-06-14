using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiningGame : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject miningCanvas;

    [Header("UI References")]
    public Slider temperatureSlider;
    public Slider progressSlider;
    public Image sliderFillImage;
    public RectTransform sweetSpotIndicator;

    [Header("Drill Settings")]
    public float maxDrillTemperature = 2500f;
    public float heatgainSpeed = 650f;
    public float coolDownSpeed = 1000f;
    public float progressSpeed = 0.1f;
    public float overheatPenaltyTime = 2f;

    [Header("Movement Settings")]
    public float driftSpeed = 50f; // Prędkość przesuwania się strefy
    private int driftDirection = 1; // 1 to w górę, -1 to w dół

    [Header("Asteroid Physics")]
    private float targetTemp;   // Pobierane z CalculateTemperature()
    private float tolerance;    // Pobierane z ToleranceTemperature()
    private float minOptimal;   // Dolna granica sweet spotu
    private float maxOptimal;   // Górna granica sweet spotu

    [Header("Values")]
    private float currentTemperature = 0f;
    private float currentProgress = 0f;
    private bool isOverheated = false;
    private float overheatTimer = 0f;
    private float yieldMultiplier = 1f;
    private bool isMining = false;

    private float instability = 0f;
    private bool thermalShockTriggered;
    private bool isDataInitialized = false;

    [Header("Asteroid Explosion")]
    [SerializeField] private GameObject explosionPrefab;


    private bool isPressingAction => Keyboard.current.spaceKey.isPressed || Pointer.current.press.isPressed;
    
    [Header("Audio")]
    [SerializeField] private EventReference successSfx;
    private EventInstance laserCollecting;
    
    void Update()
    {
        if (isMining)
        {
            HandleMining();
            UpdateSweetSpotPosition();
        }

    }

    void HandleMining()
    {   
        if (!isMining || !isDataInitialized) return;

        if (isOverheated)
        {
            HandleOverheat();
            return;
        }

        UpdateInstability();

        if (isPressingAction)
        {
            currentTemperature += heatgainSpeed * Time.deltaTime;

            PLAYBACK_STATE state;
            laserCollecting.getPlaybackState(out state);
            if (state != PLAYBACK_STATE.PLAYING) laserCollecting.start();
        }
        else
        {
           currentTemperature -= coolDownSpeed * Time.deltaTime;
           laserCollecting.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        currentTemperature = Mathf.Clamp(currentTemperature, 0, maxDrillTemperature);

        // normalizacja temperatury 
        float tempNormalized = currentTemperature / maxDrillTemperature;
        temperatureSlider.value = tempNormalized;


        if (currentTemperature >= minOptimal && currentTemperature <= maxOptimal)
        {

            currentProgress += progressSpeed * Time.deltaTime;
            
            if (sliderFillImage != null)
                sliderFillImage.color = Color.cyan; // Kolor sygnalizujący wiercenie
        }
        else if (currentTemperature > maxOptimal)
        {
            if (sliderFillImage != null) sliderFillImage.color = Color.red;
        }
        else
        {
            if (sliderFillImage != null) sliderFillImage.color = Color.white;
        }

        if (instability >= 10f && instability < 90f)
        {
            float meltRate = (instability - 10f) / 80f;
            yieldMultiplier -= 0.12f * meltRate * Time.deltaTime;
            if (yieldMultiplier < 0.3f)
            {
                EndGame("ZŁOŻE ZNISZCZONE - TOPNIENIE SUROWCA!");
                return;
            }
        }

        currentProgress = Mathf.Clamp01(currentProgress);
        progressSlider.value = currentProgress;

        yieldMultiplier = Mathf.Clamp(yieldMultiplier, 0.0f, 1f);
        
        // Przegrzanie wiertła
        if (currentTemperature >= maxDrillTemperature)
            TriggerOverheat();

        if (currentProgress >= 1f)
            EndGame("WYDOBYTO!");
    }

    void TriggerOverheat()
    {
        isOverheated = true;
        overheatTimer = overheatPenaltyTime;
        laserCollecting.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Debug.Log("PRZEGRZANIE! Czekaj na schłodzenie...");
    }

    void HandleOverheat()
    {
        overheatTimer -= Time.deltaTime;

        currentTemperature -= (maxDrillTemperature / overheatPenaltyTime) * Time.deltaTime;
        currentTemperature = Mathf.Max(currentTemperature, 0);
        temperatureSlider.value = currentTemperature / maxDrillTemperature;

        if(overheatTimer <= 0)
        {
            isOverheated = false;
        }
    }
public void StartMinigame() {
    if (MiningData.currentAsteroidObject != null)
    {
        Asteroid asteroid = MiningData.currentAsteroidObject;
        
        float rawTarget = asteroid.CalculateTemperature();
        float rawTolerance = asteroid.ToleranceTemperature();

        // LIMIT 15% PASKA (7.5% w każdą stronę)
        float maxAllowedTolerance = maxDrillTemperature * 0.075f; 
        float finalTolerance = Mathf.Min(rawTolerance, maxAllowedTolerance);

        // Zabezpieczenie przed wychodzeniem poza skalę 
        targetTemp = Mathf.Clamp(rawTarget, finalTolerance, maxDrillTemperature - finalTolerance);


        minOptimal = targetTemp - finalTolerance;
        maxOptimal = targetTemp + finalTolerance;
        
        if (sweetSpotIndicator != null)
        {
            float startAnchor = minOptimal / maxDrillTemperature;
            float endAnchor = maxOptimal / maxDrillTemperature;
            
            sweetSpotIndicator.anchorMin = new Vector2(startAnchor, 0);
            sweetSpotIndicator.anchorMax = new Vector2(endAnchor, 1);

            sweetSpotIndicator.offsetMin = Vector2.zero;
            sweetSpotIndicator.offsetMax = Vector2.zero;
        }

        isDataInitialized = true;
    } 
    else
    {
        Debug.LogError("BŁĄD: Próba startu bez obiektu asteroidy!");
        EndGame("BŁĄD DANYCH");
        return;
    }

    isMining = true;
    miningCanvas.SetActive(true);
    currentProgress = 0f;
    currentTemperature = 0f;
    yieldMultiplier = 1f;
    instability = 0f;
    thermalShockTriggered = false;

    if (laserCollecting.isValid()) laserCollecting.start();
}



    void CheckWinCondition()
    {
        // Aktualizacja Sliderów w UI
        progressSlider.value = currentProgress;
        temperatureSlider.value = currentTemperature;

       
        if (isOverheated) {
            yieldMultiplier -= 0.05f * Time.deltaTime; // Powolny spadek jakości przy awarii
        }

        yieldMultiplier = Mathf.Clamp(yieldMultiplier, 0.1f, 1f);

        // Warunek Wygranej
        if (currentProgress >= 1f) {
            EndGame("WYDOBYTO!");
        }

        // Opcjonalnie: Warunek przegranej (np. jeśli yield spadnie do zera lub czas minie)
        if (yieldMultiplier <= 0.1f) {
            EndGame("ZŁOŻE ZNISZCZONE!");
        }
    }

    void UpdateInstability()
    {
        bool inZone = currentTemperature >= minOptimal && currentTemperature <= maxOptimal;

        if (!inZone)
            instability += 10f * Time.deltaTime;

        if (currentTemperature > maxOptimal * 1.2f)
            instability += 30f * Time.deltaTime;
        else if (inZone)
            instability = Mathf.Max(0f, instability - 5f * Time.deltaTime);

        instability = Mathf.Clamp(instability, 0f, 100f);

        if (instability >= 90f && !thermalShockTriggered)
        {
            thermalShockTriggered = true;
            TriggerThermalShock();
        }
    }

    void TriggerThermalShock()
    {
        ThermalShock();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            ShipStats stats = player.GetComponent<ShipStats>();
            DamageCollision collision = player.GetComponentInChildren<DamageCollision>();
            Vector3 astPos = MiningData.currentAsteroidObject != null
                ? MiningData.currentAsteroidObject.transform.position
                : player.transform.position;
            float dist = Vector3.Distance(player.transform.position, astPos);
            float damage = Mathf.Lerp(35f, 8f, dist / 400f);

            if (stats != null)
                stats.AbsorbDamage(damage, player.transform.position);
            else if (collision != null)
                collision.SendMessage("OnCollisionEnter", new Collision(), SendMessageOptions.DontRequireReceiver);

            GameEvents.TriggerHullDamaged(damage, player.transform.position);
        }

        EndGame("THERMAL SHOCK — PRZERWANO WYDOBYWANIE!");
    }

    void ShowSampleAnalysis(Asteroid asteroid)
    {
        if (asteroid == null || asteroid.materials == null || asteroid.materials.Count == 0) return;

        float sum = 0f;
        foreach (var m in asteroid.materials) sum += m.amount;
        float avgTemp = asteroid.CalculateTemperature();
        string band = avgTemp < 1500f ? "LOW" : avgTemp < 2200f ? "MID" : "HIGH";

        Debug.Log("=== ANALIZA PRÓBKI ===");
        foreach (var m in asteroid.materials)
            Debug.Log($"  {m.definition.Name}: {(m.amount / sum) * 100f:F0}%");
        Debug.Log($"  Średnia temp: {avgTemp:F0}°C | Strefa: {band}");
    }

    void ThermalShock()
    {
        Debug.Log("Wiertło zablokowane - System chłodzenia aktywny!");
        
        if (sliderFillImage != null) {
            sliderFillImage.color = Color.white; 
        }

        // Wstrzymujemy dźwięk wiercenia
        laserCollecting.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        
        currentProgress -= 0.05f; 
        currentProgress = Mathf.Clamp01(currentProgress);
    }

    void Start() {
        laserCollecting = RuntimeManager.CreateInstance(FMODEvents.instance.laserCollecting);
        
        if (MiningData.currentAsteroidLoot != null)
        {
            if (MiningData.currentAsteroidObject != null)
                ShowSampleAnalysis(MiningData.currentAsteroidObject.GetComponent<Asteroid>());
            StartMinigame();
        }
        else {
            Debug.LogWarning("Brak danych o asteroidzie! Wracam do głównej sceny.");
            SceneManager.LoadScene("GameManager");
        }
    }

    void UpdateSweetSpotPosition()
    {
        if (!isMining || isOverheated) return;

        float movement = driftSpeed * driftDirection * Time.deltaTime;
        targetTemp += movement;

        // 2. Odbijanie od krawędzi (z uwzględnieniem tolerancji, żeby strefa nie wystawała)
        float currentTolerance = (maxOptimal - minOptimal) / 2f;
        
        if (targetTemp + currentTolerance >= maxDrillTemperature)
        {
            driftDirection = -1; // Zmień kierunek na dół
        }
        else if (targetTemp - currentTolerance <= 0)
        {
            driftDirection = 1; // Zmień kierunek na górę
        }

        // 3. Aktualizacja granic optymalnych
        minOptimal = targetTemp - currentTolerance;
        maxOptimal = targetTemp + currentTolerance;

        // 4. Aktualizacja w UI
        if (sweetSpotIndicator != null)
        {
            float startAnchor = minOptimal / maxDrillTemperature;
            float endAnchor = maxOptimal / maxDrillTemperature;

            sweetSpotIndicator.anchorMin = new Vector2(startAnchor, 0);
            sweetSpotIndicator.anchorMax = new Vector2(endAnchor, 1);
        }
    }

    void EndGame(string message)
    {
        
        Debug.Log(message);
        isMining = false;

        if (message == "WYDOBYTO!") {

            
            string summary = "WYDOBYTO:";
            if (MiningData.currentAsteroidLoot != null) {
                foreach (ResourceStack stack in MiningData.currentAsteroidLoot) {
                    int finalAmount = Mathf.CeilToInt(stack.amount * yieldMultiplier);
                    if (finalAmount > 0) summary += $"\n+ {finalAmount} {stack.definition.Name}";
                }
            }
            GameManager.Instance.ShowMiningNotification(summary, Color.green);

            PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();

            if (laserCollecting.isValid())
            {
                laserCollecting.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                Debug.Log("Zatrzymano dźwięk wydobywania i odtworzono dźwięk sukcesu ");

                RuntimeManager.PlayOneShot(successSfx);
            }

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

                if (MiningData.currentAsteroidObject != null && explosionPrefab != null)
                {
                    GameObject explosion = Instantiate(
                        explosionPrefab,
                        MiningData.currentAsteroidObject.transform.position,
                        MiningData.currentAsteroidObject.transform.rotation
                    );

                    Scene asteroidScene = MiningData.currentAsteroidObject.gameObject.scene;
                    SceneManager.MoveGameObjectToScene(explosion, asteroidScene);
                }

                Destroy(MiningData.currentAsteroidObject.gameObject);
                Debug.Log("Obiekt asteroidy usunięty z głównej sceny");
            }
        } else
        {
            GameManager.Instance.ShowMiningNotification(message, Color.red);
        }

        // Wywalamy dane z przekaźnika
        MiningData.currentAsteroidObject = null;
        MiningData.currentAsteroidLoot = null;
        MiningData.currentManager = null;
        MiningData.currentArea = null;
        MiningData.currentBelt = null;

        // Przełącamy sinlgetona na eksporacje i wywalamy scerne z miningu
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