using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vector2Handler : IInputHandler
{
  // 받은 콜백 변수
  private Action<Vector2> _callback;

  // 콜백 변수를 핸들러에 적용
  public Vector2Handler(Action<Vector2> callback)
  {
    _callback = callback;
  }

  public void Invoke(InputAction.CallbackContext context)
  {
    Vector2 value = context.ReadValue<Vector2>(); // 입력에서 방향 값 추출

    if (context.performed)
      _callback?.Invoke(value);
    else if (context.canceled)
      _callback?.Invoke(Vector2.zero);
  }
}