using UnityEngine;

public class AnimalAI : MonoBehaviour
{
    public float health = 10f;              // 동물 체력
    public float moveSpeed = 1.5f;          // 이동 속도
    public GameObject lootPrefab;           // 고기 등 드롭 아이템

    private float directionTimer = 0f;      // 방향 전환용 타이머
    private float directionChangeInterval = 4f; // 이동 방향 변경 간격
    private Vector3 moveDirection;          // 현재 이동 방향

    void Start()
    {
        PickNewDirection(); // 시작 시 방향 설정
    }

    void Update()
    {
        // 방향 변경 타이밍 체크
        directionTimer += Time.deltaTime;

        if (directionTimer >= directionChangeInterval)
        {
            PickNewDirection();
            directionTimer = 0f;
        }

        // 계속 전진
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void PickNewDirection()
    {
        // 랜덤 방향 지정
        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // 루트 아이템 드롭
        if (lootPrefab != null)
        {
            Instantiate(lootPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        // 오브젝트 제거
        Destroy(gameObject);
    }
}
