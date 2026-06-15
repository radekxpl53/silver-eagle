using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavTableInteract : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mapCanvas;
    [SerializeField] private GameObject fuelVsCargoPanel;
    
    [Header("Fuel Vs Cargo Configuration")]
    [SerializeField] private Slider fuelRatioSlider; // Slider from 0.1 to 0.9 (10% to 90% fuel ratio)
    [SerializeField] private TextMeshProUGUI fuelText;
    [SerializeField] private TextMeshProUGUI cargoText;

    private bool isMapOpen = false;
    private ShipStats playerStats;
    private ShipController playerController;

    private float baseMaxEnergy = 200f;
    private float baseMaxCargo = 100f;

    private void Start()
    {
        if (mapCanvas != null) mapCanvas.SetActive(false);
        if (fuelVsCargoPanel != null) fuelVsCargoPanel.SetActive(false);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerStats = playerObj.GetComponent<ShipStats>();
            playerController = playerObj.GetComponent<ShipController>();
            if (playerStats != null)
            {
                baseMaxEnergy = playerStats.GetMaxEnergy();
                baseMaxCargo = playerStats.GetMaxCargo();
            }
        }

        if (fuelRatioSlider != null)
        {
            fuelRatioSlider.onValueChanged.AddListener(OnRatioSliderChanged);
            OnRatioSliderChanged(fuelRatioSlider.value);
        }
    }

    private void Update()
    {
        // Check for interaction key press (E) when player is close
        if (Input.GetKeyDown(KeyCode.E) || (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame))
        {
            // Simple range check if player is near
            if (playerController != null && Vector3.Distance(transform.position, playerController.transform.position) < 5f)
            {
                ToggleMap();
            }
        }

        if (isMapOpen && (Input.GetKeyDown(KeyCode.Escape) || (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)))
        {
            CloseMap();
        }
    }

    public void ToggleMap()
    {
        if (isMapOpen) CloseMap();
        else OpenMap();
    }

    public void OpenMap()
    {
        isMapOpen = true;
        GameEvents.TriggerMapToggled();
        if (mapCanvas != null) mapCanvas.SetActive(true);
        if (fuelVsCargoPanel != null) fuelVsCargoPanel.SetActive(true);
        
        if (playerController != null)
        {
            playerController.isInteractingWithUI = true;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMap()
    {
        isMapOpen = false;
        if (mapCanvas != null) mapCanvas.SetActive(false);
        if (fuelVsCargoPanel != null) fuelVsCargoPanel.SetActive(false);
        
        if (playerController != null)
        {
            playerController.isInteractingWithUI = false;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnRatioSliderChanged(float val)
    {
        float fuelRatio = val;
        float cargoRatio = 1f - val;

        if (fuelText != null) fuelText.text = $"Fuel Ratio: {fuelRatio * 100:F0}%";
        if (cargoText != null) cargoText.text = $"Cargo Ratio: {cargoRatio * 100:F0}%";

        if (playerStats != null)
        {
            // Scale max limits based on choices
            playerStats.SetMaxEnergy(baseMaxEnergy * (fuelRatio * 2f));
            playerStats.UpdateMaxCargo(cargoRatio * 2f);
        }
    }

    // Called by UI buttons when clicking a sector on the map
    public void SelectSector(SectorDefinition sector)
    {
        if (sector != null && CockpitDisplayManager.Instance != null)
        {
            CockpitDisplayManager.Instance.ShowSectorBriefing(sector);
        }
    }

    public void JumpToSector(SectorDefinition sector)
    {
        if (sector == null) return;

        CloseMap();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        FastTravelSystem fastTravel = player != null ? player.GetComponent<FastTravelSystem>() : null;

        if (fastTravel != null && fastTravel.TryFastTravelToSector(sector.gridPosition))
            return;

        if (ChunkManager.Instance != null)
        {
            Vector3 dest = ChunkManager.Instance.GetSectorWorldCenter(sector.gridPosition);
            ChunkManager.Instance.ForcePlayerToSector(sector.gridPosition, dest);
        }
        else
        {
            GameEvents.TriggerSectorEntered(sector.gridPosition, sector);
        }
    }
}
