using UnityEngine;
using TMPro;

public class UINotificationHandler : MonoBehaviour
{
    public TextMeshProUGUI notificationText;
    public TextMeshProUGUI infoText;

    private void OnEnable()
    {
        GameManager.OnNotificationRequested += UpdateNotification;
        GameManager.OnSectorInfoRequested += UpdateInfo;
    }

    private void OnDisable()
    {
        GameManager.OnNotificationRequested -= UpdateNotification;
        GameManager.OnSectorInfoRequested -= UpdateInfo;
    }

    private void UpdateNotification(string msg, Color col)
    {
        notificationText.text = msg;
        notificationText.color = col;
        notificationText.gameObject.SetActive(true);
        CancelInvoke();
        Invoke("Hide", 8f);
    }

    private void UpdateInfo(string msg, Color col)
    {
        infoText.text = msg;
        infoText.color = col;
    }

    private void Hide() => notificationText.gameObject.SetActive(false);
}

