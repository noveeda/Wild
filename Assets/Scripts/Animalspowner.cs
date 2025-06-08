using UnityEngine;
using System.Collections.Generic;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject animalPrefab;     // 동물 프리팹
    public int maxAnimals = 50;         // 동물 최대 수
    public float spawnAreaSize = 1000f; // 스폰 맵 범위

    private List<GameObject> animals = new List<GameObject>();
    private float checkTimer = 0f;

    void Start()
    {
        SpawnInitialAnimals(); // 시작 시 동물 스폰
    }

    void Update()
    {
        // 10초마다 개수 확인
        checkTimer += Time.deltaTime;
        if (checkTimer >= 10f)
        {
            checkTimer = 0f;
            RespawnAnimals(); // 부족한 수만큼 스폰
        }
    }

    void SpawnInitialAnimals()
    {
        for (int i = 0; i < maxAnimals; i++)
        {
            SpawnAnimal();
        }
    }

    void SpawnAnimal()
    {
        // 랜덤 위치에 동물 생성
        Vector3 pos = new Vector3(Random.Range(0, spawnAreaSize), 0, Random.Range(0, spawnAreaSize));
        GameObject animal = Instantiate(animalPrefab, pos, Quaternion.identity);
        animals.Add(animal);
    }

    void RespawnAnimals()
    {
        // 제거된 객체 정리
        animals.RemoveAll(a => a == null);

        int toSpawn = maxAnimals - animals.Count;
        for (int i = 0; i < toSpawn; i++)
        {
            SpawnAnimal();
        }
    }
}
