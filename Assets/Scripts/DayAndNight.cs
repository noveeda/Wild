using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [Header("시간 설정")]
    [SerializeField] private float secondPerRealTimeSecond = 1000f;

    [Header("안개 설정")]
    [SerializeField] private float fogDensityCalc = 10f;
    [SerializeField] private float nightFogDensity = 0.2f;
    private float dayFogDensity;
    private float currentFogDensity;

    [Header("태양광")]
    [SerializeField] private Light sunLight;
    [SerializeField] private float nightLightIntensity = 0.3f;
    private float dayLightIntensity;

    [Header("Ambient Light (색 유지용)")]
    [SerializeField] private Color dayAmbient = new Color(1f, 1f, 1f);
    [SerializeField] private Color nightAmbient = new Color(0.15f, 0.15f, 0.2f); // 어두운 파랑 계열
    private float ambientLerpT = 0f;

    [Header("Fog 색상")]
    [SerializeField] private Color dayFogColor = Color.gray;
    [SerializeField] private Color nightFogColor = new Color(0.1f, 0.1f, 0.2f); // 완전 검정 X

    private bool night = false;

    void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
        currentFogDensity = dayFogDensity;

        if (sunLight != null)
            dayLightIntensity = sunLight.intensity;

        // 환경광 타입을 Skybox 또는 Gradient로 설정했는지 확인
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
    }

    void Update()
    {
        // 태양 움직임
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        // 밤 판정 (X축 회전으로 처리)
        float sunAngle = transform.eulerAngles.x;
        night = sunAngle > 180f && sunAngle < 360f;

        // Fog 밀도 보간
        if (night)
        {
            if (currentFogDensity < nightFogDensity)
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
        }
        else
        {
            if (currentFogDensity > dayFogDensity)
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
        }
        RenderSettings.fogDensity = currentFogDensity;

        // Light Intensity 보간
        if (sunLight != null)
        {
            if (night && sunLight.intensity > nightLightIntensity)
                sunLight.intensity -= 0.1f * Time.deltaTime;
            else if (!night && sunLight.intensity < dayLightIntensity)
                sunLight.intensity += 0.1f * Time.deltaTime;
        }

        // Ambient Light 보간
        if (night)
            ambientLerpT = Mathf.Min(1f, ambientLerpT + Time.deltaTime * 0.1f);
        else
            ambientLerpT = Mathf.Max(0f, ambientLerpT - Time.deltaTime * 0.1f);

        RenderSettings.ambientLight = Color.Lerp(dayAmbient, nightAmbient, ambientLerpT);
        RenderSettings.fogColor = Color.Lerp(dayFogColor, nightFogColor, ambientLerpT);
    }
}
