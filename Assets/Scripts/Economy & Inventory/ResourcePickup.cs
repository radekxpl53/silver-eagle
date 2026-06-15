using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ResourcePickup : MonoBehaviour
{
    public ResourceDefinition resource;
    public int amount = 10;
    public float lifetime = 120f;

    void Awake()
    {
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 3f;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInventory inv = other.GetComponent<PlayerInventory>();
        if (inv == null || resource == null) return;

        inv.AddResource(resource, amount);
        Destroy(gameObject);
    }

    public static GameObject Spawn(ResourceDefinition res, int amount, Vector3 position)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = $"Loot_{res.Name}";
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 1.5f;

        var rend = go.GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = new Color(0.9f, 0.75f, 0.2f);

        var pickup = go.AddComponent<ResourcePickup>();
        pickup.resource = res;
        pickup.amount = amount;
        return go;
    }
}
