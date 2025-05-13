using UnityEngine;
using UnityEngine.InputSystem;

public class RagdollActivator : MonoBehaviour
{
	private Animator animator;
	private Rigidbody[] ragdollRigidbodies;
	private CharacterController characterController;
	private InputReader inputReader;
	private MovementController movementController;
	void Start()
	{
		animator = GetComponent<Animator>();
		ragdollRigidbodies = GameObject.Find("PlayerDemo").GetComponentsInChildren<Rigidbody>();
		inputReader = GetComponent<InputReader>();
		movementController = GetComponent<MovementController>();
		characterController = GetComponent<CharacterController>();

		// Ragdoll을 비활성화합니다.
		foreach (var rb in ragdollRigidbodies)
		{
			rb.isKinematic = true; // rb가 물리엔진에 영향을 받지 않음
			rb.interpolation = RigidbodyInterpolation.None; // 기본 설정
			rb.collisionDetectionMode = CollisionDetectionMode.Discrete; // 충돌 감지 모드 (기본값ㅣDiscrete)
		}
	}


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			ActivateRagdoll();
		}
	}
	void SyncTransforms()
	{
		// Animator가 강제로 조정한 본의 위치를 Rigidbody가 사용하는 위치로 동기화
		animator.Update(0f);
		animator.transform.hasChanged = false;
		transform.hasChanged = false;
	}

	void ActivateRagdoll()
	{
		SyncTransforms(); // 필수! 안 하면 폭발함
		animator.enabled = !animator.enabled;
		// inputReader.enabled = !inputReader.enabled; // InputReader 비활성화
		characterController.enabled = !characterController.enabled; // CharacterController 비활성화
		movementController.enabled = !movementController.enabled; // MovementController 비활성화

		foreach (var rb in ragdollRigidbodies)
		{
			rb.isKinematic = !rb.isKinematic;
			rb.collisionDetectionMode = rb.isKinematic ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous; // Kinematic 상태에 따라 충돌 감지 모드 설정
		}
	}


}
