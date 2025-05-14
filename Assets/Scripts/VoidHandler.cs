using System;
using UnityEngine.InputSystem;

/// <summary>
/// 매개변수가 없는 입력 콜백을 처리하는 핸들러.
/// 예: 점프, 인벤토리 토글, 상호작용 등 단순 입력.
/// </summary>
public class VoidHandler : IInputHandler
{
  private Action _callback;
  public VoidHandler(Action callback) => _callback = callback;
  public void Invoke(InputAction.CallbackContext context) => _callback?.Invoke();

}