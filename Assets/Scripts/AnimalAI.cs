using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimalAI : MonoBehaviour
{
    public float moveSpeed = 2f; // 이동 속도
    private Vector3 targetPosition;

    private NavMeshAgent agent; // 네비게이션 에이전트
    private float moveTime = 3f; // 이동 시간
    private float waitTime = 2f; // 멈춤 시간
    private float timer;         // 현재 타이머
    private bool isMoving = true; // 현재 이동 중인지 여부

    // private DropOnDeath dropOnDeath; // 드롭 스크립트 참조

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // dropOnDeath = GetComponent<DropOnDeath>(); // DropOnDeath 스크립트 가져오기

        timer = moveTime;
        SetRandomTarget(); // 첫 목표 지점 설정
    }

    void Update()
    {
        if (isMoving)
        {
            agent.SetDestination(targetPosition); // 목표 지점으로 이동

            timer -= Time.deltaTime;
            if (timer <= 0f || Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                isMoving = false;
                timer = waitTime;
                agent.ResetPath(); // 이동 중지
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isMoving = true;
                timer = moveTime;
                SetRandomTarget(); // 새로운 목표 지점 설정
            }
        }
    }

    // 랜덤 위치 설정 함수
    void SetRandomTarget()
    {
        float range = 20f; // 맵 이동 범위
        Vector3 randomPos = new Vector3(Random.Range(-range, range), transform.position.y, Random.Range(-range, range));
        targetPosition = randomPos;
    }

    // 외부에서 호출되는 함수, 동물 제거 및 아이템 드롭 수행
    public void Die()
    {
        // dropOnDeath?.DropItem(); // 아이템 드롭 시도
        Destroy(gameObject);     // 동물 오브젝트 제거
    }
}
