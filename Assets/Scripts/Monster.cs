using UnityEngine;

public class Monster : MonoBehaviour
{
    public float health = 20f;
    public float detectionRange = 30f;
    public float attackRange = 5f;
    public float moveSpeed = 2f;
    public float directionChangeInterval = 5f;
    public float attackCooldown = 1.5f; // 공격 간격

    public Transform player;

    private Animator animator;
    private float directionTimer = 0f;
    private Vector3 moveDirection;
    private bool isDead = false;

    private float lastAttackTime = -999f;

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

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("🔁 강제 공격 애니메이션 테스트");
            animator.SetTrigger("Attack");
        }

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            Debug.Log($"📏 플레이어 거리: {distanceToPlayer:F2} (공격: {attackRange}, 탐지: {detectionRange})");

            if (distanceToPlayer <= attackRange)
            {
                animator.SetBool("isWalking", false);
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

                // 공격 쿨타임이 지났으면 공격
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    animator.SetTrigger("Attack");
                    lastAttackTime = Time.time;
                    Debug.Log("🗡️ 공격 실행");

                    Player target = player.GetComponent<Player>();
                    if (target != null)
                    {
                        target.TakeDamage(5f);
                    }
                }

                return;
            }
            else if (distanceToPlayer <= detectionRange)
            {
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
                animator.SetBool("isWalking", true);
                Debug.Log("👣 플레이어 추적 중");
                return;
            }
        }

        WanderInStraightLine();
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

        Debug.Log($"🎯 새로운 방향 설정 → {moveDirection}");
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
        animator.SetBool("isDead", true);
        Destroy(gameObject, 2.4f);
    }
}
