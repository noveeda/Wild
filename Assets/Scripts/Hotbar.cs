using UnityEngine;
using UnityEngine.InputSystem;

public class Hotbar : MonoBehaviour
{
    [Header("무기 장착 시스템 연결")]
    // public WeaponManager weaponManager;

    [Header("슬롯 개수")]
    public int totalSlots = 9;

    [SerializeField]
    private int selectedSlot = 0;

    [SerializeField]
    [Range(0.01f, 0.1f)]
    private float scrollCooldown = 0.1f;
    private float scrollTimer = 0f;

    [Header("의존 컴포넌트")]
    public InputReader inputReader;

    private void OnEnable()
    {
        inputReader.RegisterHandler<string>(InputActionName.Hotkey, OnSelectItem);
        inputReader.RegisterHandler<Vector2>(InputActionName.MouseScroll, OnSelectItem);
    }

    private void Update()
    {
        // ⏱️ 쿨타임 타이머 감소
        if (scrollTimer > 0f)
            scrollTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 숫자키 1~9 직접 슬롯 선택
    /// </summary>
    private void OnSelectItem(string buttonName)
    {

        // 숫자키 1~9 직접 슬롯 선택 (선택사항)

        selectedSlot = int.Parse(buttonName) - 1;


        Debug.Log($"selectedSlot : {selectedSlot}");
    }

    /// <summary>
    /// 마우스 스크롤로 슬롯 선택
    /// </summary>
    /// <param name="scrollValue">스크롤 값, Up은 +, Down은 -</param>
    private void OnSelectItem(Vector2 scrollDelta)
    {
        if (scrollTimer > 0f)
            return;

        if (scrollDelta.y > 0f)
        {
            selectedSlot = (selectedSlot + 1) % totalSlots;
            // weaponManager.EquipItemFromSlot(selectedSlot);
        }
        else if (scrollDelta.y < 0f)
        {
            selectedSlot = (selectedSlot - 1 + totalSlots) % totalSlots;
            // weaponManager.EquipItemFromSlot(selectedSlot);
        }

        scrollTimer = scrollCooldown; // ⏱️ 쿨타임 리셋

        Debug.Log($"selectedSlot : {selectedSlot}");
    }
}
