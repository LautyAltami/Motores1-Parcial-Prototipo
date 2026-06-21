using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ShadowAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Behavior")]
    [SerializeField] private float despawnDelay = 3f;

    [Header("Combat")]
    [SerializeField] private float attackDistance = 5.5f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip idleSound; // sonido al esperar (locker)
    [SerializeField] private AudioClip loopSound; // sonido constante

    // --- NUEVO: evento estatico para avisar a quien le interese (puertas, etc) ---
    // Manda el Transform del locker donde desapareció, asi cada puerta puede
    // chequear si le corresponde a ella reaccionar o no.
    public static event Action<Transform> OnDespawn;

    private Transform player;
    private Transform currentTarget;
    private Transform lockerActual; // Guardamos referencia al locker donde nos escondimos

    private State currentState;
    private Coroutine stunCoroutine;
    private Coroutine despawnCoroutine;

    private float attackTimer = 0f;

    private enum State
    {
        Chasing,
        Investigating,
        Waiting,
        Stunned
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
    }

    void Start()
    {
        FindPlayer();
        StartChasing();
        StartLoopAudio(); // arranca sonido ambiente
    }

    void Update()
    {
        HandleState();
    }

    // ---------- INIT ----------

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    private void StartChasing()
    {
        if (player == null) return;

        SetTarget(player);
        currentState = State.Chasing;
    }

    private void StartLoopAudio()
    {
        if (audioSource != null && loopSound != null)
        {
            audioSource.clip = loopSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // ---------- STATE MACHINE ----------

    private void HandleState()
    {
        switch (currentState)
        {
            case State.Chasing:
                FollowTarget();
                TryAttack();
                break;

            case State.Investigating:
                FollowTarget();
                CheckArrivalAtLocker();
                break;

            case State.Waiting:
                break;

            case State.Stunned:
                break;
        }
    }

    // ---------- MOVEMENT ----------

    private void FollowTarget()
    {
        if (currentTarget == null)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(currentTarget.position);
    }

    private void CheckArrivalAtLocker()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            LookAtTarget();

            if (currentState != State.Waiting)
                StartWaitingAndDespawn();
        }
    }

    private void LookAtTarget()
    {
        if (currentTarget == null) return;

        Vector3 dir = (currentTarget.position - transform.position).normalized;
        dir.y = 0;
        transform.forward = dir;
    }

    private void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    // ---------- COMBAT ----------

    private void TryAttack()
    {
        if (player == null || currentState != State.Chasing) return;

        float distance = Vector3.Distance(transform.position, player.position);

        attackTimer -= Time.deltaTime;

        if (distance <= attackDistance && attackTimer <= 0f)
        {
            AttackPlayer();
            attackTimer = attackCooldown;
        }
    }

    private void AttackPlayer()
    {
        var playerScript = player.GetComponent<PlayerControllerScript>();

        if (playerScript != null)
        {
            playerScript.KillPlayer();
        }
    }

    // ---------- LOCKER ----------

    public void OnPlayerHidden(Transform locker)
    {
        SetTarget(locker);
        lockerActual = locker;
        currentState = State.Investigating;
    }

    private void StartWaitingAndDespawn()
    {
        if (despawnCoroutine != null) return;

        currentState = State.Waiting;

        // sonido puntual
        if (audioSource != null && idleSound != null)
            audioSource.PlayOneShot(idleSound);

        despawnCoroutine = StartCoroutine(DespawnAfterDelay());
    }

    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(despawnDelay);

        // --- NUEVO: avisamos a quien este escuchando (ej: las puertas dobles) ---
        // mandamos el locker especifico para que cada puerta sepa si le toca a ella o no.
        OnDespawn?.Invoke(lockerActual);

        Destroy(gameObject);
    }

    // ---------- STUN ----------

    public void Stun(float duration)
    {
        if (player != null)
            SetTarget(player);

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        currentState = State.Stunned;

        agent.isStopped = true;
        agent.ResetPath();

        yield return new WaitForSeconds(duration);

        currentState = State.Chasing;
    }
}