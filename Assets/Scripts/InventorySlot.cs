using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public bool isEmpty => transform.childCount == 0; // 슬롯이 비어있는지 확인하는 속성


    public void OnDrop(PointerEventData eventData)
    {
        // 드래그된 아이템을 슬롯에 추가하는 로직
        if (transform.childCount == 0) // 슬롯이 비어있을 때만 아이템 추가
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform; // 드래그된 아이템의 부모 슬롯 설정
        }
        else if (transform.childCount == 1) // 슬롯에 아이템이 있을 때는 스왑
        {
            InventoryItem itemA = transform.GetChild(0).GetComponent<InventoryItem>(); // 이동할 위치에 있는 아이템
            InventoryItem itemB = eventData.pointerDrag.GetComponent<InventoryItem>(); // 이동시킬 아이템
            itemA.parentAfterDrag = itemB.parentAfterDrag; // 아이템 A의 부모 슬롯 설정
            itemA.OnEndDrag(eventData); // 위치 설정
            itemB.parentAfterDrag = transform; // 아이템 B의 부모 슬롯 설정
        }
    }
}
