using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;

    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] GameObject optionsPanel;
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
    }
    
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenuTempalte");
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