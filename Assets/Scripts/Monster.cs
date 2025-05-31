using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public float health = 20f;                     // 좀비 체력
    public float detectionRange = 10f;             // 플레이어 감지 범위
    public float attackRange = 2f;                 // 공격 거리
    public float wanderRadius = 10f;               // 랜덤 이동 반경
    public float wanderInterval = 3f;              // 이동 지연 시간

    public Transform player;                       // 플레이어 참조 (없을 수도 있음)
    private NavMeshAgent agent;                    // 네비게이션 에이전트
    private Animator animator;                     // 애니메이터 참조

    private float wanderTimer;                     // 랜덤 이동 타이머
    private bool isDead = false;                   // 죽음 상태 여부

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        wanderTimer = wanderInterval;

        // 플레이어가 에디터에서 할당 안됐을 경우 씬에서 자동 검색
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (isDead) return;

        // 플레이어가 있을 경우
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;
                animator.SetTrigger("Attack");
            }
            else if (distanceToPlayer <= detectionRange)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetBool("isWalking", true);
            }
            else
            {
                WanderRandomly();
            }
        }
        else // 플레이어가 없으면 무조건 자유롭게 랜덤 이동
        {
            WanderRandomly();
        }
    }

    void WanderRandomly()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            wanderTimer = 0;
        }

        agent.isStopped = false;
        animator.SetBool("isWalking", true);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;

        animator.SetBool("isDead", true);
        Destroy(gameObject, 2.4f);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
