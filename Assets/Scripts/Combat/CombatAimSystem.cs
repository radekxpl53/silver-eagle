using UnityEngine;
using UnityEngine.InputSystem;

public class CombatAimSystem : MonoBehaviour
{
    public static CombatAimSystem Instance { get; private set; }

    [Header("Zasięg")]
    [SerializeField] private float maxAimDistance = 6000f;
    [SerializeField] private float lockBreakDistance = 1400f;

    [Header("Namierzanie")]
    [SerializeField] private float lockAcquireAngle = 10f;
    [SerializeField] private float autoSuggestAngle = 4f;

    [Header("Ograniczenia strzału")]
    [SerializeField] private float minForwardDot = 0.15f;
    [SerializeField] private LayerMask aimMask = ~0;

    private Transform owner;
    private HeavyKineticLauncher launcher;
    private PlasmaCannon plasmaCannon;
    private Turret[] turrets;

    private Camera aimCamera;
    private Transform lockedTarget;
    private Vector3 aimPoint;
    private Vector3 fireDirection;
    private bool hasAim;
    private bool hasLock;

    public bool HasLock => hasLock && lockedTarget != null;
    public Transform LockedTarget => lockedTarget;
    public Vector3 AimPoint => aimPoint;
    public Vector3 FireDirection => fireDirection;
    public bool HasAim => hasAim;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        owner = transform;
        PlayerWeaponAim.Register(this);

