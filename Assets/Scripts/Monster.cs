using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    // 네비게이션 에이전트
    private NavMeshAgent navAgent;

    // 추적 대상 플레이어
    public Transform player;

    // 추적 조건 설정
    [SerializeField]
    private float chaseDistance = 10f;       // 최대 추적 거리
    [SerializeField]
    private float stopDistance = 2f;         // 너무 가까우면 멈춤
    [SerializeField]
    private float fieldOfView = 80f;         // 시야각 (도 단위)

    // 정찰용 패트롤 포인트
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

        // 경고용: 플레이어가 안 넣어졌을 경우
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
            else Debug.LogError("Player를 찾을 수 없습니다. 'player' 변수 할당 필요.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, toPlayer.normalized);

        DebugFOV(); // 초록 시야각 선 그리기

        // 조건: 거리 + 시야각 안에 플레이어가 있으면 추적
        if (distanceToPlayer <= chaseDistance && angleToPlayer <= fieldOfView * 0.5f)
        {
            Debug.DrawLine(transform.position, player.position, Color.red); // 빨간 추적선
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            Patrol();
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer <= stopDistance)
        {
            navAgent.isStopped = true;
        }
        else
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(player.position);
        }
    }

    private void Patrol()
    {
        // 경고 방지: 패트롤 지점이 없으면 정지
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        // Monster의 시야 방향
        Vector3 norm = transform.forward.normalized;
        // Monster -> Player의 방향의 노멀벡터
        Vector3 directionToTarget = player.transform.position - transform.position;
        directionToTarget = directionToTarget.normalized;
        
        // 두 벡터를 내적
        float dot = Vector3.Dot(norm, directionToTarget);
        // 반시야각의 cos값
        float fovOfHalf = Mathf.Cos(fieldOfView / 2);
        // 만약 Player의 방향이 시야각 내에 있으면면
        if (dot >= fovOfHalf)
        {
            if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                navAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
        }
    }

    private void DebugFOV()
    {
        Vector3 forward = transform.forward * chaseDistance;

        // 좌 시야각
        Quaternion leftRot = Quaternion.Euler(0, -fieldOfView * 0.5f, 0);
        // 우우 시야각
        Quaternion rightRot = Quaternion.Euler(0, fieldOfView * 0.5f, 0);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Debug.DrawLine(transform.position, transform.position + leftDir, Color.green);
        Debug.DrawLine(transform.position, transform.position + rightDir, Color.green);
        Vector3 origin = transform.position + Vector3.up * 1f; // 1미터 위
        Debug.DrawLine(origin, origin + leftDir, Color.green);
        Debug.DrawLine(origin, origin + rightDir, Color.green);

    }
}
