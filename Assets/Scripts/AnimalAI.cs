using UnityEngine;

/// <summary>
/// 애니메이션 포함 자유 이동 + 넉백 + 아이템 드롭을 처리하는 동물 AI
/// </summary>
public class AnimalAI : MonoBehaviour
{
    [Header("Stats")]
    public float health = 10f;              // 동물의 체력
    public float moveSpeed = 1.5f;          // 이동 속도

    [Header("Drop")]
    public GameObject lootPrefab;           // 사망 시 드롭할 아이템 프리팹

    [Header("Knockback")]
    public float knockbackForce = 4f;       // 넉백 힘
    public float knockbackDuration = 0.3f;  // 넉백 지속 시간

    private Rigidbody rb;                   // Rigidbody 컴포넌트 참조
    private Animator animator;              // Animator 컴포넌트 참조

    private bool isDead = false;            // 죽었는지 여부
    private bool isKnockedBack = false;     // 넉백 중인지 여부
    private float knockbackTimer = 0f;      // 넉백 시간 측정용 타이머

    private Vector3 moveDirection;          // 이동 방향
    private float directionTimer = 0f;      // 방향 유지 시간 측정용 타이머
    private float directionChangeInterval = 5f; // 방향 변경 주기 (초)

    void Start()
    {
        rb = GetComponent<Rigidbody>();     // Rigidbody 컴포넌트 가져오기
        animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기

        // X, Z 축의 회전을 고정하여 쓰러지지 않도록 설정
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // 초기 이동 방향 설정
        PickNewDirection();
    }

    void Update()
    {
        if (isDead) return; // 죽었으면 동작 중지

        if (isKnockedBack)
        {
            // 넉백 시간 계산
            knockbackTimer += Time.deltaTime;

            if (knockbackTimer >= knockbackDuration)
            {
                // 넉백 종료
                isKnockedBack = false;
                knockbackTimer = 0f;
            }

            // 넉백 중에는 걷기 애니메이션 비활성화
            animator.SetBool("Walking", false);
            return;
        }

        // 일반 자유 이동 실행
        WanderInStraightLine();
    }

    /// <summary>
    /// 일정 시간 동안 한 방향으로 직선 이동
    /// </summary>
    void WanderInStraightLine()
    {
        directionTimer += Time.deltaTime;

        // 일정 시간이 지나면 새 방향으로 변경
        if (directionTimer >= directionChangeInterval)
        {
            PickNewDirection();
            directionTimer = 0f;
        }

        // 이동 실행
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 걷기 애니메이션 활성화
        animator.SetBool("Walking", true);
    }

    /// <summary>
    /// 새로운 이동 방향을 무작위로 선택
    /// </summary>
    void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f); // 0~360도 각도 랜덤 생성
        moveDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized; // 해당 방향으로 단위 벡터 설정
        transform.rotation = Quaternion.LookRotation(moveDirection); // 해당 방향을 바라보도록 회전
    }

    /// <summary>
    /// 데미지를 받으면 체력 감소 및 넉백 처리
    /// </summary>
    public void TakeDamage(float damage, Transform attacker = null)
    {
        if (isDead) return; // 이미 죽었으면 무시

        health -= damage; // 체력 감소
        Debug.Log($"🦌 동물 피해: {damage} → 체력: {health}");

        if (attacker != null && rb != null)
        {
            // 공격자 기준으로 반대 방향으로 넉백 적용
            Vector3 knockbackDir = (transform.position - attacker.position).normalized;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);

            isKnockedBack = true; // 넉백 상태 시작
            knockbackTimer = 0f;
        }

        if (health <= 0f)
        {
            // 체력이 0 이하일 경우 사망 처리
            Die();
        }
    }

    /// <summary>
    /// 동물 사망 처리 (애니메이션, 아이템 드롭, 오브젝트 제거)
    /// </summary>
    void Die()
    {
        isDead = true; // 사망 상태 설정

        if (animator != null)
        {
            // 죽는 애니메이션 설정
            animator.SetBool("Dying", true);
            animator.SetBool("Walking", false);
        }

        // 아이템 드롭
        if (lootPrefab != null)
        {
            Instantiate(lootPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        // 2초 뒤 오브젝트 제거
        Destroy(gameObject, 2f);
    }
}
