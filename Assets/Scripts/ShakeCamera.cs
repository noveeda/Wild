using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour
{
    public float shakeDuration = 0.15f;   // 흔들리는 시간
    public float shakeMagnitude = 0.1f;   // 흔들림 세기

    private Vector3 initialPosition;      // 초기 위치 저장
    private Coroutine currentShake;

    private void Awake()
    {
        initialPosition = transform.localPosition;
    }

    public void TriggerShake()
    {
        if (currentShake != null)
            StopCoroutine(currentShake);

        currentShake = StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // 흔들림 효과 (랜덤 오프셋)
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = initialPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 원래 위치로 복귀
        transform.localPosition = initialPosition;
        currentShake = null;
    }
}
