using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Unity.VisualScripting.Member;

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

    private Transform player;
    private Transform currentTarget;

    private State currentState;
    private Coroutine stunCoroutine;
    private Coroutine despawnCoroutine;
    private bool isStunned = false;
    private bool despawnIsParable = true;

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

        if(!isStunned) agent.isStopped = false;

        agent.SetDestination(currentTarget.position);
    }

    private void CheckArrivalAtLocker()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
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
        currentState = State.Investigating;
    }
    public void OffPlayerHidden()
    {
        if(despawnIsParable)
        {
            SetTarget(player);
            currentState = State.Chasing;
        }
    }

    private void StartWaitingAndDespawn()
    {
        if (despawnCoroutine != null) return;

        currentState = State.Waiting;

        despawnCoroutine = StartCoroutine(DespawnAfterDelay());
    }

    private IEnumerator DespawnAfterDelay() //Primero esperará a que la espera del despawn termine para comenzar reproducir el sonido de muerte, una vez que este suene no habra forma de parar la muerte
    {
        yield return new WaitForSeconds(despawnDelay);

        // sonido puntual
        if (audioSource != null && idleSound != null)
            audioSource.PlayOneShot(idleSound);

        despawnIsParable = false;
        yield return new WaitForSeconds(idleSound.length);

        //Reemplazar por un Destroy(gameObject); si el formato pool en gamemanager está sin aplicar
        gameObject.SetActive(false);
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

        agent.isStopped = true;
        isStunned = true;

        yield return new WaitForSeconds(duration);

        isStunned = false;
    }
    public void ResetState() // Si el spawn de este gameobject NO es un tipo POOL, no utilizar esta función de reinicio de parametros y corrutinas
    {
        // --- COROUTINES ---
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
            stunCoroutine = null;
        }

        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
            despawnCoroutine = null;
        }
        // --- CONDITIONS ---
        despawnIsParable = true;
        isStunned = false;

        // --- NAVMESH ---
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        // --- TIMERS ---
        attackTimer = 0f;

        // --- TARGETS ---
        currentTarget = null;

        // --- ESTADO ---
        currentState = State.Chasing;

        // --- PLAYER ---
        FindPlayer(); // reacquire por si cambió la referencia

        if (player != null)
        {
            SetTarget(player);
        }

        // --- AUDIO ---
        if (audioSource != null)
        {
            audioSource.Stop();
            StartLoopAudio();
        }
    }
}