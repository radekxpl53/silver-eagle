using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Wymagane dla nowego systemu

public class MiningGame : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject miningCanvas;

    [Header("UI References")]
    public RectTransform greenZone;
    public RectTransform rock;
    public RectTransform background;
    public Slider progressSlider;

    [Header("Settings")]
    public float damping = 5f; // Im wyższa wartość, tym szybciej pasek się zatrzymuje (brak "ślizgania")
    public float moveForce = 20f;
    public float resistance = 12f;
    public float rockSpeed = 3f;
    public float progressGain = 0.2f; 
    public float progressLoss = 0.15f; 
    
    private float greenZoneVelocity;
    private float miningTimer;
    private float rockTargetPos;
    private float currentProgress = 0.2f;
    private bool isMining = false;

    // Referencja do akcji (możesz ją przypisać w inspektorze lub użyć Keyboard.current)
    private bool isPressingAction => Keyboard.current.spaceKey.isPressed || Pointer.current.press.isPressed;

    void Update()
    {
        // Start gry nowym systemem (WasPressedThisFrame zastępuje GetKeyDown)
        if (!isMining && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartMining();
        }

        if (isMining)
        {
            HandleMining();
        }
    }

    void HandleMining()
    {
        // 1. Logika Paska Gracza przy użyciu New Input System
        if (isPressingAction)
            greenZoneVelocity += moveForce * Time.deltaTime;
        else
            greenZoneVelocity -= resistance * Time.deltaTime;

        greenZoneVelocity -= greenZoneVelocity * damping * Time.deltaTime;
        greenZone.anchoredPosition += new Vector2(greenZoneVelocity, 0);

        // 2. Logika Kamienia (AI) - bez zmian
        miningTimer -= Time.deltaTime;
        if (miningTimer < 0)
        {
            miningTimer = Random.Range(0.5f, 2f);
            float range = (background.rect.width / 2) - (rock.rect.width / 2);
            rockTargetPos = Random.Range(-range, range);
        }
        
        float newRockX = Mathf.Lerp(rock.anchoredPosition.x, rockTargetPos, Time.deltaTime * rockSpeed);
        rock.anchoredPosition = new Vector2(newRockX, 0);

        ClampHorizontal(greenZone);
        ClampHorizontal(rock);
        CheckWinCondition();
    }

    // Pozostałe metody (StartMining, ClampHorizontal, CheckWinCondition, EndGame) 
    // pozostają takie same jak w poprzednim kroku.

    void StartMining()
    {
        isMining = true;
        currentProgress = 0.2f;
        miningCanvas.SetActive(true);
        greenZone.anchoredPosition = Vector2.zero;
        rock.anchoredPosition = Vector2.zero;
        greenZoneVelocity = 0;
    }

    void ClampHorizontal(RectTransform rect)
    {
        float limit = (background.rect.width / 2) - (rect.rect.width / 2);
        if (Mathf.Abs(rect.anchoredPosition.x) > limit)
        {
            float side = Mathf.Sign(rect.anchoredPosition.x);
            rect.anchoredPosition = new Vector2(limit * side, 0);
            if (rect == greenZone) greenZoneVelocity = 0;
        }
    }

    void CheckWinCondition()
    {
        float rockX = rock.anchoredPosition.x;
        float greenX = greenZone.anchoredPosition.x;
        float halfGreen = greenZone.rect.width / 2;

        if (rockX < greenX + halfGreen && rockX > greenX - halfGreen)
            currentProgress += progressGain * Time.deltaTime;
        else
            currentProgress -= progressLoss * Time.deltaTime;

        currentProgress = Mathf.Clamp01(currentProgress);
        progressSlider.value = currentProgress;

        if (currentProgress >= 1f) EndGame("WYDOBYTO!");
        if (currentProgress <= 0f) EndGame("PRZEGRANA!");
    }

    void EndGame(string message)
    {
        Debug.Log(message);
        isMining = false;
        miningCanvas.SetActive(false);
    }
}