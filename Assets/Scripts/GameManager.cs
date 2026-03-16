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

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        Debug.Log($"<color=yellow>GameState zmieniony na: <b>{newState}</b></color>");
    }
}