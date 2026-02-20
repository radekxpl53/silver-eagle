using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public float range = 5; // długość lasera
    public MiningGame miningGame;
    private Asteroid asteroid;
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 direction = Vector3.forward;
            Ray theRay = new Ray(transform.position, transform.TransformDirection(direction * range));
            Debug.DrawRay(transform.position, transform.TransformDirection(direction * range));
            if(Physics.Raycast(theRay, out RaycastHit hit, range))
            {
                asteroid = hit.collider.GetComponent<Asteroid>();
                if(hit.collider.tag == "Asteroid")
                {
                    Debug.Log("Wciśnij E");
                    return;
                }
            }
            asteroid = null;
        }

        if (asteroid != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            miningGame.StartMinigame(asteroid);
            asteroid = null; 
        }
    }
}
