using UnityEngine;

public class Inventory : MonoBehaviour
{
    public EquipmentAttatcher equipmentAttatcher;
    public Item[] items = new Item[27]; // 인벤토리 슬롯 수 (3cols, 9rows 그리드)
}
