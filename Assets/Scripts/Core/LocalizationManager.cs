using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private Dictionary<string, string> plTranslations = new Dictionary<string, string>();
    private Dictionary<string, string> enTranslations = new Dictionary<string, string>();
    
    [SerializeField] private string currentLanguage = "pl"; // "pl" or "en"

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTranslations();
            currentLanguage = PlayerPrefs.GetString("Language", "pl");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadTranslations()
    {
        TextAsset csvAsset = Resources.Load<TextAsset>("Localization/Strings");
        if (csvAsset == null)
        {
            Debug.LogError("Localization: Could not load Strings.csv from Resources/Localization/Strings");
            return;
        }

        string[] lines = csvAsset.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length <= 1) return;

        // Header: Key,pl,en
        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');
            if (columns.Length >= 3)
            {
                string key = columns[0].Trim();
                string plVal = columns[1].Trim();
                string enVal = columns[2].Trim();

                plTranslations[key] = plVal;
                enTranslations[key] = enVal;
            }
        }
    }

    public void SetLanguage(string lang)
    {
        if (lang == "pl" || lang == "en")
        {
            currentLanguage = lang;
            PlayerPrefs.SetString("Language", currentLanguage);
            PlayerPrefs.Save();
        }
    }

    public string GetString(string key)
    {
        Dictionary<string, string> activeDict = (currentLanguage == "en") ? enTranslations : plTranslations;
        if (activeDict.TryGetValue(key, out string translation))
        {
            return translation;
        }

        // Fallback to Polish
        if (plTranslations.TryGetValue(key, out string fallback))
        {
            return fallback;
        }

        return key;
    }
}
