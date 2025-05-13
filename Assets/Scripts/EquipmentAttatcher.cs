using UnityEngine;

public class EquipmentAttatcher : MonoBehaviour
{
    public Animator playerAnimator;
    private GameObject currentTool;

    public void SwitchTool(GameObject newToolPrefab)
    {
        // 기존 도구 제거
        if (currentTool != null)
        {
            Destroy(currentTool);
        }

        // 손 본 찾기
        Transform rightHand = playerAnimator.GetBoneTransform(HumanBodyBones.RightHand);

        if (rightHand != null && newToolPrefab != null)
        {
            currentTool = Instantiate(newToolPrefab, rightHand);
            currentTool.transform.localPosition = Vector3.zero; // 필요시 조정
            currentTool.transform.localRotation = Quaternion.identity; // 필요시 조정
        }
    }
}
