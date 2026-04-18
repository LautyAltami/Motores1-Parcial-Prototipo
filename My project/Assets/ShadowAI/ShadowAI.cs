using UnityEngine;
using UnityEngine.AI;

public class ShadowAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Vision")]
    [SerializeField, Range(0f, 100f)] private float viewDistance = 20f;
    [SerializeField, Range(0f, 60f)] private float viewAngle = 30f;
    [SerializeField] private float eyeHeight = 1f;

    private Transform player;
    private Transform currentTarget;

    private bool hasDetectedPlayer = false;

    void Awake()
    {
        InitializeAgent();
    }

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        HandleDetection();
        FollowTarget();
    }

    // ---------- Initialization ----------

    private void InitializeAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true; // usamos rotación automática
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found. Assign 'Player' tag.");
        }
    }

    // ---------- Detection ----------

    private void HandleDetection()
    {
        if (player == null) return;

        if (!hasDetectedPlayer && CanSeePlayer())
        {
            hasDetectedPlayer = true;
            SetTarget(player);
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        // Distance check
        if (distance > viewDistance) return false;

        // Angle check
        direction.Normalize();
        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > viewAngle) return false;

        // Raycast check
        Vector3 origin = transform.position + Vector3.up * eyeHeight;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, viewDistance))
        {
            return hit.transform.CompareTag("Player");
        }

        return false;
    }

    // ---------- Movement ----------

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

    // ---------- Target ----------

    private void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    // ---------- Gizmos ----------

    void OnDrawGizmosSelected()
    {
        Vector3 left = Quaternion.Euler(0, -viewAngle, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + left * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + right * viewDistance);
    }
}