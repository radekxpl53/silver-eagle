using UnityEngine;

public class asteroidSpawner : MonoBehaviour
{
    public GameObject prefab;

    public Vector3 cubeSize = new Vector3(500f, 500f, 500f);
    public Vector3 cubeCenter = Vector3.zero;


    void Start()
    {
        int randomAmount = Random.Range(5, 11);
        for (int i = 0; i < randomAmount; i++)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-cubeSize.x / 2f, cubeSize.x / 2f),
            Random.Range(-cubeSize.y / 2f, cubeSize.y / 2f),
            Random.Range(-cubeSize.z / 2f, cubeSize.z / 2f)
        );

        Instantiate(prefab, cubeCenter + randomPos, Quaternion.identity);
    }
}
