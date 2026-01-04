using UnityEngine;
using System.Collections.Generic;

public class AreaSpawnerManager : MonoBehaviour
{
    [Header("Area settings")]
    public Vector3 areaSize = new Vector3(40f, 40f, 40f);
    
    public Vector3 worldSpawnSize = new Vector3(500f, 0f, 500f);

    [Header("Objects")]
    public GameObject prefab;
    private GameObject player;

    private List<GameObject> areas = new List<GameObject>();
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        int randomAreaCount = Random.Range(0,3);

        for (int i = 0; i < randomAreaCount; i++)
        {
            CreateArea();
        }
    }

    void CreateArea()
    {
        // losowa pozycja obszaru w świecie
        Vector3 areaCenter = new Vector3(
            Random.Range(-worldSpawnSize.x / 2f, worldSpawnSize.x / 2f),
            Random.Range(0f, worldSpawnSize.y),
            Random.Range(-worldSpawnSize.z / 2f, worldSpawnSize.z / 2f)
        );

        // wizualny obszar
        GameObject area = GameObject.CreatePrimitive(PrimitiveType.Cube);
        area.name = "SpawnArea";
        area.transform.position = areaCenter;
        area.transform.localScale = areaSize;


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

        // spawn obiektów w obszarze
        int amount = Random.Range(5, 11);

        for (int i = 0; i < amount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f),
                Random.Range(-areaSize.z / 2f, areaSize.z / 2f)
            );

            GameObject obj = Instantiate(prefab, areaCenter + randomPos, Quaternion.identity);

            InteractableObject io = obj.AddComponent<InteractableObject>();
            io.manager = this;
            io.parentArea = area;
        }
        
        area.AddComponent<AreaTrigger>();
        areas.Add(area);
    }
    
    public void OnObjectInteracted(GameObject currentArea)
    {
        // jeśli jest tylko 1 obszar → po prostu dodaj drugi
        if (areas.Count == 1)
        {
            CreateArea();
            return;
        }

        // jeśli są 2 obszary → usuń ten, w którym NIE ma gracza
        if (areas.Count == 2)
        {
            foreach (GameObject area in areas)
            {
                if (area == currentArea) continue;

                AreaTrigger trigger = area.GetComponent<AreaTrigger>();

                if (!trigger.playerInside)
                {
                    areas.Remove(area);
                    
                    InteractableObject[] objectsInArea = FindObjectsByType<InteractableObject>(FindObjectsSortMode.None);
                    foreach (InteractableObject obj in objectsInArea)
                    {
                        if (obj.parentArea == area)
                        {
                            Destroy(obj.gameObject);
                        }
                    }
                    
                    Destroy(area);
                    CreateArea();
                    break;
                }
            }
        }
    }

}
