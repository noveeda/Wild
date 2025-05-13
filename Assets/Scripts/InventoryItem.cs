using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName; // 아이템 이름
    public string description; // 아이템 설명
    public Sprite icon; // 아이템 아이콘
    public GameObject itemPrefab; // 아이템 프리팹 (3D 모델 등)
    public int maxStackSize = 64; // 최대 스택 크기
    public bool isEquippable; // 장착 가능 여부
    public bool isConsumable; // 소비 가능 여부
    public bool isStackable; // 스택 가능 여부

    // 추가적인 속성이나 메서드 필요시 여기에 추가
}
