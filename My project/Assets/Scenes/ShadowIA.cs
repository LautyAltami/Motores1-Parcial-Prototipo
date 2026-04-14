using UnityEngine;
using UnityEngine.AI;

public class ShadowAI : MonoBehaviour
{
    private NavMeshAgent agent;       // Handles movement using NavMesh
    private Transform currentTarget;  // Target to follow (Player)

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
        FollowTarget();
    }

    // Initializes and caches the NavMeshAgent component
    private void InitializeAgent()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Finds the Player in the scene using its tag
    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            SetTarget(playerObj.transform);
        }
        else
        {
            Debug.LogWarning("Player not found. Make sure it has the 'Player' tag.");
        }
    }

    // Moves the agent towards the current target
    private void FollowTarget()
    {
        if (currentTarget == null) return;

        agent.SetDestination(currentTarget.position);
    }

    // Public method to assign a target dynamically (modular use)
    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }
}