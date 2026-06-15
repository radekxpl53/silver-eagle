using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Button fightButton;
    [SerializeField] private Button fleeButton;

    void Awake()
    {
        if (panelRoot != null) panelRoot.SetActive(false);

        if (fightButton != null)
            fightButton.onClick.AddListener(() => Answer(true));
        if (fleeButton != null)
            fleeButton.onClick.AddListener(() => Answer(false));
    }

    void OnEnable() => GameEvents.OnCombatPromptShown += ShowPrompt;
    void OnDisable() => GameEvents.OnCombatPromptShown -= ShowPrompt;

    private void ShowPrompt(Transform enemyContext)
    {
        if (panelRoot != null) panelRoot.SetActive(true);
        if (promptText != null)
            promptText.text = "Wykryto wroga!\nWalcz / Uciekaj";
    }

    private void Answer(bool fight)
    {
        if (CombatPromptSystem.Instance != null)
            CombatPromptSystem.Instance.AnswerCombatPrompt(fight);

        if (panelRoot != null) panelRoot.SetActive(false);
    }
}
