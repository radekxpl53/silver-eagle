using UnityEngine;

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

    [Header("--- OKNO PORAZKI ---")]
    public GameObject loseScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("<color=cyan>GameManager zosta³ zainicjalizowany jako Singleton</color>");
        }
        else
        {
            Debug.LogWarning("Drugi GameManager zosta³ zniszczony (duplikat)");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        Debug.Log($"<color=yellow>GameState zmieniony na: <b>{newState}</b></color>");
    }
}