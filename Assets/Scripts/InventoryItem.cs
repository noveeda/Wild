using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("아이템 UI 속성")]
    public Image image; // 아이템 이미지
    public TMP_Text countText; // 아이템 개수 텍스트
    [HideInInspector] public Item item; // 아이템 정보
    [HideInInspector] public Transform parentAfterDrag; // 드래그 후 부모 슬롯
    [HideInInspector] public int count = 1; // 아이템 개수 (초기값 1)

    public int maxStackSize = 1; // 최대 개수 (초기값 1)
    public int getCount => count; // 현재 개수
    public int getMaxStackSize => maxStackSize; // 최대 스택 크기
    public int getID => item.itemID; // 아이템 ID
    public int getMaxDurability => item.maxDurability; // 아이템 내구도
    public int getCurrentDurability => item.durability; // 현재 내구도
    public bool isStackable => item.isStackable; // 스택 가능 여부
    public bool isBreakable => item.isBreakable; // 파괴 가능 여부
    public bool isConsumable => item.isConsumable; // 소모 가능 여부

    void Start()
    {
        InitializeItem(item); // 아이템 초기화
    }

    public void InitializeItem(Item newItem)
    {
        item = newItem; // 아이템 정보 초기화
        image.sprite = newItem.image; // 아이콘 설정
        maxStackSize = newItem.maxStackSize;
        RefreshCount();
    }

    public void RefreshCount()
    {
        countText.text = count.ToString(); // 아이템 개수 텍스트 업데이트
        bool textActive = count > 1; // 개수가 1보다 크면 텍스트 활성화
        countText.gameObject.SetActive(textActive); // 텍스트 활성화/비활성화
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false; // 드래그 시작 시 아이콘 비활성화
        parentAfterDrag = transform.parent; // 드래그 시작 시 부모 저장
        transform.SetParent(transform.root); // 드래그 중 아이템을 루트로 이동
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // 드래그 중 아이템 위치 업데이트
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true; // 드래그 시작 시 아이콘 활성화
        transform.SetParent(parentAfterDrag); // 드래그 종료 시 원래 부모로 이동
    }
}
