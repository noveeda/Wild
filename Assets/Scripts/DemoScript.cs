using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager; // 인벤토리 매니저
    public Item[] itemsToPickup; // 아이템 배열

    public void PickupItem(int id)
    {
        inventoryManager.AddItem(itemsToPickup[id]); // 인벤토리에 아이템 추가
    }
}
