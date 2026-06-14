using UnityEngine;
using FMODUnity;

public class MusicStateController : MonoBehaviour
{
    private const string MUSIC_STATE_PARAM = "MusicState";
    private int currentMusicState = -1;

    private void Update()
    {
        if (GameManager.Instance == null) return;

        int targetState = 0;
        if (GameManager.Instance.currentState == GameState.Fighting || 
            GameManager.Instance.currentState == GameState.GameOver)
        {
            targetState = 1;
        }

        if (targetState != currentMusicState)
        {
            currentMusicState = targetState;
            SetFmodParameter(currentMusicState);
        }
    }

    private void SetFmodParameter(float val)
    {
        FMOD.RESULT result = RuntimeManager.StudioSystem.setParameterByName(MUSIC_STATE_PARAM, val);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning($"FMOD: Failed to set parameter '{MUSIC_STATE_PARAM}' to {val}: {result}");
        }
        else
        {
            Debug.Log($"FMOD: Set parameter '{MUSIC_STATE_PARAM}' to {val}");
        }
    }
}
