using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Boss : MonoBehaviour
{
    public Animator animator;             // 보스 애니메이터
    public float attackInterval = 5f;     // 공격 간격 (초 단위)
    private float attackTimer;            // 다음 공격까지 남은 시간

    void Start()
    {
        attackTimer = attackInterval;     // 타이머 초기화
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;    // 타이머 감소

        if (attackTimer <= 0f)
        {
            PerformRandomAttack();        // 공격 실행
            attackTimer = attackInterval; // 타이머 재설정
        }
    }

    void PerformRandomAttack()
    {
        int pattern = Random.Range(0, 3); // 0~2 중 랜덤 선택

        switch (pattern)
        {
            case 0:
                animator.SetTrigger("OverheadSmash");    // 위에서 내려찍기
                break;
            case 1:
                animator.SetTrigger("HorizontalSwing");   // 앞으로 휘두르기
                break;
            case 2:
                animator.SetTrigger("SpinAttack");        // 360도 회전 휘두르기
                break;
        }
    }
}
