using UnityEngine;
using UnityEngine.AI;

public class ShadowAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Detection")]
    [SerializeField] private float detectionRange;

    private Transform player;
    private Transform currentTarget;

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
            Debug.LogWarning("Player not found. Make sure it has the 'Player' tag.");
        }
    }

    // ---------- Detection ----------

    private void HandleDetection()
    {
        if (IsPlayerInRange())
        {
            if (currentTarget != player)
            {
                SetTarget(player);
            }
        }
        else
        {
            if (currentTarget != null)
            {
                SetTarget(null);
            }
        }
    }

    private bool IsPlayerInRange()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= detectionRange;
    }

    // ---------- Movement ----------

    private void FollowTarget()
    {
        if (currentTarget == null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(currentTarget.position);
    }

    // ---------- Public API ----------

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }
    // ---------- Gizmos ----------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}