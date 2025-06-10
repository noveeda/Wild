using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 동물 오브젝트를 스폰하고, 일정 시간마다 개체 수를 유지하는 스크립트
/// </summary>
public class AnimalSpawner : MonoBehaviour
{
    public GameObject animalPrefab;     // 생성할 동물 프리팹
    public int maxAnimals = 50;         // 최대 동물 수
    public float spawnAreaSize = 1000f; // 동물이 스폰될 영역의 크기 (정사각형)

    private List<GameObject> animals = new List<GameObject>(); // 현재 존재하는 동물 리스트
    private float checkTimer = 0f;      // 10초 주기 체크용 타이머

    void Start()
    {
        SpawnInitialAnimals(); // 시작할 때 초기 동물들을 생성
    }

    void Update()
    {
        // 매 프레임마다 타이머 증가
        checkTimer += Time.deltaTime;

        // 10초마다 동물 수를 확인하고 부족한 수만큼 다시 생성
        if (checkTimer >= 10f)
        {
            checkTimer = 0f;     // 타이머 초기화
            RespawnAnimals();    // 동물 보충
        }
    }

    /// <summary>
    /// 시작 시 최초로 maxAnimals 수만큼 동물을 스폰
    /// </summary>
    void SpawnInitialAnimals()
    {
        for (int i = 0; i < maxAnimals; i++)
        {
            SpawnAnimal(); // 한 마리씩 생성
        }
    }

    /// <summary>
    /// 랜덤 위치에 동물을 생성하고 리스트에 추가
    /// </summary>
    void SpawnAnimal()
    {
        // X,Z 좌표를 랜덤하게 설정 (Y는 지면 기준 0)
        Vector3 pos = new Vector3(Random.Range(0, spawnAreaSize), 0, Random.Range(0, spawnAreaSize));

        // 동물 프리팹을 해당 위치에 생성
        GameObject animal = Instantiate(animalPrefab, pos, Quaternion.identity);

        // 리스트에 추가하여 추적
        animals.Add(animal);
    }

    /// <summary>
    /// 동물이 죽어 리스트에서 제거된 경우, 부족한 수만큼 새로 스폰
    /// </summary>
    void RespawnAnimals()
    {
        // null(파괴된 오브젝트)인 항목을 리스트에서 제거
        animals.RemoveAll(a => a == null);

        // 부족한 동물 수 계산
        int toSpawn = maxAnimals - animals.Count;

        // 부족한 수만큼 동물 재스폰
        for (int i = 0; i < toSpawn; i++)
        {
            SpawnAnimal();
        }
    }
}
