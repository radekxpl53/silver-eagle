using UnityEngine;

public class ProstPool : MonoBehaviour
{
    public static ProstPool Instance { get; private set; }

    [SerializeField] private GameObject prefab;
    [SerializeField] private int prewarm = 16;

    private readonly System.Collections.Generic.Queue<GameObject> pool = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (prefab == null)
            prefab = Resources.Load<GameObject>("AI/HeavyKineticProjectile");
        Prewarm();
    }

    private void Prewarm()
    {
        if (prefab == null) return;
        for (int i = 0; i < prewarm; i++)
            pool.Enqueue(CreateInstance());
    }

    private GameObject CreateInstance()
    {
        var go = Instantiate(prefab);
        go.SetActive(false);
        return go;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject go = pool.Count > 0 ? pool.Dequeue() : CreateInstance();
        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);
        return go;
    }

    public void Release(GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);
        pool.Enqueue(go);
    }
}
