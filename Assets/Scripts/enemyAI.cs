using UnityEngine;

public class enemyAI : MonoBehaviour
{
    [Header("Parametry Fizyczne (bazowane na Excelu)")]
    // Przeciwnik jest l�ejszy (40t) i ma lepszy stosunek mocy do masy ni� Tw�j transportowiec
    public float mass = 40000f;          // kg
    public float mainThrust = 600000f;   // N (Si�a ci�gu)
    public float rotationSpeed = 2.5f;   // Pr�dko�� obrotu (zamiast Si�y Manewrowej dla uproszczenia AI)
    public float linearDrag = 0.5f;      // Op�r liniowy (z Twojego pliku)
    public float angularDrag = 1.2f;     // Op�r obrotu (z Twojego pliku)

    [Header("Logika AI")]
    public Transform playerTarget;
    public float detectionRange = 500f;  // Du�y zasi�g w kosmosie
    public float attackRange = 100f;     // Dystans otwarcia ognia
    public float stopDistance = 30f;     // Dystans hamowania (�eby si� nie zderzy�)

    private Rigidbody rb;
    private float nextAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Aplikujemy ustawienia fizyczne do Rigidbody
        rb.mass = mass;
        rb.linearDamping = linearDrag;         // "Hamowanie" w przestrzeni (pseudo-atmosfera lub systemy stabilizacji)
        rb.angularDamping = angularDrag; // Stabilizacja obrotu
        rb.useGravity = false;        // Wy��czamy grawitacj� (jeste�my w kosmosie!)

        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
    }

    void FixedUpdate() // Fizyk� obliczamy w FixedUpdate!
    {
        if (playerTarget == null) return;

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance < detectionRange)
        {
            FaceTarget(); // Obracaj si� do gracza
            MoveToTarget(distance); // Zarz�dzaj ci�giem silnik�w

            if (distance < attackRange)
            {
                TryAttack();
            }
        }
    }

    void FaceTarget()
    {
        // Oblicz kierunek do gracza
        Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;

        // P�ynny obr�t w stron� gracza (Quaternion.Slerp)
        // W pe�nej symulacji u�yliby�my AddTorque, ale dla AI Slerp jest stabilniejszy
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    void MoveToTarget(float distance)
    {
        // Sprawd�, czy dzi�b statku jest skierowany (mniej wi�cej) w stron� gracza
        float angleToPlayer = Vector3.Angle(transform.forward, playerTarget.position - transform.position);

        // Je�li patrzymy na gracza i jeste�my za daleko -> PE�NA MOC
        if (distance > stopDistance && angleToPlayer < 20f)
        {
            // F = ma (Unity robi to za nas przez AddForce)
            rb.AddRelativeForce(Vector3.forward * mainThrust);
        }
        // Je�li jeste�my za blisko -> HAMOWANIE (Wsteczny ci�g)
        else if (distance <= stopDistance)
        {
            // Symulacja hamowania (reverse thrusters)
            // U�ywamy si�y hamowania, np. 50% g��wnego ci�gu
            rb.AddRelativeForce(Vector3.back * (mainThrust * 0.5f));
        }
    }

    void TryAttack()
    {
        if (Time.time > nextAttackTime)
        {
            // Raycast, �eby sprawdzi� czy faktycznie mamy "czysty strza�"
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
            {
                if (hit.transform == playerTarget)
                {
                    Debug.Log("Pew Pew! Strza� laserem w gracza.");
                    // Tu wstawisz instancjonowanie pocisku
                    nextAttackTime = Time.time + 1.0f; // Strza� co 1 sekund�
                }
            }
        }
    }
}
