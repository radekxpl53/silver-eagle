using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMOD.Studio;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject controlsPanel;

    private EventInstance mainMusic;

    private void Start()
    {
        mainMusic = AudioManager.instance.CreateInstance(FMODEvents.instance.mainMusic);

        PLAYBACK_STATE state;
        mainMusic.getPlaybackState(out state);
        if (state != PLAYBACK_STATE.PLAYING) mainMusic.start();

        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        if (loadGameButton != null)
            loadGameButton.interactable = SaveDataJSON.HasSaveFile();

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
        Restart.ResetData();
        SaveDataJSON.PendingLoad = false;
        SceneManager.LoadScene("GameManager");
    }

    private void OnLoadGameClicked()
    {
        if (!SaveDataJSON.HasSaveFile())
        {
            Debug.LogWarning("[Menu] Brak pliku zapisu.");
            return;
        }

        SaveDataJSON.PendingLoad = true;
        SceneManager.LoadScene("GameManager");
    }

    public void OnOptionsClicked()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ShowMenu()
    {
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void OnQuitClicked()
    {
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
