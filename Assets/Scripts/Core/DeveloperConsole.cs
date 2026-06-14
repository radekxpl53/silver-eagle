#if DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeveloperConsole : MonoBehaviour
{
    public static DeveloperConsole Instance { get; private set; }

    private readonly Dictionary<string, Action<string[]>> commands = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        AddCommand("buy_upgrade", args =>
        {
            if (args.Length == 0) return;
            var all = Resources.LoadAll<UpgradeDefinition>("Upgrades");
            foreach (var u in all)
            {
                if (u.upgradeId == args[0])
                {
                    ShopSystem.Instance?.TryPurchase(u);
                    return;
                }
            }
        });
    }

    public void AddCommand(string name, Action<string[]> handler)
    {
        commands[name] = handler;
    }

    public void Execute(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return;
        var parts = line.Split(' ');
        if (commands.TryGetValue(parts[0], out var cmd))
            cmd(parts.Length > 1 ? parts[1..] : Array.Empty<string>());
    }
}
#endif
