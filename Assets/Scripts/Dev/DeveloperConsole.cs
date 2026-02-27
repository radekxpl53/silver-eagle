using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeveloperConsole : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject consoleUI;
    [SerializeField] private TMP_InputField consoleInput;
    [SerializeField] private TextMeshProUGUI logText;

    [Header("Settings")]
    [SerializeField] private InputActionReference toggleConsoleKey;
    [SerializeField] private int maxLogCount = 25;


    private Dictionary<string, Action<String[]>> commands = new Dictionary<string, Action<String[]>>();
    private Queue<String> logQueue = new Queue<string>();

    private void Start()
    {
        consoleUI.SetActive(false);
        if (consoleInput != null)
        {
            consoleInput.onSubmit.AddListener(HandleInputSubmit);
        }
    }

    private void HandleInputSubmit(string inputValue)
    {
        if (String.IsNullOrWhiteSpace(inputValue))
        {
            return;
        }

        ExecuteCommand(inputValue.Trim());

        consoleInput.text = string.Empty;

        consoleInput.ActivateInputField();
    }

    private void OnEnable()
    {
        toggleConsoleKey.action.performed += ToggleConsole;
        toggleConsoleKey.action.Enable();
        Application.logMessageReceived += HandleLogEvent;
    }

    private void OnDisable()
    {
        toggleConsoleKey.action.performed -= ToggleConsole;
        toggleConsoleKey.action.Disable();
        Application.logMessageReceived -= HandleLogEvent;

    }
    private void ToggleConsole(InputAction.CallbackContext context)
    {
        if (consoleUI == null)
        {
            return;
        }

        bool willBeActive = !consoleUI.activeSelf;
        consoleUI.SetActive(willBeActive);

        if (willBeActive)
        {
            GameManager.Instance.ChangeState(GameState.Console);
            consoleInput.ActivateInputField();
        } else
        {
            GameManager.Instance.ChangeState(GameState.Exploration);
        }
    }

    private void HandleLogEvent(string logString, string stackTrace, LogType type)
    {
        string color = type == LogType.Error || type ==  LogType.Exception ? "red" : "white";
        if (type == LogType.Warning)
        {
            color = "yellow";
        }

        string newLog = $"<color={color}>[{type}] {logString}</color>\n";
        logQueue.Enqueue(newLog);

        if (logQueue.Count >= maxLogCount)
        {
            logQueue.Dequeue();
        }

        UpdateLogUI();

    }

    private void UpdateLogUI()
    {
        if (logText == null)
        {
            return;
        }

        logText.text = string.Join("", logQueue);
    }

    public void AddCommand(String commandName, Action<String[]> functionToRun)
    {
        if (!commands.ContainsKey(commandName))
        {
            commands.Add(commandName, functionToRun);
            Debug.Log("Zarejestrowano Komendê: " + commandName);
        } else
        {
            Debug.LogWarning("Komenda " + commandName + " ju¿ istnieje!");
        }
    }

    public void ExecuteCommand (String inputValue)
    {
        string[] inputParts = inputValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string commandName = inputParts[0];

        string[] args = new string[inputParts.Length - 1];

        Array.Copy(inputParts,1, args, 0, args.Length);

        if (commands.ContainsKey(commandName))
        {
            commands[commandName].Invoke(args);
        } else
        {
            Debug.LogWarning("Nie znaleziono Komendy: " + commandName);
        }
    }
}
