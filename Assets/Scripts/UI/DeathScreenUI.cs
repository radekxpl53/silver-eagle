using UnityEngine;

public class DeathScreenUI : MonoBehaviour
{
    public void OnRestartButtonClicked()
    {
        GameManager.Instance.RespawnAtBase();

        Debug.Log("Przycisk Restart kliknięty - powrót do bazy.");
    }
}