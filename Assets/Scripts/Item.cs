using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("아이템 속성")]
    public int itemID; // 아이템 ID (고유 식별자)
    public string itemName; // 아이템 이름
    public string description; // 아이템 설명
    public bool consumable = true; // 소비 가능 여부
    public bool stackable = true; // 스택 가능 여부
    public bool breakable = false; // 내구도 소모 여부
    public int maxDurability = 0; // 최대 내구도 (최대값)
    public int durability = 0; // 현재 내구도 (소모 시 감소)
    public int maxStackSize = 64; // 최대 스택 크기
    public ItemType itemType; // 아이템 종류 (무기, 방어구 등)
    public Sprite icon; // 아이템 아이콘
    public GameObject itemPrefab; // 아이템 프리팹 (3D 모델 등)
    // 추가적인 속성이나 메서드 필요시 여기에 추가
}

public enum ItemType
{
    Weapon, // 무기(예: 검, 활 등)
    Armor,  // 방어구(예: 갑옷, 헬멧 등)
    Tool,   // 도구(예: 곡괭이, 도끼 등)
    Consumable, // 소비 아이템(예: 포션, 고기)
    Material,   // 재료 아이템(예: 나무, 돌)
    Miscellaneous // 기타 아이템(예: 퀘스트 아이템 등)
    // 필요에 따라 추가 가능
}