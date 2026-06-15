using UnityEngine;
using TMPro;

public class Sector : MonoBehaviour {
    private SectorData data;

    [SerializeField] private GameObject shopPrefab;
    [SerializeField] private GameObject repairStationPrefab;
    public void Setup(SectorData newData, float size) {
        this.data = newData;
        //Debug.Log($"Sektor {data.gridPosition}");

        AreaSpawnerManager spawner = GetComponent<AreaSpawnerManager>();
        if (spawner != null) {
            spawner.currentSectorStage = data.sectorStage;
            spawner.InitialSpawn(data);
        }

        if (newData.haveShop == true)
        {
            GameObject shop = Instantiate(shopPrefab, transform);
            shop.transform.localPosition = data.shopLocalPos;
            EnsureServiceZone(FindZoneObject(shop, "SellZone"), StationServiceZone.ServiceType.Shop);
        }

        if (newData.haveRepairStation == true)
        {
            GameObject repairStation = Instantiate(repairStationPrefab, transform);
            repairStation.transform.localPosition = data.repairStationLocalPos;
            EnsureServiceZone(FindZoneObject(repairStation, "RepairZone"), StationServiceZone.ServiceType.Repair);
        }

        // Rysujemy obwùdki sektora
        //DrawSectorBorder(size);
    }

    private static GameObject FindZoneObject(GameObject station, string childName)
    {
        if (station == null) return null;
        Transform child = station.transform.Find(childName);
        return child != null ? child.gameObject : station;
    }

    private static void EnsureServiceZone(GameObject station, StationServiceZone.ServiceType type)
    {
        if (station == null) return;
        var zone = station.GetComponent<StationServiceZone>();
        if (zone == null) zone = station.AddComponent<StationServiceZone>();
        zone.Configure(type);
    }

        //private void DrawSectorBorder(float size) {
        //    LineRenderer line = gameObject.GetComponent<LineRenderer>();
        //    if (line == null) {
        //        line = gameObject.AddComponent<LineRenderer>();
        //    }

        //    line.startWidth = 4f;
        //    line.endWidth = 4f;
        //    line.useWorldSpace = false;
        //    line.loop = false;

        //    line.material = new Material(Shader.Find("Sprites/Default"));
        //    line.startColor = new Color(0f, 1f, 1f, 0.15f);
        //    line.endColor = new Color(0f, 1f, 1f, 0.15f);

        //    float half = size / 2f;
        //    Vector3[] corners = new Vector3[8];
        //    corners[0] = new Vector3(-half, -half, -half); // Lewy Dù Przùd
        //    corners[1] = new Vector3(half, -half, -half);  // Prawy Dù Przùd
        //    corners[2] = new Vector3(half, half, -half);   // Prawy Gùra Przùd
        //    corners[3] = new Vector3(-half, half, -half);  // Lewy Gùra Przùd
        //    corners[4] = new Vector3(-half, -half, half);  // Lewy Dù Tyù
        //    corners[5] = new Vector3(half, -half, half);   // Prawy Dù Tyù
        //    corners[6] = new Vector3(half, half, half);    // Prawy Gùra Tyù
        //    corners[7] = new Vector3(-half, half, half);   // Lewy Gùra Tyù

        //    Vector3[] path = new Vector3[] {
        //        corners[0], corners[1], corners[2], corners[3], corners[0], // Przednia ùciana
        //        corners[4], corners[5], corners[1],                         // Dù
        //        corners[5], corners[6], corners[2],                         // Prawy bok
        //        corners[6], corners[7], corners[3],                         // Gùra
        //        corners[7], corners[4]                                      // Tyù i lewy bok
        //    };

        //    line.positionCount = path.Length;
        //    line.SetPositions(path);
        //}
    }
