using UnityEngine;
using System.Collections.Generic;

public class AreaSpawnerManager : MonoBehaviour
{
    [Header("Resources")]
    public ResourceDatabase database;
    
    [Header("Area settings")]
    public Vector3 areaSize = new Vector3(40f, 40f, 40f);
    
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
            // Stwórz Cube obszaru
            GameObject areaObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            areaObj.transform.position = transform.position + belt.beltCenter;
            areaObj.transform.localScale = areaSize;
            areaObj.transform.SetParent(this.transform);
            SetupMaterial(areaObj);

            foreach (AsteroidSavedData astData in belt.asteroids) {
                Vector3 worldPos = (transform.position + belt.beltCenter) + astData.localPos;
                GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, this.transform);

                InteractableObject io = obj.AddComponent<InteractableObject>();
                io.manager = this;
                io.parentArea = areaObj;
                io.lootTable = astData.loot;
            }
        }
    }

    public void CreateArea() {
        float range = 3400f;
        Vector3 areaCenter = transform.position + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));

        GameObject area = GameObject.CreatePrimitive(PrimitiveType.Cube);
        area.name = "SpawnArea";
        area.transform.position = areaCenter;
        area.transform.localScale = areaSize;
        SetupMaterial(area);

        int asteroidCount = Random.Range(5, 11);

        for (int i = 0; i < asteroidCount; i++) {
            GameObject obj = Instantiate(prefab, areaCenter + GetRandomOffset(), Quaternion.identity);
            InteractableObject io = obj.AddComponent<InteractableObject>();
            io.manager = this;
            io.parentArea = area;

            int totalUnits = Random.Range(40, 121);
            int typesCount = Random.Range(3, 7);
            int unitsPerSlot = totalUnits / typesCount;

            // Lista do pilnowania, żeby surowce w asteroidzie były RÓŻNE
            List<ResourceDefinition> uniqueResources = new List<ResourceDefinition>();

            for (int j = 0; j < typesCount; j++) {
                if (database != null) {
                    ResourceDefinition selected = null;
                    int attempts = 0;

                    while (attempts < 10) {
                        int roll = Random.Range(0, 100);
                        int targetStage = currentSectorStage;

                        if (roll < 20) targetStage -= 1;
                        else if (roll >= 90) targetStage += 1;

                        targetStage = Mathf.Clamp(targetStage, 0, 4);

                        selected = database.GetRandomResource(targetStage);

                        if (selected != null && !uniqueResources.Contains(selected)) {
                            break;
                        }
                        attempts++;
                    }

                    if (selected != null) {
                        uniqueResources.Add(selected);
                        io.lootTable.Add(new ResourceStack { definition = selected, amount = unitsPerSlot });
                    }
                }
            }
        }
        area.AddComponent<AreaTrigger>();
        areas.Add(area);
    }

    private Vector3 GetRandomOffset() {
        return new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            Random.Range(-areaSize.y / 2, areaSize.y / 2),
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );
    }
    public void OnObjectInteracted(GameObject currentArea)
    {
        // Gdy Gracz zbierze asteroidę → na świecie generuje się nowy pas
        if (ChunkManager.Instance != null) {
            ChunkManager.Instance.TrySpawnNewBeltGlobal();
        }

        InteractableObject[] objectsInArea = FindObjectsByType<InteractableObject>(FindObjectsSortMode.None);
        bool anyLeft = false;
        foreach (var obj in objectsInArea) {
            if (obj.parentArea == currentArea && obj.gameObject != this.gameObject) {
                anyLeft = true;
                break;
            }
        }

        if (!anyLeft) {
            areas.Remove(currentArea);
            Destroy(currentArea);
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
