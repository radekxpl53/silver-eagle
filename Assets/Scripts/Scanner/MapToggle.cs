using UnityEngine;
using UnityEngine.InputSystem;

public class MapToggle : MonoBehaviour {
    public GameObject map;

    private bool isOpen = false;

    void Start() {

        if (map != null) {
            map.SetActive(false);
            isOpen = false;
        }
    }

    void Update() {
        if (Keyboard.current == null) return;


        if (Keyboard.current.mKey.wasPressedThisFrame) {
            Debug.Log("Naciœniêto M");
            ToggleMap();
        }
    }

    public void ToggleMap() {
        isOpen = !isOpen;
        map.SetActive(isOpen);

        if (isOpen) {

            MapDisplay mapDisplay = Object.FindFirstObjectByType<MapDisplay>();
            if (mapDisplay != null) mapDisplay.GenerateMapUI();


            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}