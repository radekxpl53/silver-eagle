using UnityEngine;

public class Restart : MonoBehaviour
{
    public void RestartMethod()
    {
        PlayerData.Instance.ResetData();
        GameObject.FindGameObjectWithTag("Player").transform.position = PlayerData.Instance.position;
    }
}
