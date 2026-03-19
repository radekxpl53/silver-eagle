using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem; // Wymagane dla nowego systemu
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
    

    [Header("Drill Settings")]
    public float heatgainSpeed = 0.5f;
    public float coolDownSpeed = 0.3f;
    public float progressSpeed = 0.2f;
    public float overheatPenaltyTime = 2f;

    [Header("Values")]
    private float currentTemperature = 0f;
    private float currentProgress = 0f;
    private bool isOverheated = false;
    private float overheatTimer = 0f;
    private float yieldMultiplier = 1f;
    private bool isMining = false;

    [Header("Asteroid Explosion")]
    [SerializeField] private GameObject explosionPrefab;

    // Referencja do akcji (możesz ją przypisać w inspektorze lub użyć Keyboard.current)
    private bool isPressingAction => Keyboard.current.spaceKey.isPressed || Pointer.current.press.isPressed;
    
    [Header("Audio")]
    [SerializeField] private EventReference successSfx;
    private EventInstance laserCollecting;

    void Update()
    {
        if (isMining)
        {
            HandleMining();
        }

    }

    void HandleMining()
    {   
        if (isOverheated)
        {
            HandleOverheat();
            return;
        }

        // 1. Logika Paska Gracza przy użyciu New Input System
        if (isPressingAction)
        {
            currentTemperature += heatgainSpeed * Time.deltaTime;
            // Im cieplej, tym szybciej wiercisz (bonus za ryzyko)
            float heatBonus = Mathf.Lerp(0.6f, 1.4f, currentTemperature); 
            currentProgress += progressSpeed * heatBonus * Time.deltaTime;
        }
        else
        {
           currentTemperature -= coolDownSpeed * Time.deltaTime;
        }

        currentTemperature = Mathf.Clamp01(currentTemperature);
        currentProgress = Mathf.Clamp01(currentProgress);
        yieldMultiplier = Mathf.Clamp(yieldMultiplier, 0.1f, 1f);

        temperatureSlider.value = currentTemperature;
        progressSlider.value = currentProgress;

        if (sliderFillImage != null)
            sliderFillImage.color = Color.Lerp(Color.green, Color.red, currentTemperature);

        if (currentTemperature >= 1f) TriggerOverheat();
        if (currentProgress >= 1f) EndGame("WYDOBYTO!");
    }

    void TriggerOverheat()
    {
        isOverheated = true;
        overheatTimer = overheatPenaltyTime;
        Debug.Log("PRZEGRZANIE! Czekaj na schłodzenie...");
    }

    void HandleOverheat()
    {
        overheatTimer -= Time.deltaTime;
        currentTemperature -= (1f / overheatPenaltyTime) * Time.deltaTime;
        temperatureSlider.value = currentTemperature;

        if(overheatTimer <= 0)
        {
            isOverheated = false;
            currentTemperature = 0;
        }
    }
public void StartMinigame() {
        isMining = true;
        miningCanvas.SetActive(true);
        currentProgress = 0f;
        currentTemperature = 0f;
        yieldMultiplier = 1f;

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
            StartMinigame();
        }
        else {
            Debug.LogWarning("Brak danych o asteroidzie! Wracam do głównej sceny.");
            SceneManager.LoadScene("GameManager");
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