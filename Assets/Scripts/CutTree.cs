using UnityEngine;

public class ClickableTree : MonoBehaviour
{
    public GameObject branchPrefab; // 드랍할 나뭇가지 프리팹
    public int dropCount = 1;       // 드랍 개수
    public float dropRadius = 1f;   // 드랍 퍼짐 반경

    private void OnMouseDown()
    {
        DropBranches();
        Destroy(gameObject);
    }

    void DropBranches()
    {
        for (int i = 0; i < dropCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-dropRadius, dropRadius),
                0.5f,
                Random.Range(-dropRadius, dropRadius)
            );

            Vector3 spawnPos = transform.position + randomOffset;
            Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);

            Instantiate(branchPrefab, spawnPos, rot);
        }
    }
}
