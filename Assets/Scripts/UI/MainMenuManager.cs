using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using FMOD.Studio;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    private PlayerData playerData;
    private EconomyManager economyManager;

    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject controlsPanel;
    
    // audio
    private EventInstance mainMusic;

    private void Start()
    {
        // music
        mainMusic = AudioManager.instance.CreateInstance(FMODEvents.instance.mainMusic);
        
        PLAYBACK_STATE state;
        mainMusic.getPlaybackState(out state);
        if (state != PLAYBACK_STATE.PLAYING) mainMusic.start();
        
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);
        
        if (loadGameButton != null)
            loadGameButton.onClick.AddListener(OnLoadGameClicked);
        
        if (optionsButton != null)
            optionsButton.onClick.AddListener(OnOptionsClicked);
        
        if (controlsButton != null)
            controlsButton.onClick.AddListener(OnControlsClicked);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnNewGameClicked()
    {
        //Debug.Log("Starting new game...");
        
        SceneManager.LoadScene("GameManager");
    }

    void LoadGame()
    {
        playerData = PlayerData.Instance;
        economyManager = EconomyManager.Instance;

        string path = Application.persistentDataPath + "/SaveData.json";

        if(!File.Exists(path))
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        string json = File.ReadAllText(path);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        ShipStats shipStats = player.GetComponent<ShipStats>();

        player.transform.position = data.position;

        playerData.SetPlayerData(data.hp, data.credits, data.energy, data.inventory, data.position,
        data.speed, data.maneuverability, data.acceleration, data.cargoHold,
        data.durability, data.shield, data.militaryScanner, data.laserTemperature,
        data.drillDurability, data.asteroidReport, data.sectorInformation,
        data.fastTravel, data.repairDrones, data.repairKits);

        shipStats.SetHP(data.hp);
        shipStats.SetEnergy(data.energy);
        shipStats.SetCargo(data.cargoHold);
        economyManager.SetCredits(data.credits);

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        inventory.myItems.Clear();

        foreach (var item in data.inventory)
        {
            inventory.myItems.Add(new ResourceStack
            {
                definition = item.definition,
                amount = item.amount
            });
        }

        inventory.RefreshUI();

        Debug.Log("Loading saved game...");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "GameManager")
        {
            LoadGame();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    private void OnLoadGameClicked()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("GameManager");
    }

    public void OnOptionsClicked()
    {
        //Debug.Log("PRZYCISK DZIAŁA!");
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ShowMenu() {
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void OnQuitClicked()
    {
        //Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void OnControlsClicked()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        mainMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        mainMusic.release();
    }
}