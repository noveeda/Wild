using UnityEngine;
using UnityEngine.InputSystem; // ✅ Input System 사용

public class MouseLook : MonoBehaviour
{
    [Header("마우스 감도 조절")]
    public float mouseSensitivity = 10f;

    [Space(10f)]
    [Header("참조 대상")]
    public Transform playerBody;     // ⬅️ Y축 회전 (좌우)
    public Transform cameraHolder;   // ⬅️ X축 회전 (상하)
    private float xRotation = 0f;    // ⬅️ 상하 각도 누적 저장용

    [Space(10f)]
    [Header("시야 제약 조건")]
    public float xRotationLimit = 70f;

    [Space(10f)]
    [Header("의존 컴포넌트")]
    public InputReader inputReader;     // ⬅️ Input System 연동
    public ShakeCamera shakeCamera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputReader.RegisterHandler<Vector2>(InputActionName.MouseMove, OnMouseMove);
    }

    private void OnDisable()
    {
        inputReader.UnregisterHandler(InputActionName.MouseMove);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            return; // 커서가 고정 안 되어 있으면 시야 회전 막음

        if (Input.GetKeyDown(KeyCode.BackQuote))
            shakeCamera.TriggerShake();
    }

    private void OnMouseMove(Vector2 mouseDelta)
    {
        // 감도와 Time.deltaTime 반영한 마우스 이동량
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // X축 회전 누적 (상하 시야 제한)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -xRotationLimit, xRotationLimit); // 시야 제한

        // 실제 회전 적용
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // ⬅️ 상하
        playerBody.Rotate(Vector3.up * mouseX); // ⬅️ 좌우
    }
}
