using UnityEngine;

public class LootItem : MonoBehaviour
{
    public string itemName = "meat";
    public float floatHeight = 0.5f;
    public float rotateSpeed = 50f;

    private bool isCollected = false;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        float newY = Mathf.Sin(Time.time * 2f) * 0.1f + floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    private void OggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 '{itemName}' 아이템을 획득했습니다!");
            Destroy(gameObject);
        }
    }
}
