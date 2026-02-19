using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SectorData {
    public Vector2Int gridPosition;
    public int asteroidCount;
}

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private int mapCols = 6;
    [SerializeField] private int mapRows = 6;
    [SerializeField] private float sectorSize = 7000f;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject sector;
    private GameObject currentSectorObject = null;

    private Dictionary<Vector2Int, SectorData> allSectorData = new Dictionary<Vector2Int, SectorData>();

    private Vector2Int currentPlayerSector = new Vector2Int(-1, -1);


    // Generacja 6x6 mapy
    private void GenerateWorldData() {
        for (int x = 0; x < mapCols; x++) {
            for (int y = 0; y < mapRows; y++) {
                SectorData newData = new SectorData();
                newData.gridPosition = new Vector2Int(x, y);
                allSectorData.Add(newData.gridPosition, newData);
                newData.asteroidCount = Random.Range(1, 100); // Tu se sprawdzam czy sie zapisuja dane git
            }
        }

    }

    // Wyœwietlanie, aktualizacja i usuwanie aktywnego sektora
    private void RefreshSectorView(Vector2Int sectorCooRD) {
        if (currentSectorObject != null) {
            Destroy(currentSectorObject);
        }
        Vector3 sectorSpawnPos = new Vector3((sectorCooRD.x * sectorSize) + (sectorSize / 2f), 0, (sectorCooRD.y * sectorSize) + (sectorSize / 2f));
        currentSectorObject = Instantiate(sector, sectorSpawnPos, Quaternion.identity);

        if (allSectorData.ContainsKey(sectorCooRD)) {
            SectorData dataFromMemory = allSectorData[sectorCooRD];

            Sector sectorScript = currentSectorObject.GetComponent<Sector>();
            if (sectorScript != null) {
                sectorScript.Setup(dataFromMemory);
            }
        }
    }


    // Rozruch
    void Start()
    {
        GenerateWorldData();
        Debug.Log("Wygenerowano bazê danych sektorów: " + allSectorData.Count);

    }

    void Update()
    {
        if (player == null) {
            return;
        }
        // Ograniczenie Clampem, ¿eby nie wylecieæ za sektory (bo wywala b³êdy)
        float maxX = mapCols * sectorSize;
        float maxZ = mapRows * sectorSize;

        Vector3 limitedPos = player.position;
        limitedPos.x = Mathf.Clamp(limitedPos.x, 0, maxX - 1);
        limitedPos.z = Mathf.Clamp(limitedPos.z, 0, maxZ - 1);
        player.position = limitedPos;

        // Pobieranie sektora w którym znajduje sie gracz
        int playerXPosition = Mathf.FloorToInt(player.position.x / sectorSize);
        int playerZPosition = Mathf.FloorToInt(player.position.z / sectorSize);

        Vector2Int currentPos = new Vector2Int(playerXPosition, playerZPosition);

        // Zmiana informacji o sektorze
        if (currentPos != currentPlayerSector) {
            currentPlayerSector = currentPos;
            Debug.Log("Wlecia³em do nowego sektora:" + currentPlayerSector);
            RefreshSectorView(currentPos);
        }
    }
}
