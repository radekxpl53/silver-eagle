using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using FMODUnity;

public class ShipSFX : MonoBehaviour
{
    [SerializeField] private EventReference thrusterSfx;
    
    private EventInstance shipIdle;
    private EventInstance shipMove;
    private bool isMoving = false;

    void Start()
    {
        shipIdle = RuntimeManager.CreateInstance(FMODEvents.instance.shipIdle);
        shipMove = RuntimeManager.CreateInstance(FMODEvents.instance.shipMove);

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

            return kb.wKey.isPressed || kb.sKey.isPressed || 
                   kb.spaceKey.isPressed || kb.shiftKey.isPressed;
        }
        return false;
    }

    void Update()
    {
        bool currentlyThrusting = IsApplyingThrust();
        
        if (currentlyThrusting && !isMoving)
        {
            StartMoving();
        }
        else if (!currentlyThrusting && isMoving)
        {
            StopMoving();
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
        
        RuntimeManager.PlayOneShot(thrusterSfx, transform.position);
        
        shipMove.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        shipIdle.start();
    }

    void OnDestroy()
    {
        shipIdle.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        shipIdle.release();
        shipMove.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        shipMove.release();
    }
}