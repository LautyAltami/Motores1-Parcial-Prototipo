using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ShadowAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Vision")]
    [SerializeField, Range(0f, 50f)] private float viewDistance = 20f;
    [SerializeField, Range(0f, 90f)] private float viewAngle = 90f;
    [SerializeField] private float viewHeight = 1f;

    [Header("Behavior")]
    [SerializeField] private float despawnDelay = 3f;

    private Transform player;
    private Transform currentTarget;

    private State currentState = State.Idle;

    private Coroutine stunCoroutine;

    private enum State
    {
        Idle,
        Chasing,
        Investigating,
        Stunned
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Player not found.");
    }

    void Update()
    {
        HandleState();
    }

    // ---------- STATE MACHINE ----------

    private void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                DetectPlayer();
                break;

            case State.Chasing:
                FollowTarget();
                break;

            case State.Investigating:
                FollowTarget();
                CheckArrival();
                break;

            case State.Stunned:
                break;
        }
    }

    // ---------- DETECTION ----------

    private void DetectPlayer()
    {
        if (player == null) return;

        if (CanSeePlayer())
        {
            SetTarget(player);
            currentState = State.Chasing;
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        if (distance > viewDistance) return false;

        direction.Normalize();
        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > viewAngle) return false;

        Vector3 origin = transform.position + Vector3.up * viewHeight;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, viewDistance))
        {
            return hit.transform.CompareTag("Player");
        }

        return false;
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

    private void CheckArrival()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
        }
    }

    private void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    // ---------- API (INTEGRACIÓN CON OTROS SISTEMAS) ----------

    public void OnPlayerHidden(Transform locker)
    {
        SetTarget(locker);
        currentState = State.Investigating;

        StartCoroutine(DespawnAfterDelay());
    }

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

    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(despawnDelay);
        Destroy(gameObject);
    }

    // ---------- GIZMOS ----------

    void OnDrawGizmosSelected()
    {
        Vector3 left = Quaternion.Euler(0, -viewAngle, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + left * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + right * viewDistance);
    }
}