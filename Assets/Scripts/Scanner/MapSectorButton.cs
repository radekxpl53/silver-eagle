using System.Collections.Generic;
using UnityEngine;

public class MapSectorButton : MonoBehaviour {
    public Vector2Int gridPos;

    public void OnClick() {
        Debug.Log($"Klikniêto sektor: {gridPos}");
        SectorData data = ChunkManager.Instance.allSectorData[gridPos];
        if (data.hasAsteroidGroup) {
            List<string> stats = ChunkManager.Instance.GetSectorStats(data);
            foreach (string line in stats) {
                Debug.Log(line);
            }
        } else {
            Debug.Log("Ten sektor jest pusty");
        }
    }
}