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

    private Item item;
    public Transform parentAfterDrag;

    private int itemID;

    private string itemName;

    private string description;

    private int count;

    private bool isStackable;

    private bool isBreakable;

    private bool isConsumable;

    private int durability;

    private int maxDurability;

    private int maxStackSize;

    private ItemType itemType;

    public GameObject itemPrefab;

    ///===========================================
    /// 프로퍼티
    ///===========================================

    [Header("프로퍼티")]

    public int ItemID { get { return this.itemID; } }

    public string ItemName { get { return this.itemName; } }

    public string Description { get { return this.description; } }

    public int Count
    {
        get { return this.count; }
        set { this.count = value; }
    }

    public bool IsStackable { get { return this.isStackable; } }

    public bool IsBreakable { get { return this.isBreakable; } }

    public bool IsConsumable { get { return this.isConsumable; } }

    public int Durability { get { return this.durability; } }

    public int MaxDurability { get { return this.maxDurability; } }

    public int MaxStackSize { get { return this.maxStackSize; } }

    public ItemType ItemType { get { return this.itemType; } }

    public GameObject ItemPrefab { get { return this.itemPrefab; } }

    // void Start()
    // {
    //     InitializeItem(item); // 아이템 초기화
    // }

    public void InitializeItem(Item item)
    {
        this.itemID = item.itemID;
        this.itemName = item.itemName;
        this.description = item.description;
        this.itemType = item.itemType;
        this.isConsumable = item.consumable;
        this.isStackable = item.stackable;
        this.isBreakable = item.breakable;
        this.maxDurability = item.maxDurability;
        this.durability = item.durability;
        this.maxStackSize = item.maxStackSize;
        this.itemPrefab = item.itemPrefab;
        image.sprite = item.icon;

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
