using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigIKWeightController : MonoBehaviour
{
    public Rig rig; // 공격용 IK 리깅
    public Animator animator;
    public string swingStateTag = "Swing"; // 또는 Swing 상태 이름

    [Range(0f, 10f)] public float weightSmooth = 5f;

    void Update()
    {
        bool isInSwing = animator.GetCurrentAnimatorStateInfo(0).IsName("Swing");

        float targetWeight = isInSwing ? 1f : 0f;

        rig.weight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime * weightSmooth);
    }
}
