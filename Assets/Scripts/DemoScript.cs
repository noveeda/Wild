using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager; // 인벤토리 매니저
    public Item[] itemsToPickup; // 아이템 배열

    public void PickupItem(int id)
    {
        Debug.Log($"[PickupItem] 호출됨 - id: {id}");
        Debug.Log($"[PickupItem] itemsToPickup == null? {itemsToPickup == null}");
        Debug.Log($"[PickupItem] itemsToPickup.Length: {itemsToPickup?.Length ?? -1}");
        Debug.Log($"[PickupItem] itemsToPickup[{id}] == null? {itemsToPickup[id] == null}");
        inventoryManager.AddItem(itemsToPickup[id]); // 인벤토리에 아이템 추가
    }
}
