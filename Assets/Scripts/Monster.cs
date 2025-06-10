using UnityEngine;
using System.Collections;

/// <summary>
/// 몬스터의 AI를 담당하는 스크립트. 플레이어를 탐지하고 추적하며, 일정 거리 내에서는 공격하고
/// 피해를 입으면 넉백되고, 사망 시 아이템을 드랍한다.
/// </summary>
public class Monster : MonoBehaviour
{
    // === 몬스터 기본 속성 ===
    public float health = 20f;                    // 몬스터 체력
    public float detectionRange = 30f;            // 플레이어를 인식하는 범위
    public float attackRange = 5f;                // 공격 범위
    public float moveSpeed = 2f;                  // 이동 속도

    public Transform player;                      // 타겟이 되는 플레이어
    public GameObject lootPrefab;                 // 드랍 아이템 프리팹

    // === 컴포넌트 ===
    private Animator animator;                    // 애니메이터
    private Rigidbody rb;                         // 리지드바디

    // === 랜덤 이동 관련 변수 ===
    private float directionTimer = 0f;            // 방향 유지 시간 측정용
    private float directionChangeInterval = 5f;   // 일정 시간마다 방향 변경
    private Vector3 moveDirection;                // 현재 이동 방향

    // === 상태 관련 변수 ===
    private bool isDead = false;                  // 사망 여부
    private bool isKnockedBack = false;           // 넉백 상태 여부
    public float knockbackDuration = 0.3f;        // 넉백 지속 시간
    private float knockbackTimer = 0f;            // 넉백 시간 측정용
    private float knockbackForce = 5f;            // 넉백 힘

    // === 공격 관련 변수 ===
    private float attackCooldown = 1.5f;          // 공격 쿨타임
    private float lastAttackTime = 0f;            // 마지막 공격 시각

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // 플레이어를 자동으로 찾아 연결
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        PickNewDirection(); // 초기 이동 방향 설정
    }

    void Update()
    {
        // 죽었거나 넉백 중이면 행동 중지
        if (isDead || isKnockedBack) return;

        // 넉백 처리
        if (isKnockedBack)
        {
            knockbackTimer += Time.deltaTime;
            if (knockbackTimer >= knockbackDuration)
            {
                isKnockedBack = false;
                knockbackTimer = 0f;
            }

            animator.SetBool("Walking", false); // 넉백 중에는 걷지 않음
            return;
        }

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackRange)
            {
                // 공격 범위 내라면 공격
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
                return;
            }
            else if (distance <= detectionRange)
            {
                // 탐지 범위 내라면 추적
                ChasePlayer();
                return;
            }
        }

        // 기본 행동: 직선 이동
        WanderInStraightLine();
    }

    /// <summary>
    /// 플레이어에게 공격을 시도
    /// </summary>
    void Attack()
    {
        animator?.SetTrigger("Attack");

        if (player.TryGetComponent(out Player p))
        {
            p.TakeDamage(5f);
        }
    }

    /// <summary>
    /// 플레이어를 향해 추적 이동
    /// </summary>
    void ChasePlayer()
    {
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPos);
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        animator?.SetBool("isWalking", true);
    }

    /// <summary>
    /// 랜덤 방향으로 직선 이동 (5초마다 방향 전환)
    /// </summary>
    void WanderInStraightLine()
    {
        directionTimer += Time.deltaTime;

        if (directionTimer >= directionChangeInterval)
        {
            PickNewDirection();
            directionTimer = 0f;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        animator?.SetBool("isWalking", true);
    }

    /// <summary>
    /// 새로운 랜덤 방향 선택
    /// </summary>
    void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    /// <summary>
    /// 피해 처리 및 넉백 적용
    /// </summary>
    public void TakeDamage(float damage, Transform attacker = null)
    {
        if (isDead) return;
        if (attacker == null) return;

        health -= damage;

        // 넉백 방향 계산 및 힘 적용
        Vector3 knockbackDir = (transform.position - attacker.position).normalized;
        Debug.Log(knockbackDir);
        rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
        StartCoroutine(KnockbackRecovery());

        if (health <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 넉백 상태 회복 처리 코루틴
    /// </summary>
    IEnumerator KnockbackRecovery()
    {
        isKnockedBack = true;
        animator?.SetBool("isWalking", false);
        yield return new WaitForSeconds(0.3f);
        isKnockedBack = false;
    }

    /// <summary>
    /// 사망 처리 (애니메이션, 아이템 드랍, 파괴)
    /// </summary>
    void Die()
    {
        isDead = true;
        animator?.SetBool("isDead", true);

        // 아이템 드랍
        if (lootPrefab != null)
        {
            Instantiate(lootPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        // 몬스터 오브젝트 파괴 (2초 후)
        Destroy(gameObject, 2f);
    }
}
