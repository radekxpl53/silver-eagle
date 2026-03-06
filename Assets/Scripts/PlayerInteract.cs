using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInteract : MonoBehaviour {
    public float range = 20f;
    public ShipStats shipStats;
    void Update() {
        // LOG 1: Czy Unity w ogóle widzi, że klikasz E?
        if (Keyboard.current.eKey.wasPressedThisFrame) {
            Debug.Log("Naciśnięto klawisz E");

            Ray rayRight = new Ray(transform.position, transform.right);
            Ray rayLeft = new Ray(transform.position, -transform.right);
            RaycastHit hit;

            // Wizualizacja laserów w oknie Scene
            Debug.DrawRay(transform.position, transform.right * range, Color.yellow, 2f);
            Debug.DrawRay(transform.position, -transform.right * range, Color.yellow, 2f);

            // Sprawdzamy prawą stronę
            if (Physics.Raycast(rayRight, out hit, range)) {
                TryStartMining(hit);
            }
            // Sprawdzamy lewą stronę
            else if (Physics.Raycast(rayLeft, out hit, range)) {
                TryStartMining(hit);
            }
            else {
                Debug.Log("Lasery boczne w nic nie trafiły. Podleć bliżej asteroidy burtą!");
            }
        }
    }

    void TryStartMining(RaycastHit hit) {
        Debug.Log("Laser w coś trafił: " + hit.collider.name);

        

        if (hit.collider.CompareTag("Asteroid") && GameManager.Instance.currentState == GameState.Exploration) {
            if (shipStats != null) {
                // sprawdzenie czy magazyn jest pełny
                if (shipStats.CurrentCargo >= shipStats.MaxCargo) {
                    Debug.LogWarning("<color=red> Brak miejsca w ładowni Opróżnij magazyn, aby kopać.</color>");
                    // WarningPopup.Instance.Show("CARGO FULL");
                    return; 
                }
            }
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
}