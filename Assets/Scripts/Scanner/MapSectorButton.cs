using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class MapSectorButton : MonoBehaviour {
    public Vector2Int gridPos;
    public TextMeshProUGUI InfoTextMapSector;
    public int cost = 500;
    public void OnClick()
    {
        string x = ((char)('A' + gridPos[1])).ToString();
        int y = gridPos[0] + 1;
        string summary = $"Sektor: {x+y}";
        Color color;
        SectorData data = ChunkManager.Instance.allSectorData[gridPos];
        if (EconomyManager.Instance.Credits - cost > 0)
        {
            EconomyManager.Instance.SpendCredits(cost);
            if (data.hasAsteroidGroup)
            {
                List<string> stats = ChunkManager.Instance.GetSectorStats(data);
                foreach (string line in stats)
                {
                    if (line.Length > 0)
                    {
                        summary += $"\n+ {line}";
                    }
                }
                color = Color.black;

            }
            else
            {
                summary = "Ten sektor jest pusty";
                color = Color.gray;
            }
        } else
        {
            summary = "Nie staæ Ciê na skan";
            color = Color.red;
        }
            GameManager.Instance.ShowSectorInfo(summary, color);
    }
}