        launcher = GetComponent<HeavyKineticLauncher>();
        plasmaCannon = GetComponent<PlasmaCannon>();
        turrets = GetComponentsInChildren<Turret>(true);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            PlayerWeaponAim.Register(null);
        }
    }

    private void Update()
    {
        if (!CanAim()) return;

        ResolveCamera();
        HandleLockInput();
        ValidateLock();
        ComputeAim();
        SyncTurrets();
    }

    private bool CanAim()
    {
        if (GameManager.Instance == null) return false;
        if (StationProximity.RequiresCursor) return false;

        GameState state = GameManager.Instance.currentState;
        return state == GameState.Exploration || state == GameState.Fighting;
    }

    private void ResolveCamera()
    {
        if (aimCamera != null && aimCamera.enabled && aimCamera.gameObject.activeInHierarchy)
            return;

        aimCamera = null;
        foreach (Camera cam in GetComponentsInChildren<Camera>(true))
        {
            if (cam.enabled && cam.gameObject.activeInHierarchy)
            {
                aimCamera = cam;
                return;
            }
        }

        aimCamera = Camera.main;
    }

    private void HandleLockInput()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.tabKey.wasPressedThisFrame) return;

        if (lockedTarget == null)
            TryAcquireLock();
        else
            CycleLock();
    }

    private void TryAcquireLock()
    {
        Transform best = FindBestTargetInCone(lockAcquireAngle);
        if (best != null)
            lockedTarget = best;
    }

    private void CycleLock()
    {
        var candidates = CollectVisibleEnemies();
        if (candidates.Count == 0)
        {
            lockedTarget = null;
            return;
        }

        int index = lockedTarget != null ? candidates.IndexOf(lockedTarget) : -1;
        index = (index + 1) % candidates.Count;
        lockedTarget = candidates[index];
    }

    private void ValidateLock()
    {
        if (lockedTarget == null)
        {
            hasLock = false;
            return;
        }

        if (!lockedTarget.gameObject.activeInHierarchy)
        {
            lockedTarget = null;
            hasLock = false;
            return;
        }

        ShipStats stats = lockedTarget.GetComponentInParent<ShipStats>();
        if (stats != null && stats.IsDestroyed)
        {
            lockedTarget = null;
            hasLock = false;
            return;
        }

        float dist = Vector3.Distance(owner.position, lockedTarget.position);
        if (dist > lockBreakDistance)
        {
            lockedTarget = null;
            hasLock = false;
            return;
        }

        hasLock = true;
    }

    private void ComputeAim()
    {
        hasAim = false;
        fireDirection = owner.forward;
        aimPoint = owner.position + owner.forward * 500f;

        if (aimCamera == null) return;

        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 muzzle = GetMuzzlePosition();

        if (lockedTarget != null)
        {
            float speed = GetProjectileSpeed();
            aimPoint = CalculateLead(muzzle, lockedTarget, speed);
            hasAim = true;
        }
        else
        {
            Transform suggested = FindBestTargetInCone(autoSuggestAngle);
            if (suggested != null)
            {
                aimPoint = CalculateLead(muzzle, suggested, GetProjectileSpeed());
                hasAim = true;
            }
            else if (TryRaycastAim(ray, out Vector3 hitPoint))
            {
                aimPoint = hitPoint;
                hasAim = true;
            }
            else
            {
                aimPoint = ray.GetPoint(maxAimDistance);
                hasAim = true;
            }
        }

        fireDirection = (aimPoint - muzzle).sqrMagnitude > 0.01f
            ? (aimPoint - muzzle).normalized
            : owner.forward;
    }

    private void SyncTurrets()
    {
        if (!hasAim) return;

        foreach (Turret turret in turrets)
        {
            if (turret == null || !turret.isActiveAndEnabled) continue;
            turret.SetAimPoint(aimPoint);
            if (hasLock)
                turret.target = lockedTarget;
        }
    }

    public bool TryGetFireSolution(Vector3 muzzlePos, Transform shooter, out Vector3 direction, out Vector3 interceptPoint)
    {
        direction = shooter.forward;
        interceptPoint = shooter.position + shooter.forward * 300f;

        if (!hasAim || shooter != owner)
            return false;

        direction = (aimPoint - muzzlePos).normalized;
        if (direction.sqrMagnitude < 0.001f)
            return false;

        if (Vector3.Dot(direction, shooter.forward) < minForwardDot)
            return false;

        interceptPoint = aimPoint;
        return true;
    }

    public bool TryProjectToScreen(Vector3 worldPoint, out Vector2 screenPoint)
    {
        screenPoint = Vector2.zero;
        if (aimCamera == null) return false;

        Vector3 viewport = aimCamera.WorldToViewportPoint(worldPoint);
        if (viewport.z <= 0f) return false;

        screenPoint = new Vector2(viewport.x, viewport.y);
        return true;
    }

    private bool TryRaycastAim(Ray ray, out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;
        RaycastHit[] hits = Physics.RaycastAll(ray, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (IsOwnCollider(hit.collider)) continue;
            hitPoint = hit.point;
            return true;
        }

        return false;
    }

    private Transform FindBestTargetInCone(float maxAngle)
    {
        Transform best = null;
        float bestScore = float.MaxValue;

        if (GameManager.Instance == null || aimCamera == null) return null;

        Vector3 camPos = aimCamera.transform.position;
        Vector3 camFwd = aimCamera.transform.forward;

        foreach (EnemyAI enemy in GameManager.Instance.activeEnemies)
        {
            if (enemy == null) continue;

            ShipStats stats = enemy.GetComponent<ShipStats>();
            if (stats != null && stats.IsDestroyed) continue;

            Vector3 toEnemy = enemy.transform.position - camPos;
            float dist = toEnemy.magnitude;
            if (dist < 5f || dist > lockBreakDistance) continue;

            float angle = Vector3.Angle(camFwd, toEnemy);
            if (angle > maxAngle) continue;

            float score = angle + dist * 0.01f;
            if (score < bestScore)
            {
                bestScore = score;
                best = enemy.transform;
            }
        }

        return best;
    }

    private System.Collections.Generic.List<Transform> CollectVisibleEnemies()
    {
        var list = new System.Collections.Generic.List<Transform>();
        if (GameManager.Instance == null) return list;

        foreach (EnemyAI enemy in GameManager.Instance.activeEnemies)
        {
            if (enemy == null) continue;
            ShipStats stats = enemy.GetComponent<ShipStats>();
            if (stats != null && stats.IsDestroyed) continue;
            if (Vector3.Distance(owner.position, enemy.transform.position) > lockBreakDistance) continue;
            list.Add(enemy.transform);
        }

        list.Sort((a, b) =>
            Vector3.Distance(owner.position, a.position).CompareTo(Vector3.Distance(owner.position, b.position)));
        return list;
    }

    private Vector3 GetMuzzlePosition()
    {
        if (launcher != null) return launcher.MuzzlePosition;
        if (plasmaCannon != null) return plasmaCannon.transform.position;
        return owner.position + owner.forward * 2f;
    }

    private float GetProjectileSpeed()
    {
        if (launcher != null) return launcher.GetProjectileSpeed();
        return 40f;
    }

    private static Vector3 CalculateLead(Vector3 shooterPos, Transform target, float projectileSpeed)
    {
        if (target == null || projectileSpeed <= 0.01f)
            return shooterPos;

        Rigidbody targetRb = target.GetComponentInParent<Rigidbody>();
        Vector3 velocity = targetRb != null ? targetRb.linearVelocity : Vector3.zero;
        Vector3 predicted = target.position;

        for (int i = 0; i < 3; i++)
        {
            float time = Vector3.Distance(shooterPos, predicted) / projectileSpeed;
            predicted = target.position + velocity * time;
        }

        return predicted;
    }

    private bool IsOwnCollider(Collider col)
    {
        return col.transform == owner || col.transform.IsChildOf(owner);
    }
}
