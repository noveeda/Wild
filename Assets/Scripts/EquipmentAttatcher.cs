using UnityEngine;

public class EquipmentAttatcher : MonoBehaviour
{
    public Animator playerAnimator;
    private GameObject currentEquipment;
    public Transform placeHolder;

    // TODO EquipItem 테스트 필요
    public void EquipItem(InventoryItem item)
    {
        if (currentEquipment != null)
            Destroy(currentEquipment);

        if (item == null || item.ItemPrefab == null)
            return;

        currentEquipment = Instantiate(item.ItemPrefab, placeHolder);
        currentEquipment.transform.localPosition = Vector3.zero;
        currentEquipment.transform.localRotation = Quaternion.identity;
    }

    // TODO Unequip 미완성
    public void Unequip()
    {

    }
}
