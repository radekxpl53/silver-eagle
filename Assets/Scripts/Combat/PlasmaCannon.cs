using UnityEngine;

public class PlasmaCannon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float fireInterval = 1.8f;
    [SerializeField] private float damage = 55f;
    [SerializeField] private int shotsBeforeOverheat = 3;
    [SerializeField] private float overheatCooldown = 4f;

    private float lastShot = -99f;
    private int shotsFired;
    private float overheatUntil;
    private Rigidbody parentRb;

    void Start()
    {
        parentRb = GetComponentInParent<Rigidbody>();
        if (muzzle == null) muzzle = transform;
    }

    public bool CanFire() => Time.time >= overheatUntil && Time.time - lastShot >= fireInterval;

    public void TryFire()
    {
        if (!CanFire()) return;

        Transform owner = parentRb != null ? parentRb.transform : transform;
        ShipStats stats = owner.GetComponent<ShipStats>();
        if (stats != null && stats.IsDestroyed) return;

        Vector3 spawnPos = muzzle.position;
        Vector3 dir = owner.CompareTag("Player")
            ? PlayerWeaponAim.GetDirection(spawnPos, owner)
            : muzzle.forward;

        lastShot = Time.time;
        shotsFired++;
        if (shotsFired >= shotsBeforeOverheat)
        {
            shotsFired = 0;
            overheatUntil = Time.time + overheatCooldown;
        }

        Vector3 shooterVelocity = parentRb != null ? parentRb.linearVelocity : Vector3.zero;
        GameObject go = ProstPool.Instance != null
            ? ProstPool.Instance.Get(spawnPos, Quaternion.LookRotation(dir))
            : Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir));

        var kinetic = go.GetComponent<HeavyKineticProjectile>();
        if (kinetic != null)
            kinetic.Launch(dir, shooterVelocity, owner, damage);
    }
}
