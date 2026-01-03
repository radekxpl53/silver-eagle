using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public AreaSpawnerManager manager;
    public GameObject parentArea;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.OnObjectInteracted(parentArea);
            Destroy(gameObject);
        }
    }
}