using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;

public class InteractableObject : MonoBehaviour {
    public AreaSpawnerManager manager;
    public GameObject parentArea;
    public List<ResourceStack> lootTable = new List<ResourceStack>();

    public AsteroidSavedData myData;
    public BeltSavedData myBelt;
    
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (myData != null) {
                myData.loot.Clear();
            }

            if (manager != null) {
                manager.OnObjectInteracted(parentArea, myBelt);
            }

            //Destroy(gameObject);
            Debug.Log("Dotk�e� asteroide :) UwU");
        }
    }
}