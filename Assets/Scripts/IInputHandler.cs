using UnityEngine.InputSystem;
/// <summary>
/// 다양한 타입의 입력 콜백(Action, Action<Vector2> 등)을 공통으로 처리하기 위한 인터페이스
/// 모든 핸들러는 이 인터페이스를 구현하여, Invoke를 통해 입력을 처리한다.
/// </summary>
public interface IInputHandler
{
  void Invoke(InputAction.CallbackContext context);
}
