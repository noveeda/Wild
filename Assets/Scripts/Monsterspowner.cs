using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public int maxMonsterCount = 50;
    public float mapSize = 1000f;
    public Transform player;

    private List<GameObject> currentMonsters = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 사망한 몬스터 제거
            currentMonsters.RemoveAll(monster => monster == null);

            int toSpawn = maxMonsterCount - currentMonsters.Count;
            if (toSpawn > 0)
            {
                Debug.Log($"⏳ {toSpawn}마리 몬스터 스폰 준비중...");
                for (int i = 0; i < toSpawn; i++)
                {
                    Vector3 spawnPos = GetRandomPositionOnNavMesh();
                    GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                    
                    Monster monsterScript = monster.GetComponent<Monster>();
                    if (monsterScript != null)
                    {
                        monsterScript.player = player;
                    }

                    currentMonsters.Add(monster);
                    yield return new WaitForSeconds(0.1f); // 스폰 간격
                }
            }

            yield return new WaitForSeconds(10f); // 10초마다 체크
        }
    }

    Vector3 GetRandomPositionOnNavMesh()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = new Vector3(Random.Range(-mapSize / 2, mapSize / 2), 0, Random.Range(-mapSize / 2, mapSize / 2));
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 30f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return transform.position;
    }
}
