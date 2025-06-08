using UnityEngine;

public class Monster : MonoBehaviour
{
    public float health = 20f;
    public float detectionRange = 30f;
    public float attackRange = 5f;
    public float moveSpeed = 2f;

    public Transform player;
    public GameObject lootPrefab;

    private Animator animator;
    private float directionTimer = 0f;
    private float directionChangeInterval = 5f;
    private Vector3 moveDirection;
    private bool isDead = false;

    private float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        PickNewDirection();
    }

    void Update()
    {
        if (isDead) return;

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            Debug.Log($"📏 플레이어 거리: {distance:F2} (공격: {attackRange}, 탐지: {detectionRange})");

            if (distance <= attackRange)
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }

                animator.SetBool("isWalking", false);
                Debug.Log("💥 상태: 공격 중");
                return;
            }
            else if (distance <= detectionRange)
            {
                ChasePlayer();
                Debug.Log("🏃 상태: 추적 중");
                return;
            }
        }

        WanderInStraightLine();
        Debug.Log("🔄 상태: 방향 이동 중");
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        if (player.TryGetComponent(out Player p))
        {
            p.TakeDamage(5f);
        }
    }

    void ChasePlayer()
    {
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPos);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        animator.SetBool("isWalking", true);
    }

    void WanderInStraightLine()
    {
        directionTimer += Time.deltaTime;

        if (directionTimer >= directionChangeInterval)
        {
            PickNewDirection();
            directionTimer = 0f;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        animator.SetBool("isWalking", true);
    }

    void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection);

        Debug.Log($"🎯 [Direction] 새로운 방향 설정됨 → {moveDirection}");
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log($"💢 피격! 남은 체력: {health}");

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);

        if (lootPrefab != null)
        {
            Instantiate(lootPrefab, transform.position + Vector3.up, Quaternion.identity);
            Debug.Log("📦 아이템 드롭됨");
        }

        Destroy(gameObject, 2f);
    }
}
