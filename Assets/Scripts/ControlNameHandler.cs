using System;
using UnityEngine.InputSystem;

/// <summary>
/// 한 액션에 여러 바인딩의 이름을 전달하는 핸들러
/// 예: 마우스 버튼
/// </summary>
public class ControlNameHandler : IInputHandler
{
  private Action<string> _callback;
  public ControlNameHandler(Action<string> callback) => _callback = callback;
  public void Invoke(InputAction.CallbackContext context) => _callback?.Invoke(context.control.name);

}