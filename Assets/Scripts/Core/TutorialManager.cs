using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialOverlayText;
    [SerializeField] private GameObject tutorialOverlayPanel;

    private int currentStep = 0;
    private bool tutorialActive = false;
    private Rigidbody playerRigidbody;

    private void Start()
    {
        // Check if tutorial is already completed
        if (PlayerPrefs.GetInt("tutorialDone", 0) == 1)
        {
            if (tutorialOverlayPanel != null) tutorialOverlayPanel.SetActive(false);
            return;
        }

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerRigidbody = playerObj.GetComponent<Rigidbody>();
        }

        tutorialActive = true;
        if (tutorialOverlayPanel != null) tutorialOverlayPanel.SetActive(true);
        StartStep(1);

        // Subscriptions
        GameEvents.OnMiningComplete += HandleMiningComplete;
        GameEvents.OnResourcesSold += HandleResourcesSold;
        GameEvents.OnMapToggled += HandleMapToggled;
    }

    private void OnDestroy()
    {
        GameEvents.OnMiningComplete -= HandleMiningComplete;
        GameEvents.OnResourcesSold -= HandleResourcesSold;
        GameEvents.OnMapToggled -= HandleMapToggled;
    }

    private void Update()
    {
        if (!tutorialActive) return;

        switch (currentStep)
        {
            case 1: // WASD Flight
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || 
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
                    (playerRigidbody != null && playerRigidbody.linearVelocity.magnitude > 1f))
                {
                    StartStep(2);
                }
                break;

            case 2: // Fly to Asteroid and press E
                if (Input.GetKeyDown(KeyCode.E) || (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame))
                {
                    // Basic step progression, minigame will start
                    StartStep(3);
                }
                break;
        }
    }

    private void StartStep(int step)
    {
        currentStep = step;
        switch (step)
        {
            case 1:
                SetText("TUTORIAL [1/5]\nPress WASD keys to maneuver your ship.");
                break;
            case 2:
                SetText("TUTORIAL [2/5]\nFly towards an asteroid and press E to lock on.");
                break;
            case 3:
                SetText("TUTORIAL [3/5]\nComplete the mining minigame to harvest resources.");
                break;
            case 4:
                SetText("TUTORIAL [4/5]\nFly back to the Sell Zone to offload your cargo.");
                break;
            case 5:
                SetText("TUTORIAL [5/5]\nPress M (or Nav Table) to open the star map.");
                break;
        }
    }

    private void HandleMiningComplete()
    {
        if (currentStep == 3)
        {
            StartStep(4);
        }
    }

    private void HandleResourcesSold()
    {
        if (currentStep == 4)
        {
            StartStep(5);
        }
    }

    private void HandleMapToggled()
    {
        if (currentStep == 5)
        {
            CompleteTutorial();
        }
    }

    private void CompleteTutorial()
    {
        tutorialActive = false;
        PlayerPrefs.SetInt("tutorialDone", 1);
        PlayerPrefs.Save();
        SetText("TUTORIAL COMPLETE\nSafe flights, Captain.");
        Invoke(nameof(HideOverlay), 3f);
    }

    private void SetText(string txt)
    {
        if (tutorialOverlayText != null)
        {
            tutorialOverlayText.text = txt;
        }
    }

    private void HideOverlay()
    {
        if (tutorialOverlayPanel != null)
        {
            tutorialOverlayPanel.SetActive(false);
        }
    }
}
