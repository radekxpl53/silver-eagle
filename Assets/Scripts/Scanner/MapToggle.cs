using UnityEngine;
using UnityEngine.InputSystem;

public class MapToggle : MonoBehaviour {
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject info;


    private bool isOpen = false;

    public static bool IsOpen { get; private set; }

    void Start() {

        if (map != null && info != null) {
            map.SetActive(false);
            info.SetActive(false);
            isOpen = false;
            IsOpen = false;
        }
    }

    void Update() {
        if (Keyboard.current == null) return;


        if (Keyboard.current.mKey.wasPressedThisFrame && GameManager.Instance.currentState == GameState.Exploration) {
            //Debug.Log("Naciùniùto M");
            ToggleMap();
        }
    }

    public void ToggleMap() {
        isOpen = !isOpen;
        IsOpen = isOpen;
        map.SetActive(isOpen);
        info.SetActive(isOpen);

        if (isOpen) {

            MapDisplay mapDisplay = Object.FindFirstObjectByType<MapDisplay>();
            MapSectorButton infoDisplay = Object.FindFirstObjectByType<MapSectorButton>();
            if (mapDisplay != null)
            {
                mapDisplay.GenerateMapUI();
            }



            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else {

            if (!StationProximity.RequiresCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}