using UnityEngine;
using TMPro;
using System;
using NUnit.Framework;
using System.Collections.Generic;
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
    public static GameManager Instance {get; private set; }
    public GameState currentState = GameState.Exploration;

    private GameObject _deathScreenCanvas;
    private Transform _baseSpawnPoint;
    private GameObject _player;
    public List<Transform> allRepairStationsPosition = new List<Transform>();
    public static event Action<string, Color> OnNotificationRequested;
    public static event Action<string, Color> OnSectorInfoRequested;


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
        if (_deathScreenCanvas != null)
        {
            _deathScreenCanvas.SetActive(false);
        }
    }

    public void RegisterPlayer(GameObject playerInstance)
    {
        _player = playerInstance;
        Debug.Log("<color=green>Gracz został pomyślnie zarejestrowany!</color>");
    }
    public void RegisterDeathScreen(GameObject canvas) => _deathScreenCanvas = canvas;
    public void RegisterSpawnPoint(Transform spawn) => _baseSpawnPoint = spawn; 

    public void ShowMiningNotification(string message, Color color)
    {
        OnNotificationRequested?.Invoke(message, color);
    }

    public void ShowSectorInfo(string message, Color color)
    {
        OnSectorInfoRequested?.Invoke(message, color);
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
        if (_deathScreenCanvas != null) _deathScreenCanvas.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RespawnAtBase()
    {
        Time.timeScale = 1f;
        if (_player != null)
        {
            Vector3 targetPosition = Vector3.zero;
            bool stationFound = false;

            if (allRepairStationsPosition.Count > 0)
            {
                float minDistance = Mathf.Infinity;
                Transform nearestStation = null;
                foreach (Transform t in allRepairStationsPosition)
                {
                    if (t == null) continue; // Zabezpieczenie przed usuniętymi obiektami
                    float distance = Vector3.Distance(_player.transform.position, t.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestStation = t;
                    }
                }
                if (nearestStation != null) { targetPosition = nearestStation.position; stationFound = true; }
            }

            if (!stationFound && _baseSpawnPoint != null) targetPosition = _baseSpawnPoint.position;

            _player.transform.position = targetPosition;
            _player.transform.rotation = Quaternion.identity;

            if (_player.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (_deathScreenCanvas != null) _deathScreenCanvas.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ChangeState(GameState.Exploration);
    }
}