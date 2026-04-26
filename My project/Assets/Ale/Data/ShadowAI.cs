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
    [SerializeField] private AudioClip idleSound;

    private Transform player;
    private Transform currentTarget;

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
        // SOLO ataca si está persiguiendo
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
            playerScript.KillPlayer(); // modular
        }
    }

    // ---------- LOCKER ----------

    public void OnPlayerHidden(Transform locker)
    {
        SetTarget(locker);
        currentState = State.Investigating;
    }

    private void StartWaitingAndDespawn()
    {
        if (despawnCoroutine != null) return;

        currentState = State.Waiting;

        if (audioSource != null && idleSound != null)
            audioSource.PlayOneShot(idleSound);

        despawnCoroutine = StartCoroutine(DespawnAfterDelay());
    }

    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(despawnDelay);
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