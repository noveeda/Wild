using System;
using UnityEngine;

/// <summary>
/// Rigidbody를 이용한 물리 기반 이동 및 충돌 시 몬스터/동물에게 데미지를 주는 플레이어 스크립트
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))] // 이 스크립트를 가진 오브젝트는 Rigidbody와 CapsuleCollider를 필수로 가져야 함
public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;    // 플레이어 이동 속도
    public float maxHP = 100f;      // 최대 체력
    private float currentHP;        // 현재 체력 (실제 게임 중 변화)
    private Rigidbody rb;           // Rigidbody 캐시용 변수

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트를 가져옴
        rb.constraints = RigidbodyConstraints.FreezeRotation; // 물리 회전 방지 (움직일 땐 회전하지 않게 고정)
        currentHP = maxHP;             // 현재 체력을 최대 체력으로 초기화
    }

    void Update()
    {
        Move(); // 키보드 입력을 받아 이동 처리

        // 스페이스바를 누르면 주변 가장 가까운 몬스터에게 데미지를 줌
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 씬에 존재하는 모든 몬스터를 검색
            Monster[] targets = FindObjectsByType<Monster>(FindObjectsSortMode.None);

            Monster nearestTarget = null;
            float nearestTargetDistance = 10000f;

            // 가장 가까운 몬스터를 탐색
            foreach (var target in targets)
            {
                Transform targetTransform = target.GetComponent<Transform>();
                float distance = Vector3.Distance(transform.position, targetTransform.position);

                if (distance < nearestTargetDistance)
                {
                    nearestTargetDistance = distance;
                    nearestTarget = target;
                }
            }

            // 가장 가까운 몬스터에게 0의 데미지를 줌 (테스트 용도)
            if (nearestTarget != null)
                nearestTarget.TakeDamage(0f, transform);
        }
    }

    /// <summary>
    /// 이동 처리 함수. WASD 또는 방향키 입력에 따라 물리 기반 이동을 수행
    /// </summary>
    void Move()
    {
        float h = Input.GetAxis("Horizontal"); // 좌우 입력 (A/D 또는 ←/→)
        float v = Input.GetAxis("Vertical");   // 상하 입력 (W/S 또는 ↑/↓)

        Vector3 dir = new Vector3(h, 0, v).normalized; // 방향 벡터 정규화

        // 현재 위치 + 방향 * 속도 * 시간 만큼 이동
        rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 다른 콜라이더와 충돌했을 때 호출되는 함수
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        // 몬스터와 충돌했을 경우
        if (collision.gameObject.CompareTag("Monster"))
        {
            Debug.Log(collision); // 충돌 정보를 디버그로 출력
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if (monster != null)
                monster.TakeDamage(5f, transform); // 5의 데미지를 줌
        }
        // 동물과 충돌했을 경우
        else if (collision.gameObject.CompareTag("Animal"))
        {
            AnimalAI animal = collision.gameObject.GetComponent<AnimalAI>();
            if (animal != null)
                animal.TakeDamage(5f, transform); // 5의 데미지를 줌
        }
    }

    /// <summary>
    /// 플레이어가 데미지를 입었을 때 호출되는 함수
    /// </summary>
    public void TakeDamage(float damage, Transform attacker = null)
    {
        currentHP -= damage; // 체력 감소
        Debug.Log($"💥 플레이어가 {damage}의 데미지를 입었습니다. 남은 체력: {currentHP}");

        // 공격자가 존재하면 넉백 효과 적용
        if (attacker != null)
        {
            Vector3 knockbackDir = (transform.position - attacker.position).normalized; // 밀려나는 방향 계산
            knockbackDir.y = 0; // 위아래 방향 제거 (수평 방향으로만 넉백)
            float knockbackForce = 5f;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse); // 넉백 적용 (순간적인 힘)
        }

        // 체력이 0 이하로 떨어지면 사망 처리
        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 플레이어 사망 처리 함수
    /// </summary>
    void Die()
    {
        Debug.Log("☠️ 플레이어 사망");
        // 사망 시 추가 처리 (예: 애니메이션 재생, 게임오버 UI 등) 필요 시 여기에 구현
    }
}
