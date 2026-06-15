using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSectorButton : MonoBehaviour
{
    public Vector2Int gridPos;
    public TextMeshProUGUI InfoTextMapSector;
    public int cost = 500;

    public void OnClick()
    {
        string label = SectorCoordinates.GridToLabel(gridPos);
        SectorDefinition def = SectorContentDatabase.Instance.GetSector(gridPos);
        SectorData data = ChunkManager.Instance.allSectorData[gridPos];

        string summary = def != null
            ? $"Sektor {label}: {def.sectorName}\n{def.profileText}"
            : $"Sektor {label}";
        Color color = Color.white;

        if (EconomyManager.Instance.Credits < cost)
        {
            summary = "Nie sta? Ci? na skan";
            color = Color.red;
        }
        else
        {
            EconomyManager.Instance.SpendCredits(cost);
            if (data.hasAsteroidGroup)
            {
                List<string> stats = ChunkManager.Instance.GetSectorStats(data);
                foreach (string line in stats)
                {
                    if (line.Length > 0)
                        summary += $"\n+ {line}";
                }
                color = Color.black;
            }
            else
            {
                summary += "\nTen sektor jest pusty";
                color = Color.gray;
            }

            if (def != null && CockpitDisplayManager.Instance != null)
            {
                CockpitDisplayManager.Instance.ShowSectorBriefing(def);
                CockpitDisplayManager.Instance.ShowCRTLog(def.crtLogEntries);
            }
        }

        GameManager.Instance.ShowSectorInfo(summary, color);
    }
}
