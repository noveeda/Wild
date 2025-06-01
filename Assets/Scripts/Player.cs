using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float health = 20f;
    private bool isDead = false;

    void Update()
    {
        if (isDead) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.World);
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

        // 1) 오브젝트 완전 제거
        Destroy(gameObject);

        // 또는 2) 비활성화만 하고 싶다면 아래 한 줄을 사용하세요
        // gameObject.SetActive(false);
    }
}
