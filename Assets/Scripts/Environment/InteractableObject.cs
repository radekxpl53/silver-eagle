using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObject : MonoBehaviour {
    public AreaSpawnerManager manager;
    public GameObject parentArea;
    public List<ResourceStack> lootTable = new List<ResourceStack>();

    public AsteroidSavedData myData;
    public BeltSavedData myBelt;

    public float distanceBetweenObjects;

    [SerializeField] private float maxDistanceFromBeltCenter = 120f;
    [SerializeField] private float distanceCheckInterval = 2f;

    private float timer;

    [Header("Asteroid Explosion")]
    [SerializeField] private GameObject explosionPrefab;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (myData != null) {
                myData.loot.Clear();
            }

            if (manager != null) {
                manager.OnObjectInteracted(parentArea, myBelt);
            }

            //Destroy(gameObject);
            Debug.Log("Dotknąłeś asteroide :) UwU");
        }
    }

    private void Update()
    {
        if (parentArea == null) return;

        timer += Time.deltaTime;
        if (timer >= distanceCheckInterval)
        {
            distanceBetweenObjects = Vector3.Distance(transform.position, parentArea.transform.position);
            Debug.DrawLine(transform.position, parentArea.transform.position, Color.green);
            CheckDistance();
            timer = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (parentArea == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, parentArea.transform.position);
        Gizmos.DrawWireSphere(parentArea.transform.position, maxDistanceFromBeltCenter);
    }

    void CheckDistance()
    {
        if (distanceBetweenObjects > maxDistanceFromBeltCenter)
        {
            if (myData != null)
            {
                myData.loot.Clear();
            }
            manager.OnObjectInteracted(parentArea, myBelt);

            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(
                    explosionPrefab,
                    transform.position,
                    transform.rotation
                );

                Scene asteroidScene = gameObject.scene;
                SceneManager.MoveGameObjectToScene(explosion, asteroidScene);
            }

            Destroy(gameObject);
            Debug.Log("Obiekt asteroidy usunięty z głównej sceny");

            
        }
    }
}