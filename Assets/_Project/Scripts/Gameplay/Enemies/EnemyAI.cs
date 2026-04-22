using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Patrolling, Chasing, Attacking }
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

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 startPosition;
    private float stateTimer;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Buscamos componentes en el padre
        agent = GetComponentInParent<NavMeshAgent>();

        // Componentes en este objeto (Visuals)
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        if (transform.parent != null)
            startPosition = transform.parent.position;
        else
            startPosition = transform.position;
    }

    void Update()
    {
        if (player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        HandleSpriteFlip();

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= detectionRange) ChangeState(State.Chasing);
                else if (CheckTimer(2f)) ChangeState(State.Patrolling);
                break;

            case State.Patrolling:
                if (distanceToPlayer <= detectionRange) ChangeState(State.Chasing);
                else if (!agent.pathPending && agent.remainingDistance < 0.5f) ChangeState(State.Idle);
                break;

            case State.Chasing:
                agent.isStopped = false;
                agent.SetDestination(player.position);
                if (distanceToPlayer <= attackRange) ChangeState(State.Attacking);
                else if (distanceToPlayer > chaseStopDistance) ReturnToOrigin();
                break;

            case State.Attacking:
                agent.isStopped = true; // Se detiene para no empujar al jugador
                if (distanceToPlayer > attackRange + 0.5f) // Pequeño margen para no alternar estados brusco
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
        float directionX = (player.position.x - transform.position.x);

        // true o false dependiendo de hacia donde mira tu dibujo original
        spriteRenderer.flipX = (directionX > 0);
    }

    void ChangeState(State newState)
    {
        currentState = newState;
        stateTimer = 0;
        if (currentState == State.Patrolling) SetRandomPatrolPoint();

        if (currentState == State.Attacking && anim != null)
        {
            anim.SetTrigger("Attack");
        }
    }

    // Se llama desde el Animation Event
    public void PerformDamage()
    {
        Debug.Log("Evento de daño disparado"); // Mira la consola para ver si esto sale
        if (attackPoint == null) return;

        // Cambiamos a OverlapSphere para detectar mejor en 3D
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
            anim.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
        }
    }

    // --- REVISIÓN DE GIZMOS ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
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

    void ReturnToOrigin()
    {
        agent.SetDestination(startPosition);
        if (agent.remainingDistance < 1f) ChangeState(State.Idle);
    }

    private bool CheckTimer(float duration)
    {
        stateTimer += Time.deltaTime;
        return stateTimer >= duration;
    }
}