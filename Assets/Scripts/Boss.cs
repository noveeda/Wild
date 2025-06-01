using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Boss : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    public float detectionRange = 20f;
    public float attackRange = 3f;
    public float attackCooldown = 2f;

    public int maxHealth = 200;
    private int currentHealth;

    private bool isAttacking = false;
    private float lastAttackTime = -999f;

    private int damageDownStrike = 5;
    private int damageFrontSwing = 7;
    private int damage360Swing = 10;

    public float roamInterval = 5f;  // 자유 이동 간격
    private float roamTimer;

    public Vector2 roamAreaSize = new Vector2(1000f, 1000f);  // 전체 맵 자유이동 영역

    void Start()
    {
        currentHealth = maxHealth;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        roamTimer = roamInterval;
    }

    void Update()
    {
        if (player == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                if (Time.time - lastAttackTime > attackCooldown && !isAttacking)
                {
                    StartCoroutine(PerformAttack());
                    lastAttackTime = Time.time;
                }

                agent.isStopped = true;
                animator.SetBool("isWalking", false);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            // 자유 이동 로직
            roamTimer -= Time.deltaTime;
            if (roamTimer <= 0f || agent.remainingDistance < 1f)
            {
                Vector3 randomPos = GetRandomRoamPosition();
                agent.SetDestination(randomPos);
                agent.isStopped = false;
                animator.SetBool("isWalking", true);
                roamTimer = roamInterval;
            }
        }
    }

    Vector3 GetRandomRoamPosition()
    {
        float x = Random.Range(0f, roamAreaSize.x);
        float z = Random.Range(0f, roamAreaSize.y);
        Vector3 worldPos = new Vector3(x, 0, z);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(worldPos, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position; // 실패 시 현재 위치 유지
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
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
        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }

    void DealDamage(int damage)
    {
        Debug.Log($"플레이어에게 {damage} 데미지 입힘");
        // if (player != null) player.GetComponent<PlayerHealth>().TakeDamage(damage);
    }

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
        Debug.Log("보스 사망");
        Destroy(gameObject);
    }
}
