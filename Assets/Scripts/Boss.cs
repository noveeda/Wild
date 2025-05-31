using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Boss : MonoBehaviour
{
    public Transform player;                 // 플레이어 위치 참조
    public NavMeshAgent agent;               // 네비메시 에이전트
    public Animator animator;                // 애니메이터

    public float detectionRange = 20f;      // 플레이어 감지 거리
    public float attackRange = 3f;          // 공격 거리
    public float attackCooldown = 2f;       // 공격 쿨타임

    public int maxHealth = 200;              // 보스 최대 체력
    private int currentHealth;

    private bool isAttacking = false;
    private float lastAttackTime = -999f;

    // 공격 패턴 데미지
    private int damageDownStrike = 5;
    private int damageFrontSwing = 7;
    private int damage360Swing = 10;

    private Vector3 moveAreaCenter = new Vector3(500, 0, 500);  // 이동 제한 영역 중심 (맵 중앙 예시)
    private float moveAreaRadius = 50f;                         // 이동 제한 반경 (100x100 범위 -> 반경 50)

    void Start()
    {
        currentHealth = maxHealth;

        // 에이전트, 애니메이터 컴포넌트 자동 할당(없으면 에러)
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 플레이어가 없으면 작동 안함
        if (player == null) return;

        // NavMeshAgent가 NavMesh 위에 있는지 확인
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent가 NavMesh 위에 없습니다!");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // 공격 거리 이내
            if (distanceToPlayer <= attackRange)
            {
                // 공격 쿨타임 및 공격 중 아님 확인 후 공격 실행
                if (Time.time - lastAttackTime > attackCooldown && !isAttacking)
                {
                    StartCoroutine(PerformAttack());
                    lastAttackTime = Time.time;
                }

                agent.isStopped = true;            // 이동 멈춤
                animator.SetBool("isWalking", false);
            }
            else
            {
                // 플레이어 따라가기
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            // 플레이어 감지 거리 벗어나면 멈춤
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
        }

        // 이동 제한 처리 (맵 내 특정 영역 벗어나지 않도록)
        Vector3 offsetFromCenter = transform.position - moveAreaCenter;
        if (offsetFromCenter.magnitude > moveAreaRadius)
        {
            Vector3 clampedPos = moveAreaCenter + offsetFromCenter.normalized * moveAreaRadius;
            agent.Warp(clampedPos); // 위치 강제 보정
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        // 공격 애니메이션 랜덤 선택
        int attackType = Random.Range(0, 3);
        switch (attackType)
        {
            case 0:
                animator.SetTrigger("AttackDownStrike");
                DealDamage(damageDownStrike);
                break;
            case 1:
                animator.SetTrigger("AttackFrontSwing");
                DealDamage(damageFrontSwing);
                break;
            case 2:
                animator.SetTrigger("Attack360Swing");
                DealDamage(damage360Swing);
                break;
        }

        // 공격 애니메이션 재생시간 기다림 (예: 2초)
        yield return new WaitForSeconds(2f);

        isAttacking = false;
    }

    void DealDamage(int damage)
    {
        // 여기서는 플레이어에게 데미지를 주는 코드가 들어가야 함
        Debug.Log($"플레이어에게 {damage} 데미지 입힘");
        // 예) player.GetComponent<PlayerHealth>().TakeDamage(damage);
    }

    // 보스 체력 관리 함수
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 보스 사망 처리
        Debug.Log("보스 사망");
        Destroy(gameObject);
    }
}
