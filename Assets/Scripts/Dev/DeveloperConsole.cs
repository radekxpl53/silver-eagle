using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DeveloperConsole : MonoBehaviour
{
    public static DeveloperConsole Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject consoleUI;
    [SerializeField] private TMP_InputField consoleInput;
    [SerializeField] private GameObject logsSpace;
    [SerializeField] private GameObject logTextPrefab;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Settings")]
    [SerializeField] private InputActionReference toggleConsoleKey;
    [SerializeField] private int maxLogCount = 50;


    private Dictionary<string, Action<String[]>> commands = new Dictionary<string, Action<String[]>>();
    private Queue<GameObject> logObjectsQueue = new Queue<GameObject>();

    private void Awake() {
        Instance = this;
    }

    private void Start()
    {
        consoleUI.SetActive(false);
        if (consoleInput != null)
        {
            consoleInput.onSubmit.AddListener(HandleInputSubmit);
        }

        AddCommand("help", ShowAllCommands);
    }

    private void ShowAllCommands(string[] args) {
        string commandList = "\nDostêpne komendy: ";

        foreach (string cmd in commands.Keys) {
            commandList += cmd + "\n ";
        }

        string newLogMessage = $"<color=#00FFFF>{commandList}</color>\n";
        SpawnLogText(newLogMessage);
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

        if (willBeActive) {
            GameManager.Instance.ChangeState(GameState.Console);
            consoleInput.ActivateInputField();
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            GameManager.Instance.ChangeState(GameState.Exploration);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void HandleLogEvent(string logString, string stackTrace, LogType type)
    {
        string color = type == LogType.Error || type ==  LogType.Exception ? "red" : "white";
        if (type == LogType.Warning)
        {
            color = "yellow";
        }

        string newLogMessage = $"<color={color}>[{type}] {logString}</color>\n";

        SpawnLogText(newLogMessage);

    }

    private void SpawnLogText(string message) {
        if (logsSpace == null || logTextPrefab == null) {
            Debug.LogWarning("Brakuje referencji do logsSpace lub logTextPrefab w skrypcie DeveloperConsole!");
            return;
        }

        GameObject newLogObj = Instantiate(logTextPrefab);

        newLogObj.transform.SetParent(logsSpace.transform, false);

        newLogObj.SetActive(true);

        if (newLogObj.TryGetComponent<TMP_Text>(out TMP_Text textComponent)) {
            textComponent.text = message;
        }


        logObjectsQueue.Enqueue(newLogObj);

        if (logObjectsQueue.Count >= maxLogCount) {
            GameObject oldestLog = logObjectsQueue.Dequeue();
            Destroy(oldestLog);
        }

        if (gameObject.activeInHierarchy) {
            StartCoroutine(ScrollToBottom());
        }
    }

    private IEnumerator ScrollToBottom() {
        yield return new WaitForEndOfFrame();

        if (scrollRect != null) {
            scrollRect.verticalNormalizedPosition = 0f;
        }
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
