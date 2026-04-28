using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Añadimos un estado Returning para mayor claridad
    public enum State { Idle, Patrolling, Chasing, Attacking, Returning }
    public State currentState = State.Idle;

    [Header("Parameters")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float patrolRadius = 15f;
    public float chaseStopDistance = 20f;

    [Header("Combat Settings")]
    public float damageDealt = 15f;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask playerLayer;
    public LayerMask obstructionMask;
    public float attackCooldown = 1.5f; // Tiempo entre ataques
    private float lastAttackTime;

    [Header("SFX hit player")]
    public AudioSource audioSource;
    public AudioClip hitClip;

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 startPosition;
    private float stateTimer;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = FindFirstObjectByType<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        startPosition = (transform.parent != null) ? transform.parent.position : transform.position;
    }

    void Update()
    {
        if (player == null || agent == null) return;

        float pathDistance = GetNavMeshPathDistance(player.position);
        bool canSee = CanSeePlayer();

        // LLAMADA MEJORADA: Ahora decide el flip según el movimiento
        HandleSpriteFlip();

        switch (currentState)
        {
            case State.Idle:
                if (canSee && pathDistance <= detectionRange && pathDistance != -1f)
                    ChangeState(State.Chasing);
                else if (CheckTimer(5f))
                    ChangeState(State.Patrolling);
                break;

            case State.Patrolling:
                // 1. Si ve al jugador, persigue (esto ya estaba bien)
                if (canSee && pathDistance <= detectionRange && pathDistance != -1f)
                {
                    ChangeState(State.Chasing);
                }
                // 2. Condición de llegada: Si está cerca del destino O si el agente se detuvo
                else if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance + 0.1f || agent.velocity.sqrMagnitude < 0.01f))
                {
                    ChangeState(State.Idle);
                }
                // 3. Failsafe: Si lleva más de 10 segundos patrullando y no ha llegado, vuelve a Idle
                else if (CheckTimer(10f))
                {
                    ChangeState(State.Idle);
                }
                break;

            case State.Chasing:
                agent.isStopped = false;
                agent.SetDestination(player.position);

                if (pathDistance > chaseStopDistance || pathDistance == -1f)
                    ChangeState(State.Returning); // Cambiamos a Returning en lugar de solo llamar la función
                else if (pathDistance <= attackRange)
                    ChangeState(State.Attacking);
                break;

            case State.Attacking:
                agent.isStopped = true;

                // Mirar siempre al jugador mientras ataca
                HandleSpriteFlip();

                // Reproduce el sonido de daño al jugador
                if (audioSource != null && hitClip != null)
                    audioSource.PlayOneShot(hitClip);

                // Lógica de repetición de ataque
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    // Disparamos la animación (esto activará PerformDamage vía Animation Event si lo tienes así)
                    if (anim != null) anim.SetTrigger("Attack");

                    lastAttackTime = Time.time;
                    // Si no usas Animation Events, puedes llamar a PerformDamage() aquí directamente
                }

                // Si el jugador se aleja, volver a perseguir
                if (pathDistance > attackRange + 0.5f || pathDistance == -1f)
                {
                    ChangeState(State.Chasing);
                }
                break;

            case State.Returning:
                agent.isStopped = false;
                agent.SetDestination(startPosition);

                // Verificamos si llegó comparando la distancia Y si el agente ya se detuvo
                bool hasReachedStart = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f;
                bool isStationary = agent.velocity.sqrMagnitude < 0.01f;

                if (hasReachedStart || isStationary)
                {
                    ChangeState(State.Idle);
                }
                // Si vuelve a ver al jugador mientras regresa, lo persigue
                else if (canSee && pathDistance <= detectionRange && pathDistance != -1f)
                {
                    ChangeState(State.Chasing);
                }
                break;
        }

        UpdateAnimation();
    }

    void HandleSpriteFlip()
    {
        if (spriteRenderer == null) return;

        // Si se está moviendo significativamente, mira hacia donde camina
        if (agent.velocity.magnitude > 0.1f)
        {
            spriteRenderer.flipX = (agent.velocity.x > 0);
        }
        // Si está quieto (ej. atacando), mira hacia el jugador
        else if (currentState == State.Attacking || currentState == State.Chasing)
        {
            float directionX = (player.position.x - transform.position.x);
            spriteRenderer.flipX = (directionX > 0);
        }
    }

    // --- EL RESTO DE FUNCIONES (CanSeePlayer, GetNavMeshPathDistance, etc.) SE MANTIENEN IGUAL ---

    float GetNavMeshPathDistance(Vector3 targetPos)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(targetPos, path))
        {
            if (path.status != NavMeshPathStatus.PathComplete) return -1f;
            float distance = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++)
                distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            return distance;
        }
        return -1f;
    }

    bool CanSeePlayer()
    {
        Vector3 eyeLevel = transform.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = (player.position - eyeLevel).normalized;
        if (Physics.Raycast(eyeLevel, directionToPlayer, out RaycastHit hit, detectionRange, playerLayer | obstructionMask))
        {
            if (hit.transform.CompareTag("Player")) return true;
        }
        return false;
    }

    void ChangeState(State newState)
    {
        currentState = newState;
        stateTimer = 0;
        if (currentState == State.Patrolling) SetRandomPatrolPoint();
        if (currentState == State.Attacking && anim != null) anim.SetTrigger("Attack");
    }

    public void PerformDamage()
    {
        if (attackPoint == null) return;
        Collider[] hitColliders = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth health = hit.GetComponent<PlayerHealth>();
                if (health != null) health.TakeDamage(damageDealt);
            }
        }
    }

    void UpdateAnimation()
    {
        if (anim != null && agent != null)
        {
            anim.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !agent.isStopped);
        }
    }

    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = CanSeePlayer() ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, player.position);
        }
        Gizmos.color = Color.blue;
        if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    void SetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
            agent.SetDestination(hit.position);
    }

    private bool CheckTimer(float duration)
    {
        stateTimer += Time.deltaTime;
        return stateTimer >= duration;
    }
}