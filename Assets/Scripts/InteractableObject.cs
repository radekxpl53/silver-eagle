using UnityEngine;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    public AreaSpawnerManager manager;
    public GameObject parentArea;
    public List<ResourceStack> lootTable = new List<ResourceStack>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.OnObjectInteracted(parentArea);
            Destroy(gameObject);
        }
    }
}