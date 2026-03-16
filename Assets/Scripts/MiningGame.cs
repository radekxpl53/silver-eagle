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
    public RectTransform greenZone;
    public RectTransform rock;
    public RectTransform background;
    public Slider progressSlider;

    [Header("Settings")]
    public float damping = 5f; // Im wyższa wartość, tym szybciej pasek się zatrzymuje (brak "ślizgania")
    public float moveForce = 20f;
    public float resistance = 12f;
    public float rockSpeed = 3f;
    public float progressGain = 0.2f; 
    public float progressLoss = 0.15f; 
    
    private float greenZoneVelocity;
    private float miningTimer;
    private float rockTargetPos;
    private float currentProgress = 0.2f;
    private bool isMining = false;
    [Header("Asteroid Explosion")]
    [SerializeField] private GameObject explosionPrefab;

    [Header("Overheat / Instability")]       
    public float smallErrorRate = 0.05f;     // 5% niestabilności
    public float criticalErrorRate = 0.1f;  // 10% niestabilności
    public float instability;

    [Header("Yield")]
    public float yieldMultiplier = 1f;

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
        // 1. Logika Paska Gracza przy użyciu New Input System
        if (isPressingAction)
            greenZoneVelocity += moveForce * Time.deltaTime;
        else
            greenZoneVelocity -= resistance * Time.deltaTime;

        greenZoneVelocity -= greenZoneVelocity * damping * Time.deltaTime;
        greenZone.anchoredPosition += new Vector2(greenZoneVelocity, 0);

        // 2. Logika Kamienia (AI) - bez zmian
        miningTimer -= Time.deltaTime;
        if (miningTimer < 0)
        {
            miningTimer = Random.Range(0.5f, 2f);
            float range = (background.rect.width / 2) - (rock.rect.width / 2);
            rockTargetPos = Random.Range(-range, range);
        }
        
        float newRockX = Mathf.Lerp(rock.anchoredPosition.x, rockTargetPos, Time.deltaTime * rockSpeed);
        rock.anchoredPosition = new Vector2(newRockX, 0);

        ClampHorizontal(greenZone);
        ClampHorizontal(rock);
        CheckWinCondition();
    }

    public void StartMinigame() {
        PLAYBACK_STATE state;
        laserCollecting.getPlaybackState(out state);
        if (state != PLAYBACK_STATE.PLAYING)
        {
            laserCollecting.start();
        }
        
        isMining = true;
        miningCanvas.SetActive(true);
        
        currentProgress = 0.2f;
        instability = 0f;
        yieldMultiplier = 1f;
    }


    void ClampHorizontal(RectTransform rect)
    {
        float limit = (background.rect.width / 2) - (rect.rect.width / 2);
        if (Mathf.Abs(rect.anchoredPosition.x) > limit)
        {
            float side = Mathf.Sign(rect.anchoredPosition.x);
            rect.anchoredPosition = new Vector2(limit * side, 0);
            if (rect == greenZone) greenZoneVelocity = 0;
        }
    }

    void CheckWinCondition()
    {
        float rockX = rock.anchoredPosition.x;
        float greenX = greenZone.anchoredPosition.x;
        float halfGreen = greenZone.rect.width / 2;

        if (rockX < greenX + halfGreen && rockX > greenX - halfGreen)
            currentProgress += progressGain * yieldMultiplier * Time.deltaTime;
        else{
            currentProgress -= progressLoss * Time.deltaTime;

            float distance = Mathf.Abs(rockX - greenX);

            if (distance < halfGreen * 2f)
                instability += smallErrorRate * Time.deltaTime;
            else
                instability += criticalErrorRate * Time.deltaTime;
        }

        if (instability >= 0.1f && instability < 0.9f)
        {
            yieldMultiplier = 1f - instability; // ilość surowców zmniejsza się proporcjonalnie z niestabilnością
        }
        else if (instability >= 0.9f)
        {
            yieldMultiplier = 0f; // jeżeli przekraczamy 90% niestabilności nie zyskujemy nic (Thermal Shock)
        }
        else
        {
            yieldMultiplier = 1f; // wydobyto 
        }

        currentProgress = Mathf.Clamp01(currentProgress);
        progressSlider.value = currentProgress;

        if (instability >= 0.9f)
        {
            ThermalShock();
            return;
        }

        instability = Mathf.Clamp01(instability);

        if (currentProgress >= 1f){
            EndGame("WYDOBYTO!");
        }
        if (currentProgress <= 0f){
            yieldMultiplier = 0;
            EndGame("PRZEGRANA!");
        }
    }

    void ThermalShock()
    {
        Debug.Log("THERMAL SHOCK!");
        EndGame("ASTEROIDA ROZWALONA");
        // tutaj można dodać logikę obrażenie od odłamków
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
        
        if (laserCollecting.isValid())
        {
            laserCollecting.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            RuntimeManager.PlayOneShot(successSfx, transform.position);
        }

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