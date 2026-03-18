using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    public  TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public TMP_Dropdown graphicsDropdown;
    public Toggle fullscreenToggle;
    
    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;

        if (resolutions.Length == 0) {
            //Debug.LogError("Unity nie znalaz�o �adnych rozdzielczo�ci!");
            return;
        }

        //Debug.Log("Znaleziono rozdzielczo�ci: " + resolutions.Length);

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
        //Debug.Log("Suwak wysy�a: " + volume);
        if (volume <= 0.0001f) volume = 0.0001f;

        AudioManager.instance.masterVolume = volume;

        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        } else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Resolution currentRes = resolutions[resolutionDropdown.value];
            Screen.SetResolution(currentRes.width, currentRes.height, false);
        }
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        //Debug.Log("Zmieniono na: " + resolutionIndex);
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("volume"))
        {
            float volume = PlayerPrefs.GetFloat("volume");
            volumeSlider.value = volume;
            SetVolume(volume);
        }

        if (PlayerPrefs.HasKey("graphics"))
        {
            int quality = PlayerPrefs.GetInt("graphics");
            graphicsDropdown.value = quality;
            SetQuality(quality);
        }

        if (PlayerPrefs.HasKey("fullscreen"))
        {
            bool fullscreen = PlayerPrefs.GetInt("fullscreen") == 1;
            fullscreenToggle.isOn = fullscreen;
            SetFullscreen(fullscreen);
        }
    }
}
