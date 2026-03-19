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
    public TextMeshProUGUI infoText;

    [Header("Death System")]
    [SerializeField] private GameObject deathScreenCanvas;
    [SerializeField] private Transform baseSpawnPoint;
    [SerializeField] private GameObject player;

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
        if (deathScreenCanvas != null)
        {
            deathScreenCanvas.SetActive(false);
        }
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
            Invoke("HideNotification", 8f);
        }
    }

    public void ShowSectorInfo(string message, Color color)
    {
        if (infoText != null)
        {
            infoText.text = message;
            infoText.color = color;
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

    public void TriggerGameOver()
    {
        ChangeState(GameState.GameOver);

        if (deathScreenCanvas != null)
            deathScreenCanvas.SetActive(true);

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RespawnAtBase()
    {
        Time.timeScale = 1f;

        if (player != null && baseSpawnPoint != null)
        {
            player.transform.position = baseSpawnPoint.position;
        }

        if (deathScreenCanvas != null)
            deathScreenCanvas.SetActive(false);

        ChangeState(GameState.Exploration);
    }
}