using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using NUnit.Framework.Constraints;
using UnityEditor.Rendering;
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("아이템 속성")]
    public Image image; // 아이템 이미지
    public TMP_Text countText; // 아이템 개수 텍스트

    [SerializeField]
    public Item item;
    public Transform parentAfterDrag;


    ///===========================================
    /// 프로퍼티
    ///===========================================

    // TODO 필드 프로퍼티로 변경. 추후 테스트 필요
    [Header("프로퍼티")]

    public int Count { get; set; } = 1;

    public int ItemID { get; set; }

    public string ItemName { get; set; }

    public string Description { get; set; }

    public ItemType ItemType { get; set; }

    public bool Stackable { get; set; }

    public bool Breakable { get; set; }

    public bool Consumable { get; set; }

    public int Durability { get; set; }

    public int MaxDurability { get; set; }

    public int MaxStackSize { get; set; }

    public GameObject ItemPrefab { get; set; }

    void Start()
    {
        InitializeItem(item); // 아이템 초기화
    }

    public void InitializeItem(Item newItem)
    {
        ItemID = newItem.itemID;
        ItemName = newItem.itemName;
        Description = newItem.description;
        ItemType = newItem.itemType;
        Consumable = newItem.consumable;
        Stackable = newItem.stackable;
        Breakable = newItem.breakable;
        MaxDurability = newItem.maxDurability;
        Durability = newItem.durability;
        image.sprite = newItem.icon;
        MaxStackSize = newItem.maxStackSize;
        ItemPrefab = newItem.itemPrefab;
        RefreshCount();
    }

    public void RefreshCount()
    {
        countText.text = Count.ToString(); // 아이템 개수 텍스트 업데이트
        bool textActive = Count > 1; // 개수가 1보다 크면 텍스트 활성화
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
