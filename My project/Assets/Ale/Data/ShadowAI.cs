using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ShadowAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Behavior")]
    [SerializeField] private float despawnDelay = 3f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip idleSound;

    private Transform player;
    private Transform currentTarget;

    private State currentState;
    private Coroutine stunCoroutine;
    private Coroutine despawnCoroutine;

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

    // ---------- STATE MACHINE ----------

    private void HandleState()
    {
        switch (currentState)
        {
            case State.Chasing:
                FollowTarget();
                break;

            case State.Investigating:
                FollowTarget();
                CheckArrivalAtLocker();
                break;

            case State.Waiting:
                // quieto mirando
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

    // ---------- SPAWN (CLAVE) ----------

    public void Spawn(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);

        // IMPORTANTE: aseguramos que tenga player
        if (player == null)
            FindPlayer();

        StartChasing();
    }

    private void StartChasing()
    {
        if (player == null) return;

        SetTarget(player);
        currentState = State.Chasing;
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