using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class AsteroidSavedData {
    public Vector3 localPos;
    public List<ResourceStack> loot = new List<ResourceStack>();
}

[System.Serializable]
public class BeltSavedData {
    public Vector3 beltCenter;
    public List<AsteroidSavedData> asteroids = new List<AsteroidSavedData>();
}
public class SectorData {
    public Vector2Int gridPosition;
    public int sectorStage;
    public bool hasAsteroidGroup;
    public bool wasSpawned = false;
    public List<BeltSavedData> belts = new List<BeltSavedData>();
}

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance;

    void Awake() {
        Instance = this;
    }

    [SerializeField] private int mapCols = 6;
    [SerializeField] private int mapRows = 6;
    [SerializeField] private float sectorSize = 7000f;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject sector;
    [SerializeField] private ResourceDatabase resourceDB;
    private GameObject currentSectorObject = null;

    private Dictionary<Vector2Int, SectorData> allSectorData = new Dictionary<Vector2Int, SectorData>();

    private Vector2Int currentPlayerSector = new Vector2Int(-1, -1);

    public static int globalGroupCount = 0;
    public int maxGlobalGroups = 20;

    // Generacja 6x6 mapy oraz predefiniowanie danych w asteroidach na ca³ej mapie
    private void GenerateWorldData() {
        List<Vector2Int> allCoords = new List<Vector2Int>();
        for (int x = 0; x < mapCols; x++) {
            for (int y = 0; y < mapRows; y++) {
                Vector2Int pos = new Vector2Int(x, y);
                allCoords.Add(pos);

                SectorData newData = new SectorData();
                int stage = Mathf.Max(x, y);
                newData.gridPosition = pos;
                newData.sectorStage = Mathf.Clamp(stage == 0 ? 0 : stage - 1, 0, 4);
                newData.hasAsteroidGroup = false;

                newData.belts = new List<BeltSavedData>();

                allSectorData.Add(pos, newData);
            }
        }

        allSectorData[Vector2Int.zero].hasAsteroidGroup = true;

        int groupsToSpawn = 30;
        for (int i = 0; i < groupsToSpawn; i++) {
            if (allCoords.Count == 0) break;

            int randomIndex = Random.Range(0, allCoords.Count);
            Vector2Int picked = allCoords[randomIndex];
            allCoords.RemoveAt(randomIndex);

            SectorData sd = allSectorData[picked];
            sd.hasAsteroidGroup = true;

            int beltCount = Random.Range(1, 3);
            for (int b = 0; b < beltCount; b++) {
                BeltSavedData belt = new BeltSavedData();
                belt.beltCenter = new Vector3(Random.Range(-3000f, 3000f), 0, Random.Range(-3000f, 3000f));

                int astCount = Random.Range(5, 11);
                for (int a = 0; a < astCount; a++) {
                    AsteroidSavedData ast = new AsteroidSavedData();
                    ast.localPos = new Vector3(Random.Range(-30f, 30f), Random.Range(-10f, 10f), Random.Range(-30f, 30f));
                    ast.loot = PreGenerateLoot(sd.sectorStage);
                    belt.asteroids.Add(ast);
                }
                sd.belts.Add(belt);
            }
        }

        // TEST
        DebugMapStats();
    }

    public void DebugMapStats() {
        int totalSectorsWithAsteroids = 0;
        int totalBelts = 0;
        int totalAsteroids = 0;

        foreach (var data in allSectorData.Values) {
            if (data.hasAsteroidGroup) {
                totalSectorsWithAsteroids++;
                totalBelts += data.belts.Count;
                foreach (var belt in data.belts) {
                    totalAsteroids += belt.asteroids.Count;
                }
            }
        }

        Debug.Log("RAPORT GENERACJI ŒWIATA");
        Debug.Log($"Sektory z asteroidami: {totalSectorsWithAsteroids} / 36");
        Debug.Log($"£¹czna liczba pasów: {totalBelts}");
        Debug.Log($"£¹czna liczba asteroid w pamiêci: {totalAsteroids}");
    }

    private List<ResourceStack> PreGenerateLoot(int stage) {
        List<ResourceStack> generatedLoot = new List<ResourceStack>();
        int totalUnits = Random.Range(40, 121);
        int typesCount = Random.Range(3, 7);
        int amountPerType = totalUnits / typesCount;

        for (int i = 0; i < typesCount; i++) {
            int roll = Random.Range(0, 100);
            int targetStage = stage;
            if (roll < 20) targetStage--;
            else if (roll >= 90) targetStage++;

            ResourceDefinition res = resourceDB.GetRandomResource(Mathf.Clamp(targetStage, 0, 4));
            if (res != null) {
                generatedLoot.Add(new ResourceStack { definition = res, amount = amountPerType });
            }
        }
        return generatedLoot;
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

    public void TrySpawnNewBeltGlobal() {
        if (allSectorData.Count == 0) return;

        List<Vector2Int> emptySectors = new List<Vector2Int>();
        foreach (var sector in allSectorData) {
            if (!sector.Value.hasAsteroidGroup) {
                emptySectors.Add(sector.Key);
            }
        }

        if (emptySectors.Count > 0 && GetTotalGroupCount() < 30) {
            Vector2Int targetPos = emptySectors[Random.Range(0, emptySectors.Count)];
            allSectorData[targetPos].hasAsteroidGroup = true;

            Debug.Log($"GDD Respawn: Nowa grupa asteroid zespawnowana w sektorze {targetPos}");
        }
    }

    private int GetTotalGroupCount() {
        int count = 0;
        foreach (var s in allSectorData.Values) {
            if (s.hasAsteroidGroup) count++;
        }
        return count;
    }
}
