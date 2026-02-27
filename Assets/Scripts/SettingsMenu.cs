using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    public  TMP_Dropdown resolutionDropdown;
    
    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;

        if (resolutions.Length == 0) {
            Debug.LogError("Unity nie znalaz³o ¿adnych rozdzielczoœci!");
            return;
        }

        Debug.Log("Znaleziono rozdzielczoœci: " + resolutions.Length);

        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio.value.ToString("F0") + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        LoadSettings();
    }
    
    public void SetVolume(float volume)
    {
        Debug.Log("Suwak wysy³a: " + volume);
        if (volume <= 0.0001f) volume = 0.0001f;

        float dB = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("volume", dB);

        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log("Zmieniono na: " + resolutionIndex);
    }

    private void LoadSettings() {
        if (PlayerPrefs.HasKey("volume")) {
            float volume = PlayerPrefs.GetFloat("volume");
            SetVolume(volume);
        }
    }
}
