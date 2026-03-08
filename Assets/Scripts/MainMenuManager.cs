using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using FMOD.Studio;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    // debug variable for testing
    [SerializeField] private bool saveFileExists = true;
    
    // audio
    private EventInstance mainMusic;

    private void Start()
    {
        // music
        mainMusic = AudioManager.instance.CreateInstance(FMODEvents.instance.mainMusic);
        mainMusic.start();
        
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);

        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);
        
        if (loadGameButton != null)
            loadGameButton.onClick.AddListener(OnLoadGameClicked);
        
        if (optionsButton != null)
            optionsButton.onClick.AddListener(OnOptionsClicked);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnNewGameClicked()
    {
        //Debug.Log("Starting new game...");
        
        mainMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mainMusic.release();
        
        SceneManager.LoadScene("GameManager");
    }

    private void OnLoadGameClicked()
    {
        Debug.Log("Loading saved game...");

        if(saveFileExists)
        {
            Debug.Log("Loading recent save...");
        } else
        {
            Debug.LogWarning("No save file found!");
        }
    }

    public void OnOptionsClicked()
    {
        //Debug.Log("PRZYCISK DZIAŁA!");
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ShowMenu() {
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
    
    private void OnDestroy()
    {
        mainMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        mainMusic.release();
    }
}