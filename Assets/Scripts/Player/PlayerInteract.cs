using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour {
    public float range = 20f;
    public ShipStats shipStats;
    public bool canSell;
    [SerializeField] private GameObject contextCanvas;

    private void Start()
    {
        if (contextCanvas != null)
            contextCanvas.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance.currentState == GameState.Exploration)
        {
            //Debug.Log("Naciśnięto klawisz G");

            Ray rayRight = new Ray(transform.position, transform.right);
            Ray rayLeft = new Ray(transform.position, -transform.right);
            RaycastHit hit;

            Debug.DrawRay(transform.position, transform.right * range, Color.yellow, 2f);
            Debug.DrawRay(transform.position, -transform.right * range, Color.yellow, 2f);

            bool foundAsteroid = false;
            
            if (Physics.Raycast(rayRight, out hit, range))  // <- Kacper miał racje, outy są mega
            {
                if (hit.collider.CompareTag("Asteroid"))
                {
                    //TryStartMining(hit);
                    foundAsteroid = true;
                }
                else
                {
                    //Debug.Log("Prawy laser trafił w: " + hit.collider.tag);
                }
            }
            if (!foundAsteroid && Physics.Raycast(rayLeft, out hit, range)) 
            {
                if (hit.collider.CompareTag("Asteroid"))
                {
                    //TryStartMining(hit);
                    foundAsteroid = true;
                }
                else
                {
                    //Debug.Log("Lewy laser trafił w: " + hit.collider.tag);
                }
            }

            if (foundAsteroid)
            {
                contextCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "Aby wydobyć surowce naciśnij 'G'";
                contextCanvas.SetActive(true);
                
                // sprawdz klawisz
                if (Keyboard.current.gKey.wasPressedThisFrame)
                {
                    TryStartMining(hit);
                    contextCanvas.SetActive(false);
                }
            }
            else
            {
                contextCanvas.SetActive(false);
            }

            if (canSell)
            {
                contextCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "Aby sprzedać surowce naciśnij 'C'";
                contextCanvas.SetActive(true);
            }
        }
    }

    void TryStartMining(RaycastHit hit) {
        Debug.Log("Laser w coś trafił: " + hit.collider.name);

        if (hit.collider.CompareTag("Asteroid") && GameManager.Instance.currentState == GameState.Exploration) {
            
            Asteroid target = hit.collider.GetComponent<Asteroid>();

            InteractableObject io = hit.collider.GetComponent<InteractableObject>();
            if (target != null) {
                MiningData.currentAsteroidLoot = target.materials;
                MiningData.currentAsteroidObject = target;

                MiningData.currentManager = io.manager;
                MiningData.currentBelt = io.myBelt;
                MiningData.currentArea = io.parentArea;

                SceneManager.LoadScene("MiningScene", LoadSceneMode.Additive);

                // Zmieniamy stan gry na Mining
                GameManager.Instance.ChangeState(GameState.Mining);
            }
            else {
                Debug.LogError("Obiekt ma tag Asteroid, ale brakuje mu skryptu Asteroid.cs!");
            }
        }
        else if (GameManager.Instance.currentState == GameState.Mining) {
            Debug.Log("Przecież już kopiesz lol");
        }
        else {
            Debug.Log("Trafiony obiekt nie ma tagu 'Asteroid'. Ma tag: " + hit.collider.tag);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SellZone"))
        {
            canSell = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SellZone"))
        {
            canSell = false;
        }
    }
}