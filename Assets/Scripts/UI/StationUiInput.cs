using UnityEngine.EventSystems;

public static class StationUiInput
{
    public static bool BlocksWeaponInput =>
        StationProximity.RequiresCursor || IsPointerOverUi();

    public static bool IsPointerOverUi()
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject(-1);
    }
}
