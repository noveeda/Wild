using System;
using UnityEditor.Build.Content;
using UnityEngine.InputSystem;

public class BoolHandler : IInputHandler
{
  // 받은 콜백 변수를 저장할 
  private Action<bool> _callback;

  // 콜백 변수를 핸들러에 적용
  public BoolHandler(Action<bool> callback)
  {
    _callback = callback;
  }

  public void Invoke(InputAction.CallbackContext context)
  {
    if (context.performed)
      this._callback?.Invoke(true);
    else if (context.canceled)
      this._callback?.Invoke(false);
  }
}