using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using UnityEngine.Rendering.Universal;

public class MovementController : MonoBehaviour
{
  [Header("Move Settings")]
  public float walkSpeed = 3f;
  public float runSpeed = 6f; // 달리기 속도
  public float speedSmoothTime = 0.1f; // 달리기 지속 시간
  private float speedVelocity; // 달리기 속도(smoothDamp 사용)
  public float moveSpeed;// 이동 속도
  [Header("Jump/Gravity Settings")]
  public float jumpHeight = 1.5f; // 점프 높이
  public float gravity = -9.81f;  // 중력 가속도
  public float gravityScale = 1f; // 중력 스케일

  private bool isSprinting; // 달리기 상태
  private float verticalVelocity; // y축 속도

  [Header("의존 컴포넌트 주입")]
  public InputReader inputReader; // InputReader 컴포넌트
  private CharacterController controller;
  private Vector2 moveInput;

  public bool GetIsSprinting => isSprinting; // 달리기 상태를 외부에서 확인할 수 있도록 프로퍼티로 제공
  public Vector2 GetMoveInput => moveInput; // 외부에서 이동 입력을 확인할 수 있도록 프로퍼티로 제공

  private List<string> registedInputActionNameList = new List<string>();
  private void Awake()
  {
    // 컴포넌트 등록
    controller = GetComponent<CharacterController>();

    // 값 초기화
    moveSpeed = walkSpeed;
    moveInput = Vector2.zero; // 초기화
    verticalVelocity = 0f; // 초기화
    isSprinting = false; // 초기화
    moveSpeed = walkSpeed; // 초기화
    speedVelocity = 0f; // 초기화
    verticalVelocity = 0f; // 초기화
  }


  private void OnEnable()
  {
    // InputReader의 MovePerformed 이벤트에 OnMove 메서드를 구독합니다.
    inputReader.RegisterHandler<Vector2>(InputActionName.Move, OnMove);
    inputReader.RegisterHandler(InputActionName.Jump, OnJump);
    inputReader.RegisterHandler<bool>(InputActionName.Sprint, OnSprint);
    inputReader.RegisterHandler<string>(InputActionName.MouseClick, OnMouseClick);

    // 등록한 action을 리스트에 추가한다.(OnDisable에서 일괄적으로 삭제하기 위함)
    registedInputActionNameList.Add(InputActionName.Move);
    registedInputActionNameList.Add(InputActionName.Jump);
    registedInputActionNameList.Add(InputActionName.Sprint);
    registedInputActionNameList.Add(InputActionName.MouseMove);
    registedInputActionNameList.Add(InputActionName.MouseClick);
  }


  private void OnDisable()
  {
    foreach (string actionName in registedInputActionNameList)
    {
      bool result = inputReader.UnregisterHandler(actionName);

      // if (result)
      //   Debug.Log($"{actionName} 구독 해제 완료");
      // else
      //   Debug.Log($"{actionName} 구독 해제 실패");
    }
  }

  // TODO 마우스 관련 로직 분리 필요
  private void OnMouseClick(string buttonName)
  {
    // if (buttonName == "leftButton")
    // {
    //   Debug.Log("좌클릭 감지");
    // }
    // else if (buttonName == "rightButton")
    // {
    //   Debug.Log("우클릭 감지");
    // }
  }

  private void OnSprint(bool isSprinting)
  {
    this.isSprinting = isSprinting;
  }

  private void OnMove(Vector2 input)
  {
    // MovePerformed 이벤트에서 전달된 Vector3 값을 moveInput에 저장합니다.

    moveInput = input;
  }

  private void OnJump()
  {
    if (controller.isGrounded)
    {
      // 원하는 높이만큼 올라갔다가 딱 멈추도록 초기 속도(v₀)를 물리 공식을 통해 계산하기 위함.
      // v² = v₀² + 2a * s (v: 최종 속도, v₀: 초기 속도, a: 가속도, s: 거리)
      // v = 0 (최종 속도), a = gravity (중력), s = jumpHeight (점프 높이)
      // v₀ = √(v² - 2a * s) = √(0 - 2 * gravity * jumpHeight)
      // gravity는 음수이므로 -2 * gravity는 양수가 됨.
      // 따라서 v₀ = √(2 * jumpHeight * -gravity) = √(jumpHeight * -2 * gravity)

      /*
        v² = v₀² + 2a * s
        0 = v₀² + 2·gravity·jumpHeight  
        → v₀² = –2·gravity·jumpHeight  
        → v₀  = √(–2·gravity·jumpHeight)
      */
      verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
  }

  private void Update()
  {

    // 캐릭터가 바닥에 닿아 있는지 확인 후 최소한의 접지력만 유지
    if (controller.isGrounded && verticalVelocity < 0)
      verticalVelocity = -1f;

    // 공중에 있는 동안 중력 적용  
    verticalVelocity += gravity * gravityScale * Time.deltaTime;

    // 이동 속도 설정
    if (isSprinting && moveInput.y > 0)
    {
      // Mathf.SmoothDamp(moveSpeed, targetSpeed, ref speedVelocity, speedSmoothTime);
      moveSpeed = Mathf.SmoothDamp(moveSpeed, runSpeed, ref speedVelocity, speedSmoothTime);
    }
    else
    {
      // Mathf.SmoothDamp(moveSpeed, targetSpeed, ref speedVelocity, speedSmoothTime);
      moveSpeed = Mathf.SmoothDamp(moveSpeed, walkSpeed, ref speedVelocity, speedSmoothTime);
    }

    // 캐릭터 이동
    PerformMovement();
  }

  private void PerformMovement()
  {
    // 이동 방향
    Vector3 horizontalDir = (transform.right * moveInput.x) + (transform.forward * moveInput.y);
    Vector3 velticalDir = new Vector3(0, verticalVelocity, 0);
    Vector3 dir = horizontalDir * moveSpeed + velticalDir;
    controller.Move(dir * Time.deltaTime);
  }

  public bool GetIsJumping()
  {
    return verticalVelocity > 0;
  }
}
