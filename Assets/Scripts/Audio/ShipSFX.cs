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
    private bool isMoving = false;
    private bool isRotating = false;

    private bool alarmIsPlaying = false;

    void Start()
    {
        shipIdle = RuntimeManager.CreateInstance(FMODEvents.instance.shipIdle);
        shipMove = RuntimeManager.CreateInstance(FMODEvents.instance.shipMove);

        alarmSfx = RuntimeManager.CreateInstance(FMODEvents.instance.alarm);

        shipIdle.start();
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

        if (currentlyThrusting && !isMoving)
        {
            StartMoving();
        }
        else if (!currentlyThrusting && isMoving)
        {
            StopMoving();
        }
        else if (currentlyRotating && !isRotating)
        {
            StartRotating();
        }
        else if (!currentlyRotating && isRotating)
        {
            StopRotating();
        }

        // alarm, alarm!
        if (shipStats != null && shipStats.CurrentHP <= shipStats.GetMaxHP() * 0.2)
        {
            if (!alarmIsPlaying)
            {
                alarmIsPlaying = true;
                alarmSfx.start();
                Debug.Log("Alarm started");
            }
        }
        else
        {
            alarmSfx.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            alarmIsPlaying = false;
        }
    }

    void StartMoving()
    {
        isMoving = true;
        
        shipIdle.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        
        shipMove.start();
    }

    void StopMoving()
    {
        isMoving = false;
        
        shipMove.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        shipIdle.start();
    }

    void StartRotating() 
    { 
        isRotating = true;
    }

    void StopRotating()
    {
        isRotating = false;

        RuntimeManager.PlayOneShot(thrusterSfx, transform.position);
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