using UnityEngine;
using System.Collections.Generic;

public class AreaSpawnerManager : MonoBehaviour
{
    [Header("Resources")]
    public ResourceDatabase database;
    
    [Header("Area settings")]
    public Vector3 areaSize = new Vector3(300f, 40f, 80f);

    public Vector3 worldSpawnSize = new Vector3(500f, 0f, 500f);

    [Header("Objects")]
    public GameObject prefab;
    private GameObject player;

    private List<GameObject> areas = new List<GameObject>();

    public int currentSectorStage;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    public void InitialSpawn(SectorData data) {
        if (!data.hasAsteroidGroup) return;

        foreach (BeltSavedData belt in data.belts) {
            // Najpierw sprawdzamy, czy wszytskie asteroidy w pasie są wykopane
            int emptyCount = 0;
            foreach (var ast in belt.asteroids) {
                if (ast.loot.Count == 0) emptyCount++;
            }

            // Jeśli tak, to kładziemy na niego lache
            if (emptyCount == belt.asteroids.Count) continue;

            // Stwórz Cube obszaru
            GameObject areaObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            areaObj.transform.position = transform.position + belt.beltCenter;
            areaObj.transform.localScale = areaSize;
            areaObj.transform.SetParent(this.transform);
            SetupMaterial(areaObj);

            foreach (AsteroidSavedData astData in belt.asteroids) {
                if (astData.loot.Count == 0) continue;

                Vector3 worldPos = (transform.position + belt.beltCenter) + astData.localPos;
                GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, this.transform);

                InteractableObject io = obj.GetComponent<InteractableObject>();
                if (io == null) {
                    io = obj.AddComponent<InteractableObject>();
                }

                io.manager = this;
                io.parentArea = areaObj;
                io.lootTable = astData.loot;
                io.myBelt = belt;
                io.myData = astData;
                

                Asteroid asteroidVisual = obj.GetComponent<Asteroid>();
                if (asteroidVisual != null) {
                    asteroidVisual.materials = astData.loot;
                    asteroidVisual.showInfoAsteroid(); 
                }
            }
            areas.Add(areaObj);
        }
    }

    //public void CreateArea() {
    //    float range = 3400f;
    //    Vector3 areaCenter = transform.position + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));

    //    GameObject area = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //    area.name = "SpawnArea";
    //    area.transform.position = areaCenter;
    //    area.transform.localScale = areaSize;
    //    SetupMaterial(area);

    //    int asteroidCount = Random.Range(5, 11);

    //    for (int i = 0; i < asteroidCount; i++) {
    //        GameObject obj = Instantiate(prefab, areaCenter + GetRandomOffset(), Quaternion.identity);
    //        InteractableObject io = obj.AddComponent<InteractableObject>();
    //        io.manager = this;
    //        io.parentArea = area;

    //        int totalUnits = Random.Range(40, 121);
    //        int typesCount = Random.Range(3, 7);
    //        int unitsPerSlot = totalUnits / typesCount;

    //        // Lista do pilnowania, żeby surowce w asteroidzie były RÓŻNE
    //        List<ResourceDefinition> uniqueResources = new List<ResourceDefinition>();

    //        for (int j = 0; j < typesCount; j++) {
    //            if (database != null) {
    //                ResourceDefinition selected = null;
    //                int attempts = 0;

    //                while (attempts < 10) {
    //                    int roll = Random.Range(0, 100);
    //                    int targetStage = currentSectorStage;

    //                    if (roll < 20) targetStage -= 1;
    //                    else if (roll >= 90) targetStage += 1;

    //                    targetStage = Mathf.Clamp(targetStage, 0, 4);

    //                    selected = database.GetRandomResource(targetStage);

    //                    if (selected != null && !uniqueResources.Contains(selected)) {
    //                        break;
    //                    }
    //                    attempts++;
    //                }

    //                if (selected != null) {
    //                    uniqueResources.Add(selected);
    //                    io.lootTable.Add(new ResourceStack { definition = selected, amount = unitsPerSlot });
    //                }
    //            }
    //        }
    //    }
    //    area.AddComponent<AreaTrigger>();
    //    areas.Add(area);
    //}

    private Vector3 GetRandomOffset() {
        return new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            Random.Range(-areaSize.y / 2, areaSize.y / 2),
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );
    }
    public void OnObjectInteracted(GameObject currentArea, BeltSavedData beltData)
    {
        // Sprawdzamy ile pasów jest pustych
        int emptyCount = 0;
        int totalAsteroids = beltData.asteroids.Count;

        foreach (var ast in beltData.asteroids) {
            if (ast.loot.Count == 0) emptyCount++;
        }


        // Jak wykopane asteroidy w pasie >80% to tworzy nowy pas na mapie
        float minedPercentage = (float)emptyCount / totalAsteroids;

        if (minedPercentage >= 0.80f && !beltData.respawnTriggered) {
            if (ChunkManager.Instance != null) {
                ChunkManager.Instance.TrySpawnNewBeltGlobal();
            }
            beltData.respawnTriggered = true; // Blokada, żeby respiło tylko raz
        }

        // Jak wykopiemy całe to go usuwa na ament
        if (emptyCount == totalAsteroids) {
            if (areas.Contains(currentArea)) areas.Remove(currentArea);
            if (currentArea != null) Destroy(currentArea);
        }
    }

    public void SetupMaterial(GameObject area) {
        BoxCollider col = area.GetComponent<BoxCollider>();
        col.isTrigger = true;
        col.enabled = true;

        // półprzezroczysty materiał (opcjonalnie)
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0f, 1f, 0f, 0.2f);
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        area.GetComponent<Renderer>().material = mat;
    }

}
