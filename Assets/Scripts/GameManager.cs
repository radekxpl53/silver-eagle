using UnityEngine;

public enum GameState
{
    Exploration,
    Mining,
    Fighting,
    Menu,
    Console,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState = GameState.Exploration;

    [Header("--- WYTRZYMA£OŒÆ GRACZA ---")]
    public float maxHullPoints = 1000f; // hp
    public float currentHullPoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);     // przetrwa zmianê scen
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
        // Inicjalizacja HP na starcie
        currentHullPoints = maxHullPoints;
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log($"<color=yellow>GameState zmieniony na: <b>{newState}</b></color>");
    }

    // metoda do zarz¹dzania obra¿eniami z poziomu GameManagera
    public void ApplyDamage(float amount)
    {
        currentHullPoints -= amount;

        if (currentHullPoints <= 0)
        {
            currentHullPoints = 0;
            ExplodeShip();
        }
    }

    private void ExplodeShip()
    {
        Debug.Log("STATEK ZNISZCZONY!");
    }
}