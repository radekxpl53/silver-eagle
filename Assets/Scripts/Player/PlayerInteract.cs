using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour {
    public float range = 25f;
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
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (StationProximity.RequiresCursor)
        {
            if (contextCanvas != null)
                contextCanvas.SetActive(false);
            return;
        }

        if (gm.currentState == GameState.Exploration)
        {
            if (contextCanvas == null) return;

            Ray rayRight = new Ray(transform.position, transform.right);
            Ray rayLeft = new Ray(transform.position, -transform.right);
            RaycastHit hit;

            Debug.DrawRay(transform.position, transform.right * range, Color.yellow, 2f);
            Debug.DrawRay(transform.position, -transform.right * range, Color.yellow, 2f);

            bool foundAsteroid = false;

            if (Physics.Raycast(rayRight, out hit, range))
            {
                if (hit.collider.CompareTag("Asteroid"))
                    foundAsteroid = true;
            }
            if (!foundAsteroid && Physics.Raycast(rayLeft, out hit, range))
            {
                if (hit.collider.CompareTag("Asteroid"))
                    foundAsteroid = true;
            }

            if (foundAsteroid)
            {
                SetContextText("Aby wydobyć surowce naciśnij 'G'");
                contextCanvas.SetActive(true);

                if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
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
                SetContextText("Aby sprzedać surowce naciśnij 'C'");
                contextCanvas.SetActive(true);
            }
        }
    }

    private void SetContextText(string message)
    {
        if (contextCanvas == null) return;
        var label = contextCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = message;
    }

    void TryStartMining(RaycastHit hit) {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (hit.collider.CompareTag("Asteroid") && gm.currentState == GameState.Exploration) {

            Asteroid target = hit.collider.GetComponent<Asteroid>();
            InteractableObject io = hit.collider.GetComponent<InteractableObject>();
            if (target != null) {
                MiningAnalysisHelper.EmitAnalysisReady(target);

                MiningData.currentAsteroidLoot = target.materials;
                MiningData.currentAsteroidObject = target;

                MiningData.currentManager = io.manager;
                MiningData.currentBelt = io.myBelt;
                MiningData.currentArea = io.parentArea;

                SceneManager.LoadScene("MiningScene", LoadSceneMode.Additive);
                gm.ChangeState(GameState.Mining);
            }
            else {
                Debug.LogError("Obiekt ma tag Asteroid, ale brakuje mu skryptu Asteroid.cs!");
            }
        }
        else if (gm.currentState == GameState.Mining) {
            Debug.Log("Przecież już kopiesz lol");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SellZone"))
        {
            canSell = true;
            StationProximity.SetActive(StationServiceZone.ServiceType.Shop, true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SellZone"))
        {
            canSell = false;
            StationProximity.SetActive(StationServiceZone.ServiceType.Shop, false);
        }
    }
}
