using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;

    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] GameObject optionsPanel;
    
    // audio
    private EventInstance mainMusic;
    
    void Start()
    {
        pauseMenu.SetActive(false);
        pauseMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    // AUTOMATYCZNIE wywoływane przez PlayerInput
    public void OnPause()
    {
        Debug.Log("ESCAPE DZIAŁA");
        if (isPaused)
        {
            ResumeGame();
            ShowMenu();
        }
        else
            PauseGame();
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        mainMusic = AudioManager.instance.CreateInstance(FMODEvents.instance.mainMusic);
        mainMusic.start();
    }
    
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        mainMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mainMusic.release();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
            #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
            #else
                        Application.Quit();
            #endif
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void OptionsMenu()
    {
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }
    public void ShowMenu()
    {
        optionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}