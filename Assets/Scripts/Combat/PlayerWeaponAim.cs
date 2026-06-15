using UnityEngine;

public static class PlayerWeaponAim
{
    private static CombatAimSystem registeredSystem;

    internal static void Register(CombatAimSystem system) => registeredSystem = system;

    public static Vector3 GetDirection(Vector3 muzzlePos, Transform owner)
    {
        if (registeredSystem != null &&
            registeredSystem.TryGetFireSolution(muzzlePos, owner, out Vector3 direction, out _))
            return direction;

        TryGetAimPoint(muzzlePos, owner, 5000f, out _, out Vector3 fallback);
        return fallback;
    }

    public static bool TryGetAimPoint(Vector3 muzzlePos, Transform owner, float maxDistance, out Vector3 aimPoint, out Vector3 direction)
    {
        if (registeredSystem != null &&
            registeredSystem.TryGetFireSolution(muzzlePos, owner, out direction, out aimPoint))
            return true;

        return TryLegacyAim(muzzlePos, owner, maxDistance, out aimPoint, out direction);
    }

    private static bool TryLegacyAim(Vector3 muzzlePos, Transform owner, float maxDistance, out Vector3 aimPoint, out Vector3 direction)
    {
        Camera cam = ResolveCamera(owner);
        if (cam == null)
        {
            aimPoint = muzzlePos + owner.forward * maxDistance;
            direction = owner.forward;
            return false;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        aimPoint = ray.GetPoint(maxDistance);

        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (IsOwnCollider(hit.collider, owner)) continue;
            aimPoint = hit.point;
            direction = (aimPoint - muzzlePos).normalized;
            return true;
        }

        direction = (aimPoint - muzzlePos).normalized;
        return false;
    }

    private static Camera ResolveCamera(Transform owner)
    {
        foreach (Camera cam in owner.GetComponentsInChildren<Camera>(true))
        {
            if (cam.enabled && cam.gameObject.activeInHierarchy)
                return cam;
        }

        return Camera.main;
    }

    private static bool IsOwnCollider(Collider col, Transform owner)
    {
        return col.transform == owner || col.transform.IsChildOf(owner);
    }
}
