using System.Collections.Generic;
using UnityEngine;

public class DevConsoleKolisionTest : MonoBehaviour
{
    public static DevConsoleKolisionTest Instance;
    private Dictionary<string, System.Action<string[]>> commands = new Dictionary<string, System.Action<string[]>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Zachowaj między scenami
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCommand(string commandName, System.Action<string[]> action)
    {
        if (!commands.ContainsKey(commandName))
        {
            commands.Add(commandName, action);
        }
    }

    // Dodaj metodę do wykonywania komend (np. w Update lub z UI)
    public void ExecuteCommand(string command, string[] args)
    {
        if (commands.TryGetValue(command, out var action))
        {
            action.Invoke(args);
        }
        else
        {
            Debug.Log("Nieznana komenda: " + command);
        }
    }
}
