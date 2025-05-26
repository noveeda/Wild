using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // 생성할 몬스터 프리팹
    public int maxCount = 5;         // 최대 유지할 몬스터 수
    public float spawnRadius = 30f;  // 스폰 위치의 반경 범위
    public float spawnInterval = 3f; // 개체 수를 체크하는 시간 간격(초)

    private List<GameObject> spawnedMonsters = new List<GameObject>(); // 현재 스폰된 몬스터 리스트

    void Start()
    {
        // 일정 시간마다 CheckAndRespawn 함수를 호출 (반복)
        InvokeRepeating(nameof(CheckAndRespawn), 0f, spawnInterval);
    }

    // 현재 몬스터 수가 부족하면 추가로 스폰
    void CheckAndRespawn()
    {
        // 이미 제거된(죽은) 몬스터를 리스트에서 제거 (null인 객체 제거)
        spawnedMonsters.RemoveAll(monster => monster == null);

        // 부족한 몬스터 수 계산
        int missing = maxCount - spawnedMonsters.Count;

        // 부족한 만큼 스폰
        for (int i = 0; i < missing; i++)
        {
            SpawnMonster();
        }
    }

    // 몬스터를 하나 생성하여 리스트에 추가
    void SpawnMonster()
    {
        Vector3 spawnPos = GetRandomNavMeshPosition(transform.position, spawnRadius);
        GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
        spawnedMonsters.Add(monster);
    }

    // NavMesh 안에서 랜덤 위치를 반환
    Vector3 GetRandomNavMeshPosition(Vector3 center, float radius)
    {
        Vector3 randomDir = Random.insideUnitSphere * radius + center; // 중심 기준 랜덤 방향
        NavMeshHit hit;

        // NavMesh 상에 있는 위치가 있으면 반환, 없으면 center 반환
        if (NavMesh.SamplePosition(randomDir, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return center;
    }
}
