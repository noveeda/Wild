using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 일정 수량만큼 몬스터를 NavMesh 위에 랜덤하게 스폰하는 스크립트
/// </summary>
public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // 생성할 몬스터 프리팹
    public int maxMonsterCount = 50; // 최대 몬스터 수
    public float mapSize = 1000f; // 맵 크기 (정사각형 맵 기준)
    public Transform player; // 플레이어 Transform (몬스터가 추적 대상으로 인식함)

    private List<GameObject> currentMonsters = new List<GameObject>(); // 현재 스폰된 몬스터 목록

    void Start()
    {
        // 게임 시작 시 스폰 루틴 시작
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// 주기적으로 몬스터를 스폰하는 코루틴
    /// </summary>
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 이미 파괴된 몬스터는 리스트에서 제거
            currentMonsters.RemoveAll(monster => monster == null);

            // 현재 스폰된 몬스터 수가 최대 수보다 적으면 스폰
            int toSpawn = maxMonsterCount - currentMonsters.Count;
            if (toSpawn > 0)
            {
                Debug.Log($"⏳ {toSpawn}마리 몬스터 스폰 준비중...");

                for (int i = 0; i < toSpawn; i++)
                {
                    // NavMesh 위의 랜덤한 위치를 가져옴
                    Vector3 spawnPos = GetRandomPositionOnNavMesh();

                    // 몬스터 생성
                    GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);

                    // 몬스터에게 플레이어 정보 전달 (추적용)
                    Monster monsterScript = monster.GetComponent<Monster>();
                    if (monsterScript != null)
                        monsterScript.player = player;

                    // 리스트에 추가
                    currentMonsters.Add(monster);

                    // 한 마리 생성 후 잠깐 대기 (연속 생성 방지)
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // 다음 스폰 검사까지 대기
            yield return new WaitForSeconds(10f);
        }
    }

    /// <summary>
    /// NavMesh 위의 랜덤 위치를 반환
    /// </summary>
    /// <returns>유효한 NavMesh 위치</returns>
    Vector3 GetRandomPositionOnNavMesh()
    {
        for (int i = 0; i < 30; i++)
        {
            // 맵 범위 내에서 랜덤한 위치 생성
            Vector3 randomPoint = new Vector3(
                Random.Range(-mapSize / 2, mapSize / 2),
                0,
                Random.Range(-mapSize / 2, mapSize / 2)
            );

            // 해당 위치가 NavMesh 위에 존재하는지 검사
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 30f, NavMesh.AllAreas))
            {
                return hit.position; // 유효한 위치 반환
            }
        }

        // 실패 시 현재 위치 반환
        return transform.position;
    }
}
