using UnityEngine;

public interface IDiegeticDisplay
{
    void SetCredits(float credits);
    void SetHP(float current, float max);
    void SetEnergy(float current, float max);
    void SetCargo(float current, float max);
    void ShowSectorBriefing(SectorDefinition sector);
    void ShowCRTLog(string[] entries);
    void ShowNotification(string message, Color color);
}
