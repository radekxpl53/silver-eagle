using UnityEngine;
using TMPro;
public enum GameState
{
    Exploration,
    Mining,
    Fighting,
    Menu,
    Console,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentState = GameState.Exploration;

    [Header("UI Notifications")]
    public TextMeshProUGUI notificationText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // przetrwa zmianę sceny
            //Debug.Log("<color=cyan>GameManager został zainicjalizowany jako Singleton</color>");
        }
        else
        {
            Debug.LogWarning("Drugi GameManager został zniszczony (duplikat)");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }

    public void ShowMiningNotification(string message, Color color)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.color = color;
            notificationText.gameObject.SetActive(true);

            // Wyłączamy napis po 3 sekundach
            CancelInvoke("HideNotification"); 
            Invoke("HideNotification", 6f);
        }
    }
    private void HideNotification()
    {
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        Debug.Log($"<color=yellow>GameState zmieniony na: <b>{newState}</b></color>");
    }
}