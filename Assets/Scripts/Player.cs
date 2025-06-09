using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float health = 20f;
    private bool isDead = false;

    public GameObject itemPrefab; // 죽을 때 드롭할 아이템

    void Update()
    {
        if (isDead) return;

        // 이동
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.World);

        // ✅ K 키로 즉사 테스트
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("🧪 [K] 키 입력됨 - 테스트용 즉사 실행");
            TakeDamage(9999f); 
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log("플레이어 피격! 남은 체력: " + health);

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("💀 플레이어 사망");

        DropItem();
        Destroy(gameObject); // 또는 gameObject.SetActive(false);
    }

    void DropItem()
    {
        if (itemPrefab != null)
        {
            Vector3 dropPosition = transform.position + Vector3.up * 1f;
            Instantiate(itemPrefab, dropPosition, Quaternion.identity);
            Debug.Log("💎 아이템 드롭 완료");
        }
        else
        {
            Debug.LogWarning("⚠ itemPrefab이 연결되지 않았습니다.");
        }
    }
}
