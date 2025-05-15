using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using UnityEditor;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// InputActionAsset을 바탕으로, 액션 이름과 대응되는 핸들러를 자동으로 바인딩하여
/// 처리하는 클래스.
/// 매번 새로운 InputAction이 생겨도 이 클래스 내부 코드를 수정할 필요 없이 외부에서
/// 등록만 하면 됨.
/// </summary>
public class InputReader : MonoBehaviour
{
  [Header("Input Actions Asset")]
  // 에디터에서 연결한 InputActions
  [SerializeField]
  private InputActionAsset inputActions;

  // 문자열로 된 키값에 대응하는 입력 핸들러 저장소
  private Dictionary<string, IInputHandler> handlerMap = new();

  // ===============================
  // 외부에서 사용할 등록 함수들
  // ===============================

  /// <summary>
  /// Action타입이 void가 아닌 다른 타입인 발행자
  /// </summary>
  /// <typeparam name="T">callback의 타입</typeparam>
  /// <param name="actionName">Input System에 정의된 Action 이름(InputActionNames.cs에 정의됨)</param>
  /// <param name="callback">등록할 콜백함수</param>
  public void RegisterHandler<T>(string actionName, Action<T> callback)
  {
    if (typeof(T) == typeof(Vector2))
    {
      handlerMap[actionName] = new Vector2Handler(callback as Action<Vector2>);
    }
    else if (typeof(T) == typeof(bool))
    {
      handlerMap[actionName] = new BoolHandler(callback as Action<bool>);
    }
    else if (typeof(T) == typeof(string))
    {
      handlerMap[actionName] = new ControlNameHandler(callback as Action<string>);
    }
    else if (typeof(T) == typeof(float))
    {
      handlerMap[actionName] = new FloatHander(callback as Action<float>);
    }

    ApplyActions();
  }


  /// <summary>
  /// Action 타입이 void인 발행자
  /// </summary>
  /// <param name="actionName">>Input System에 정의된 Action 이름(InputActionNames.cs에 정의됨)</param>
  /// <param name="callback">등록할 콜백함수</param>
  public void RegisterHandler(string actionName, Action callback)
  {
    handlerMap[actionName] = new VoidHandler(callback);
    ApplyActions();
  }

  /// <summary>
  /// 구독자를 구독 해제시키는 메소드
  /// </summary>
  /// <param name="actionName">구독 해제할 Action 이름</param>
  /// <returns>해제 성공은 true, 아니면 false</returns>
  public bool UnregisterHandler(string actionName)
    => handlerMap.Remove(actionName);

  //@FIXME: 나중에 등록된 Handler도 반영되게끔 고쳐야함. ToggleInventory가 등록이 안되는중.
  void OnEnable()
  {

  }

  void OnDisable()
  {
    foreach (var map in inputActions.actionMaps)
    {
      foreach (var action in map.actions)
      {
        action.Disable();
      }
    }
    inputActions.Disable();
  }


  private void ApplyActions()
  {
    // Input System의 Action map을 순회
    foreach (var map in inputActions.actionMaps)
    {
      // Action Map을 순회하며 등록된 action을 가져옴
      foreach (var action in map.actions)
      {
        if (!handlerMap.TryGetValue(action.name, out var handler))
        {
          continue;
        }

        // 각 action가 performed될 때 실행할 컨텍스트를 추가
        action.performed += ctx => TryInvoke(action.name, ctx);

        if (handler is Vector2Handler || handler is BoolHandler)
          action.canceled += ctx => TryInvoke(action.name, ctx);

        action.Enable();
      }
    }
  }

  /// <summary>
  /// handlerMap에서 actionName에 해당하는 핸들러를 찾아서 Invoke 한다
  /// </summary>
  /// <param name="actionName">등록할 핸들러 이름</param>
  /// <param name="context">콜백 컨텍스트</param>
  private void TryInvoke(string actionName, InputAction.CallbackContext context)
  {
    if (handlerMap.TryGetValue(actionName, out var handler))
    {
      handler.Invoke(context);
    }
    else
    {
      Debug.LogWarning($"[InputReader(TryInvoke)] 등록되지 않은 액션 : {actionName}");
    }
  }
}
