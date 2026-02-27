using System;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    [SerializeField] private DeveloperConsole developerConsole;

    private int playerHealth = 100;

    private void Start()
    {
        if (developerConsole != null)
        {
            developerConsole.AddCommand("set_hp", SetHpCommand);
        }
    }

    private void SetHpCommand(string[] args)
    {
        if (args.Length == 0)
        {
            Debug.LogWarning("Usage: set_hp <value>");
            return;
        }

        if (int.TryParse(args[0], out int newHp))
        {
            playerHealth = newHp;
            Debug.Log("Player HP set to: " + playerHealth);
        }
        else
        {
            Debug.LogWarning("Invalid HP value: " + args[0]);
        }
    }
}
