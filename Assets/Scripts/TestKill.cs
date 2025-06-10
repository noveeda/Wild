using UnityEngine;

public class TestKill : MonoBehaviour
{
    // 테스트용 키 설정
    public KeyCode killMonstersKey = KeyCode.K;     // 몬스터 
    public KeyCode killAnimalsKey = KeyCode.L;      // 동물 

    void Update()
    {
        // K 키를 누르면 씬 내 모든 Monster에게 데미지를 넣음
        if (Input.GetKeyDown(killMonstersKey))
        {
            Monster[] monsters = FindObjectsOfType<Monster>();
            foreach (Monster m in monsters)
            {
                m.TakeDamage(5f); 
            }
            Debug.Log("⚔ 모든 몬스터를 제거했습니다.");
        }

        // L 키를 누르면 씬 내 모든 AnimalAI에게 강제로 데미지를 넣음
        if (Input.GetKeyDown(killAnimalsKey))
        {
            AnimalAI[] animals = FindObjectsOfType<AnimalAI>();
            foreach (AnimalAI a in animals)
            {
                a.TakeDamage(5f);
            }
            Debug.Log("🪓 모든 동물을 제거했습니다.");
        }
    }
}
