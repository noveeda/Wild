using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Spawnable
    {
        public GameObject prefab;
        public float minHeight = 0f;
        public float maxHeight = 100f;
        public int spawnCount = 10;
    }

    public Spawnable[] spawnables;
    public Vector3 areaSize = new Vector3(50, 0, 50);
    public LayerMask groundMask;

    public float spawnDelay = 0.01f; // 프레임 나눠주는 간격

    void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        foreach (var spawnable in spawnables)
        {
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = spawnable.spawnCount * 10;

            while (spawned < spawnable.spawnCount && attempts < maxAttempts)
            {
                attempts++;

                Vector3 randomPos = transform.position + new Vector3(
                    Random.Range(-areaSize.x / 2, areaSize.x / 2),
                    100f,
                    Random.Range(-areaSize.z / 2, areaSize.z / 2)
                );

                if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 200f, groundMask))
                {
                    float y = hit.point.y;
                    if (y >= spawnable.minHeight && y <= spawnable.maxHeight)
                    {
                        Vector3 spawnPos = hit.point;
                        Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                        Instantiate(spawnable.prefab, spawnPos, rot, transform);
                        spawned++;

                        // 프레임 나누기
                        if (spawned % 50 == 0)
                            yield return null;
                    }
                }

                // 너무 많은 연산을 방지
                if (attempts % 100 == 0)
                    yield return null;
            }

            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
