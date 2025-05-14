using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots; // 인벤토리 슬롯 배열
    public GameObject inventoryItemPrefab; // 인벤토리 아이템 프리팹
    public GameObject inventoryGroup;
    public InputReader inputReader;


    void OnEnable()
    {
        Debug.Log($"inputReader is null: {inputReader == null}");
        inputReader.RegisterHandler(InputActionName.ToggleInventory, ToggleInventory);
    }

    private void ToggleInventory()
    {
        Debug.Log("인벤토리 토글함.");
        inventoryGroup.SetActive(!inventoryGroup.activeSelf);
    }

    public bool AddItem(Item item)
    {

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot.isEmpty)
                continue;
            // 슬롯의 아이템을 가져오기
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            // 최대 스택 크기까지 채우기
            if (
                itemInSlot.getID == item.itemID &&
                itemInSlot.getCount < itemInSlot.getMaxStackSize &&
                itemInSlot.isStackable == true)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true; // true면 바닥에 떨어진 아이템 Destory(먹고 난 후)
            }
        }

        // 빈 슬롯을 찾아서 아이템 추가
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot.isEmpty)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }

    public void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGO = Instantiate(inventoryItemPrefab, slot.transform); // 슬롯에 아이템 프리팹 생성
        InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>(); // InventoryItem 컴포넌트 가져오기
        inventoryItem.InitializeItem(item); // 아이템 초기화
    }
}
