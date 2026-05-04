using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using FMODUnity;

public class ShipSFX : MonoBehaviour
{
    [SerializeField] private EventReference thrusterSfx;
    [SerializeField] private ShipStats shipStats;

    private EventInstance shipIdle;
    private EventInstance shipMove;
    private EventInstance alarmSfx;
    
    private bool isRotating = false;
    private bool alarmIsPlaying = false;

    [SerializeField] private float fadeSpeed = 2f;
    private float idleVolume = 1f;
    private float moveVolume = 0f;

    void Start()
    {
        shipIdle = RuntimeManager.CreateInstance(FMODEvents.instance.shipIdle);
        shipMove = RuntimeManager.CreateInstance(FMODEvents.instance.shipMove);
        alarmSfx = RuntimeManager.CreateInstance(FMODEvents.instance.alarm);

        shipIdle.setVolume(1f);
        shipMove.setVolume(0f);
        shipIdle.start();
        shipMove.start();

        RuntimeManager.AttachInstanceToGameObject(shipIdle, gameObject);
        RuntimeManager.AttachInstanceToGameObject(shipMove, gameObject);
    }

    bool IsApplyingThrust()
    {
        if (GameManager.Instance.currentState != GameState.Mining)
        {
            var kb = Keyboard.current;
            if (kb == null) return false;
            return kb.wKey.isPressed || kb.sKey.isPressed || kb.spaceKey.isPressed || kb.shiftKey.isPressed;
        }
        return false;
    }

    bool IsRotating()
    {
        if (GameManager.Instance.currentState != GameState.Mining)
        {
            var kb = Keyboard.current;
            if (kb == null) return false;
            return kb.aKey.isPressed || kb.dKey.isPressed;
        }
        return false;
    }

    void Update()
    {
        bool currentlyThrusting = IsApplyingThrust();
        bool currentlyRotating = IsRotating();

        float targetIdleVolume = currentlyThrusting ? 0f : 1f;
        float targetMoveVolume = currentlyThrusting ? 1f : 0f;

        idleVolume = Mathf.MoveTowards(idleVolume, targetIdleVolume, fadeSpeed * Time.deltaTime);
        moveVolume = Mathf.MoveTowards(moveVolume, targetMoveVolume, fadeSpeed * Time.deltaTime);

        shipIdle.setVolume(idleVolume);
        shipMove.setVolume(moveVolume);

        if (!isRotating && currentlyRotating)
        {
            isRotating = true;
        }
        else if (isRotating && !currentlyRotating)
        {
            isRotating = false;
            RuntimeManager.PlayOneShot(thrusterSfx, transform.position);
        }
        
        if (shipStats != null && shipStats.CurrentHP <= shipStats.GetMaxHP() * 0.2f)
        {
            if (!alarmIsPlaying)
            {
                alarmIsPlaying = true;
                alarmSfx.start();
            }
        }
        else
        {
            if (alarmIsPlaying)
            {
                alarmSfx.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                alarmIsPlaying = false;
            }
        }
    }

    void OnDestroy()
    {
        shipIdle.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        shipIdle.release();
        shipMove.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        shipMove.release();
        alarmSfx.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        alarmSfx.release();
    }
}