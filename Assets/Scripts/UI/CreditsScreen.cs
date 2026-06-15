using TMPro;
using UnityEngine;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI creditsBody;

    public void SetBody(TextMeshProUGUI body) => creditsBody = body;

    void OnEnable()
    {
        if (creditsBody == null) return;

        creditsBody.text =
            "SILVER EAGLE\n\n" +
            "Projekt: Zespół Silver Eagle\n" +
            "Silnik: Unity 6000.3\n" +
            "Audio: FMOD\n" +
            "AI wróg: EnemyAI (Kaparee)\n\n" +
            "Programowanie, design, lore — zespół produkcyjny\n" +
            "Dziękujemy za grę!";
    }
}
