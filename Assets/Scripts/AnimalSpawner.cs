using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject animalPrefab;  // 생성할 동물 프리팹
    public int maxCount = 10;        // 최대 유지할 동물 수
    public float spawnRadius = 40f;  // 스폰 위치 반경
    public float spawnInterval = 3f; // 개체 수 확인 주기 (초)

    private List<GameObject> spawnedAnimals = new List<GameObject>(); // 현재 스폰된 동물 리스트

    void Start()
    {
        // spawnInterval 간격마다 동물 수 확인 및 부족하면 재스폰
        InvokeRepeating(nameof(CheckAndRespawn), 0f, spawnInterval);
    }

    // 현재 살아있는 동물 수 확인 후 부족하면 스폰
    void CheckAndRespawn()
    {
        // null 객체 제거 (동물이 Destroy 되었을 경우)
        spawnedAnimals.RemoveAll(animal => animal == null);

        int missing = maxCount - spawnedAnimals.Count;

        // 부족한 수 만큼 새 동물 스폰
        for (int i = 0; i < missing; i++)
        {
            SpawnAnimal();
        }
    }

    // 동물을 하나 생성하고 리스트에 등록
    void SpawnAnimal()
    {
        Vector3 spawnPos = GetRandomNavMeshPosition(transform.position, spawnRadius);
        GameObject animal = Instantiate(animalPrefab, spawnPos, Quaternion.identity);
        spawnedAnimals.Add(animal);
    }

    // NavMesh 상에서 유효한 랜덤 위치 반환
    Vector3 GetRandomNavMeshPosition(Vector3 center, float radius)
    {
        Vector3 randomDir = Random.insideUnitSphere * radius + center;
        NavMeshHit hit;

        // NavMesh에 유효한 지점이 있으면 해당 위치 반환
        if (NavMesh.SamplePosition(randomDir, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return center;
    }
}
