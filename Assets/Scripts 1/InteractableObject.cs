using UnityEngine;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    public AreaSpawnerManager manager;
    public GameObject parentArea;
    public List<ResourceDefinition> assignedResources = new List<ResourceDefinition>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.OnObjectInteracted(parentArea);
            Destroy(gameObject);
        }
    }
}