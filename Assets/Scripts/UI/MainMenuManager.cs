using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    // debug variable for testing
    [SerializeField] private bool saveFileExists = true;

    private void Start()
    {
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
        Debug.Log("Starting new game...");
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

    private void OnOptionsClicked()
    {
        Debug.Log("Opening options menu...");
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}