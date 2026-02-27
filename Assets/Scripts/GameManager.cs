using UnityEngine;

public enum GameState {
    Exploration,
    Mining,
    Fighting,
    Menu,
    Console,
}

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    public GameState currentState = GameState.Exploration;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);     // przetrwa zmianê scen
            Debug.Log("<color=cyan>GameManager zosta³ zainicjalizowany jako Singleton</color>");
        }
        else {
            Debug.LogWarning("Drugi GameManager zosta³ zniszczony (duplikat)");
            Destroy(gameObject);
        }
    }

    public void ChangeState(GameState newState) {
        if (currentState == newState) return;

        currentState = newState;

        Debug.Log($"<color=yellow>GameState zmieniony na: <b>{newState}</b></color>");
    }
}