using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ShadowAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Behavior")]
    [SerializeField] private float despawnDelay = 3f;

    [Header("Combat")]
    [SerializeField] private float attackDistance = 5.5f;

    [Header("Locker")]
    [SerializeField] private float lockerReachDistance = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip loopSound;

    public static event Action<Transform> OnDespawn;

    private Transform player;
    private Transform currentTarget;
    private Transform lockerActual;
    private bool playerIsHidden;

    private State currentState;
    private Coroutine stunCoroutine;
    private Coroutine despawnCoroutine;

    
    // Le permite al SanityManager saber si el monstruo está en estado Chasing
    public bool IsChasing => currentState == State.Chasing;

    private string currentAnim = "";

    private enum State
    {
        Chasing,
        Investigating,
        Waiting,
        Stunned,
        Dead
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.updateRotation = true;
    }

    private void Start()
    {
        FindPlayer();
        StartChasing();
        StartLoopAudio();
    }

    private void Update()
    {
        HandleState();
        UpdateAnimations();
    }

    // ---------------- ANIMATIONS ----------------

    private void UpdateAnimations()
    {
        if (animator == null)
            return;

        string anim = currentAnim;

        switch (currentState)
        {
            case State.Chasing:
            case State.Investigating:
                anim = "Walking";
                break;

            case State.Waiting:
            case State.Stunned:
                anim = "Idle";
                break;

            case State.Dead:
                anim = "Death";
                break;
        }

        PlayAnim(anim);
    }

    private void PlayAttack()
    {
        PlayAnim("Zombie Attack");
    }

    private void PlayDeath()
    {
        currentState = State.Dead;
        PlayAnim("Death");
    }

    private void PlayAnim(string name)
    {
        if (currentAnim == name)
            return;

        animator.Play(name);
        currentAnim = name;
    }

    // ---------------- INIT ----------------

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    private void StartChasing()
    {
        if (player == null)
            return;

        agent.isStopped = false;

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

    // ---------------- STATE MACHINE ----------------

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
                agent.isStopped = true;
                break;

            case State.Stunned:
                agent.isStopped = true;
                break;

            case State.Dead:
                agent.isStopped = true;
                break;
        }
    }

    // ---------------- MOVEMENT ----------------

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
        if (currentTarget == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                currentTarget.position);

        if (distance <= lockerReachDistance)
        {
            agent.isStopped = true;
            agent.ResetPath();

            LookAtTarget();

            if (currentState != State.Waiting)
            {
                StartWaitingAndDespawn();
            }
        }
    }

    private void LookAtTarget()
    {
        if (currentTarget == null)
            return;

        Vector3 dir =
            (currentTarget.position - transform.position).normalized;

        dir.y = 0;

        if (dir != Vector3.zero)
            transform.forward = dir;
    }

    private void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    // ---------------- COMBAT ----------------

    private void TryAttack()
    {
        if (player == null || currentState != State.Chasing)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= attackDistance)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        agent.isStopped = true;
        PlayAttack();

        var playerScript =
            player.GetComponent<PlayerControllerScript>();

        if (playerScript != null)
        {
            playerScript.KillPlayer();
        }
    }

    // ---------------- LOCKER ----------------

    public void OnPlayerHidden(Transform locker)
    {
        playerIsHidden = true;

        lockerActual = locker;

        // Si ya está esperando en ESTE locker, ignoramos
        if (currentState == State.Waiting)
            return;

        SetTarget(locker);
        currentState = State.Investigating;
    }

    public void OnPlayerExitedLocker()
    {
        playerIsHidden = false;

        if (currentState != State.Waiting)
            return;

        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
            despawnCoroutine = null;
        }

        StartChasing();
    }

    private void StartWaitingAndDespawn()
    {
        if (despawnCoroutine != null)
            return;

        currentState = State.Waiting;

        agent.isStopped = true;
        agent.ResetPath();

        if (audioSource != null &&
            idleSound != null)
        {
            if (audioSource != null && idleSound != null)
            {
                audioSource.PlayOneShot(idleSound);
            }
        }

        despawnCoroutine =
            StartCoroutine(DespawnAfterDelay());
    }

    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(despawnDelay);

        Debug.Log("Voy a morir");

        // OnDespawn?.Invoke(lockerActual);
        PlayDeath();

        yield return new WaitForSeconds(1.5f);

        Debug.Log("Me destruyo");

        Destroy(gameObject);
    }

    // ---------------- STUN ----------------

    public void Stun(float duration)
    {
        if (player != null)
        {
            SetTarget(player);
        }

        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine =
            StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        currentState = State.Stunned;

        agent.isStopped = true;
        agent.ResetPath();

        yield return new WaitForSeconds(duration);

        StartChasing();
    }
}