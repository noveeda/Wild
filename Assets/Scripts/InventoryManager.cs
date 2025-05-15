using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [Header("컴포넌트 설정")]
    public InventorySlot[] inventorySlots; // 인벤토리 슬롯 배열
    public GameObject inventoryItemPrefab; // 빈 아이템 프리팹
    public GameObject inventoryGroup;
    public InputReader inputReader;
    public EquipmentAttatcher equipmentAttatcher;

    [Header("핫바 설정")]
    public int totalHotbarSlots = 9;
    private int currentSelectedSlot = 0;


    void OnEnable()
    {
        inputReader.RegisterHandler(InputActionName.ToggleInventory, ToggleInventory);
    }

    private void ToggleInventory()
    {
        Debug.Log("인벤토리 토글함.");
        inventoryGroup.SetActive(!inventoryGroup.activeSelf);
    }

    public bool AddItem(Item item)
    {
        // 이미 갖고 있는 아이템인지 확인
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot.isEmpty)
                continue;

            // 슬롯의 아이템을 가져오기
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            // 최대 스택 크기까지 채우기
            if (
                itemInSlot.ItemID == item.itemID &&
                itemInSlot.Count < itemInSlot.MaxStackSize &&
                itemInSlot.IsStackable == true)
            {
                itemInSlot.Count++;
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

    public void SelectHotbarSlot(int index)
    {
        index = Mathf.Clamp(index, 0, totalHotbarSlots - 1);

        currentSelectedSlot = index;

        Debug.Log($"[InventoryManager] 핫바 슬롯 선택됨: {index}");

        // 무기 장착 처리
        InventorySlot slot = inventorySlots[index];
        InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
        if (item != null)
        {
            equipmentAttatcher.EquipItem(item); // 무기 장착
        }
        else
        {
            equipmentAttatcher.Unequip(); // 빈 슬롯이면 해제
        }
    }

    public void ScrollHotbarSlot(int direction)
    {
        currentSelectedSlot = (currentSelectedSlot + direction + totalHotbarSlots) % totalHotbarSlots;
        SelectHotbarSlot(currentSelectedSlot);
    }
}
