using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private GameObject backgroud;
    [SerializeField] private GameObject sector;
    [SerializeField] private GameObject labelPrefab;

    public void GenerateMapUI()
    {
        foreach (Transform child in backgroud.transform)
        {
            Destroy(child.gameObject);
        }

        for (var j = ChunkManager.Instance.mapRows; j >= 0; j--)
        {
            for (var i = 0; i < (ChunkManager.Instance.mapCols + 1); i++)
            {
                int playerXPosition = Mathf.FloorToInt(ChunkManager.Instance.Player.position.x / ChunkManager.Instance.SectorSize);
                int playerZPosition = Mathf.FloorToInt(ChunkManager.Instance.Player.position.z / ChunkManager.Instance.SectorSize);

                Vector2Int currentPos = new Vector2Int(playerXPosition, playerZPosition);
                if (i == ChunkManager.Instance.mapCols || j == 0)
                {
                    GameObject label = Instantiate(labelPrefab, backgroud.transform);
                    TextMeshProUGUI textInfo = label.GetComponentInChildren<TextMeshProUGUI>();

                    if (i == ChunkManager.Instance.mapCols && j == 0)
                    {
                        textInfo.SetText("");
                    }
                    else if (j == 0)
                    {
                        textInfo.SetText((i + 1).ToString());
                        textInfo.color = Color.black;
                    }
                    else if (i == ChunkManager.Instance.mapCols)
                    {
                        textInfo.SetText(((char)('A' + (j - 1))).ToString());
                        textInfo.color = Color.black;
                    }
                    continue;
                }

                Vector2Int gridCoords = new Vector2Int(i, j - 1);

                GameObject newSector = Instantiate(sector, backgroud.transform);
                MapSectorButton btnScript = newSector.GetComponent<MapSectorButton>();
                if (btnScript != null)
                {
                    btnScript.gridPos = gridCoords;
                }

                Image img = newSector.GetComponent<Image>();


                if(gridCoords == currentPos)
                {
                    img.color = Color.green;
                }
                else if (ChunkManager.Instance.allSectorData[gridCoords].hasAsteroidGroup)
                {
                    img.color = Color.brown;
                }
                else if(!ChunkManager.Instance.allSectorData[gridCoords].hasAsteroidGroup)
                {
                    img.color = Color.gray;
                }

                Button btn = newSector.GetComponent<Button>();
                btn.onClick.AddListener(btnScript.OnClick);
            }
        }
    }
}


