using System;
using UnityEditor.Build.Content;
using UnityEngine.InputSystem;

public class FloatHander : IInputHandler
{
  // 받은 콜백 변수를 저장할 
  private Action<float> _callback;

  // 콜백 변수를 핸들러에 적용
  public FloatHander(Action<float> callback)
  {
    _callback = callback;
  }

  public void Invoke(InputAction.CallbackContext context)
  {
    if (context.performed)
      this._callback?.Invoke(context.ReadValue<float>());
    else if (context.canceled)
      this._callback?.Invoke(0f);
  }
}