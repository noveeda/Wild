using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{
    private NavMeshAgent navAgent;

    public float moveRadius = 20f;       // 맵 범위 내 랜덤 이동 반경
    public float moveDelay = 2f;         // 멈추는 시간 (초)
    public float minStopTime = 1f;       // 최소 멈춤 시간
    public float maxStopTime = 3f;       // 최대 멈춤 시간

    private bool isWaiting = false;      // 현재 멈춘 상태인지
    private float waitTimer = 0f;        // 멈춤 시간 카운트

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        SetRandomDestination();
    }

    void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                SetRandomDestination();
            }
        }
        else
        {
            if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
            {
                // 목적지 도달 → 멈춤 상태로 전환
                isWaiting = true;
                waitTimer = Random.Range(minStopTime, maxStopTime);
                navAgent.isStopped = true;
            }
        }
    }

    void SetRandomDestination()
    {
        // 맵 내 랜덤 위치 지정
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;
        NavMeshHit hit;

        // 랜덤 위치가 NavMesh 위인지 확인
        if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius, NavMesh.AllAreas))
        {
            navAgent.SetDestination(hit.position);
            navAgent.isStopped = false;
        }
    }
}
