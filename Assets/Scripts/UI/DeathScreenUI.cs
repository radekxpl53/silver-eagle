using UnityEngine;

public class DeathScreenUI : MonoBehaviour
{
    public void OnRestartButtonClicked()
    {
        GameManager.Instance.RespawnAtBase();

        Debug.Log("Przycisk Restart kliknięty - powrót do bazy.");
    }
    void Start() {
        GameManager.Instance.RegisterDeathScreen(this.gameObject);
        this.gameObject.SetActive(false); // Wyłącz go na starcie
    }
}