using UnityEngine;

public class Inventory : MonoBehaviour
{
    public EquipmentAttatcher equipmentAttatcher;
    public Item[] items = new Item[27]; // 인벤토리 슬롯 수 (3cols, 9rows 그리드)

    // 인벤토리에서 특정 슬롯(index)의 아이템 선택
    public void SelectItem(int index)
    {
        if (index >= 0 && index < items.Length)
        {
            equipmentAttatcher.SwitchTool(items[index].itemPrefab);
        }
    }
}
