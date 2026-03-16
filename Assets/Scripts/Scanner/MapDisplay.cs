using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MapDisplay : MonoBehaviour {
    [SerializeField] private GameObject backgroud;
    [SerializeField] private GameObject sector;
    private bool isGenerated = false;

    public void GenerateMapUI() {
        if (!isGenerated) {
            for (var i = 0; i < ChunkManager.Instance.mapCols; i++) {
                for (var j = 0; j < ChunkManager.Instance.mapRows; j++) {
                    GameObject newSector = Instantiate(sector, backgroud.transform);

                    MapSectorButton btnScript = newSector.GetComponent<MapSectorButton>();
                    if (btnScript != null) {
                        btnScript.gridPos = new Vector2Int(i, j);
                    }

                    Image img = newSector.GetComponent<Image>();

                    if (ChunkManager.Instance.allSectorData[new Vector2Int(i, j)].hasAsteroidGroup) {
                        img.color = Color.brown;
                    }
                    else {
                        img.color = Color.gray;
                    }
                    Button btn = newSector.GetComponent<Button>();

                    btn.onClick.AddListener(btnScript.OnClick);
                    //Debug.Log($"Przypisano klikniêcie do sektora {i}, {j}");
                }
            }
            isGenerated = true;
        }
    }
}


