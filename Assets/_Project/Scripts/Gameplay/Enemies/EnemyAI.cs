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

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 startPosition;
    private float stateTimer;

    // Referencia opcional para animaciones
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>(); //
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform.position;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= detectionRange) 
                    ChangeState(State.Chasing);
                else if (CheckTimer(2f)) 
                    ChangeState(State.Patrolling);
                break;

            case State.Patrolling:
                if (distanceToPlayer <= detectionRange) 
                    ChangeState(State.Chasing);
                // Si llegó al punto de patrulla, volver a Idle para descansar
                else if (!agent.pathPending && agent.remainingDistance < 0.5f) 
                    ChangeState(State.Idle);
                break;

            case State.Chasing:
                agent.SetDestination(player.position);
                if (distanceToPlayer <= attackRange) 
                    ChangeState(State.Attacking);
                else if (distanceToPlayer > chaseStopDistance) 
                    ReturnToOrigin();
                break;

            case State.Attacking:
                agent.ResetPath();
                // Mirar al jugador mientras ataca
                Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
                transform.LookAt(lookPos);

                if (distanceToPlayer > attackRange) 
                    ChangeState(State.Chasing);
                break;
        }

        UpdateAnimation();
    }

    // Nueva función para manejar el cambio de lógica al entrar a un estado
    void ChangeState(State newState)
    {
        currentState = newState;
        stateTimer = 0;

        if (currentState == State.Patrolling)
        {
            SetRandomPatrolPoint();
        }
    }

    void SetRandomPatrolPoint()
    {
        // Buscamos un punto aleatorio dentro de un círculo
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition; // Patrullar alrededor de su zona de spawn

        NavMeshHit hit;
        // Buscamos el punto más cercano válido en el NavMesh
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    void ReturnToOrigin()
    {
        agent.SetDestination(startPosition);
        if (agent.remainingDistance < 1f) ChangeState(State.Idle);
    }

    private bool CheckTimer(float duration)
    {
        stateTimer += Time.deltaTime;
        if (stateTimer >= duration) { stateTimer = 0; return true; }
        return false;
    }

    private void UpdateAnimation()
    {
        if (anim != null)
        {
            // Enviamos la velocidad actual al Animator para caminar/correr
            //anim.SetFloat("Speed", agent.velocity.magnitude); //
        }
    }

    // Para visualizar el radio de patrulla en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPosition, patrolRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}