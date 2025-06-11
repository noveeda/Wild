using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// NavMeshAgent를 사용하는 몬스터 AI 스크립트
/// 주요 기능: 플레이어 추적, 공격, 자유 이동, 넉백, 드롭 아이템, 사망 처리
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Monster : MonoBehaviour
{
    // === 몬스터 상태 값 ===
    public float health = 20f;              // 몬스터 체력
    public float detectionRange = 30f;      // 플레이어 탐지 범위
    public float attackRange = 5f;          // 공격 범위
    public float moveSpeed = 3.5f;          // 이동 속도

    // === 참조 변수 ===
    public Transform player;                // 추적할 플레이어
    public GameObject lootPrefab;           // 드롭 아이템 프리팹

    // === 내부 컴포넌트 ===
    private Animator animator;              // 애니메이터
    private Rigidbody rb;                   // 넉백 전용 리지드바디
    private NavMeshAgent agent;             // 네비메시 이동 담당

    // === 자유 이동 관련 ===
    private float directionTimer = 0f;
    private float directionChangeInterval = 5f; // 5초마다 새로운 위치 지정
    private Vector3 wanderTarget;               // 자유 이동 목표 지점

    // === 상태 플래그 ===
    private bool isDead = false;
    private bool isKnockedBack = false;

    // === 넉백 관련 ===
    public float knockbackDuration = 0.3f; // 넉백 지속 시간
    private float knockbackForce = 5f;     // 넉백 힘

    // === 공격 쿨타임 ===
    private float attackCooldown = 1.5f;   // 공격 간격
    private float lastAttackTime = 0f;

    void Start()
    {
        // 컴포넌트 초기화
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        // 네비메시 속도 설정
        agent.speed = moveSpeed;
        agent.angularSpeed = 360f;
        agent.acceleration = 8f;

        // 플레이어 자동 찾기
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        // 최초 자유 이동 위치 설정
        PickNewWanderTarget();
    }

    void Update()
    {
        // 사망 시 무시
        if (isDead) return;

        // 넉백 중이면 이동 멈춤
        if (isKnockedBack)
        {
            agent.isStopped = true;
            return;
        }

        // 플레이어와의 거리 측정
        float distance = player ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        // 공격 가능 거리면 공격 시도
        if (player && distance <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }

            // 공격 시 멈추고 걷기 애니메이션 중지
            agent.isStopped = true;
            animator?.SetBool("isWalking", false);
        }
        // 탐지 범위 안이면 추적
        else if (player && distance <= detectionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator?.SetBool("isWalking", true);
        }
        // 탐지 범위 밖이면 자유 이동
        else
        {
            Wander();
        }
    }

    /// <summary>
    /// 공격 실행 (애니메이션 및 플레이어 데미지)
    /// </summary>
    void Attack()
    {
        animator?.SetTrigger("Attack");

        // 플레이어에게 데미지 적용
        if (player.TryGetComponent(out Player p))
        {
            p.TakeDamage(5f);
        }
    }

    /// <summary>
    /// 자유 이동(Wander) 로직: 랜덤 위치로 이동
    /// </summary>
    void Wander()
    {
        directionTimer += Time.deltaTime;

        // 일정 시간마다 또는 도착 시 새로운 위치 설정
        if (directionTimer >= directionChangeInterval || Vector3.Distance(transform.position, wanderTarget) < 1f)
        {
            PickNewWanderTarget();
            directionTimer = 0f;
        }

        // 목표 지점으로 이동
        agent.isStopped = false;
        agent.SetDestination(wanderTarget);
        animator?.SetBool("isWalking", true);
    }

    /// <summary>
    /// 랜덤한 자유 이동 위치를 설정
    /// </summary>
    void PickNewWanderTarget()
    {
        float radius = 20f;
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
        }
        else
        {
            wanderTarget = transform.position;
        }
    }

    /// <summary>
    /// 데미지 처리 및 넉백 적용
    /// </summary>
    public void TakeDamage(float damage, Transform attacker = null)
    {
        if (isDead || attacker == null) return;

        health -= damage;

        // 넉백 방향 계산
        Vector3 dir = (transform.position - attacker.position).normalized;
        StartCoroutine(ApplyKnockback(dir));

        if (health <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 넉백 효과를 잠깐 적용한 후 원상 복구
    /// </summary>
    IEnumerator ApplyKnockback(Vector3 direction)
    {
        isKnockedBack = true;
        agent.isStopped = true;

        // 힘을 가해 넉백
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);

        // 걷기 애니메이션 정지
        animator?.SetBool("isWalking", false);

        // 넉백 지속 시간 대기
        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector3.zero;
        isKnockedBack = false;
        agent.isStopped = false;
    }

    /// <summary>
    /// 사망 처리 및 아이템 드롭
    /// </summary>
    void Die()
    {
        isDead = true;
        agent.isStopped = true;

        animator?.SetBool("isDead", true);

        // 아이템 드롭
        if (lootPrefab != null)
        {
            Instantiate(lootPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        // 일정 시간 후 삭제
        Destroy(gameObject, 2f);
    }
}
