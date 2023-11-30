using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    
    public Material skyboxMaterial; // 上記で作成したカスタムシェーダーを使用するマテリアル

    public Texture2D morningSkybox;
    public Texture2D noonSkybox;
    public Texture2D eveningSkybox;
    public Texture2D nightSkybox;

    [Header("ブレンド速度"), Range(1, 60)]public float blendDuration = 60.0f;

    [Header("朝"), SerializeField] private int morningTime = 5;
    [Header("昼"), SerializeField] private int noonTime = 7;
    [Header("夕"), SerializeField] private int eveningTime = 17;
    [Header("夜"), SerializeField] private int nightTime = 19;

    [SerializeField] private TimeManager timeManager;
    [SerializeField] private Light sun;
    private float sunPos;

    private void Start()
    {
        if (!timeManager || !sun)Debug.LogError("アタッチされていません");
        if (skyboxMaterial == null)Debug.LogError("SkyBoxマテリアルが見つかりません");

        RenderSettings.skybox = skyboxMaterial;
        SetInitialSkybox();
    }

    private void SetInitialSkybox()
    {
        int hour = timeManager.startHour;

        if (hour >= nightTime || hour < morningTime)
        {
            SetSkyboxTextures(nightSkybox, nightSkybox, 1.0f);
        }
        else if (hour >= morningTime && hour < noonTime)
        {
            SetSkyboxTextures(morningSkybox, morningSkybox, 1.0f);
        }
        else if (hour >= noonTime && hour < eveningTime)
        {
            SetSkyboxTextures(noonSkybox, noonSkybox, 1.0f);
        }
        else if (hour >= eveningTime && hour < nightTime)
        {
            SetSkyboxTextures(eveningSkybox, eveningSkybox, 1.0f);
        }
    }

    private void Update()
    {
        UpdateSkyboxTexture();
        UpdateSunPosition();
    }

    private void UpdateSkyboxTexture()
    {
        int hour = timeManager.GetHour();
        int minute = timeManager.GetMinute();

        float blendValue = Mathf.Clamp01(minute / blendDuration);

        if (hour == morningTime)
        {
            SetSkyboxTextures(nightSkybox, morningSkybox, blendValue);
        }
        else if (hour == noonTime)
        {
            SetSkyboxTextures(morningSkybox, noonSkybox, blendValue);
        }
        else if (hour == eveningTime)
        {
            SetSkyboxTextures(noonSkybox, eveningSkybox, blendValue);
        }
        else if (hour == nightTime)
        {
            SetSkyboxTextures(eveningSkybox, nightSkybox, blendValue);
        }
       
       
    }

    private void UpdateSunPosition()
    {
        int hour = timeManager.GetHour();
        int minute = timeManager.GetMinute();

        float timeInHours = hour + minute / 60f;

        if (timeInHours >= morningTime && timeInHours <= 12) // morningTimeから12時まで
        {
            sunPos = ((timeInHours - morningTime) / (12 - morningTime)) * 90;
        }
        else if (timeInHours > 12 && timeInHours <= nightTime) // 12時からnightTimeまで
        {
            sunPos = 90 + ((timeInHours - 12) / (nightTime - 12)) * 90;
        }
        else if (timeInHours > nightTime && timeInHours <= 24) // nightTimeから24時まで
        {
            sunPos = 180 + ((timeInHours - nightTime) / (24 - nightTime)) * 90;
        }
        else // 0時からmorningTimeまで
        {
            sunPos = 270 + (timeInHours / morningTime) * 90;
        }

        // sunのX軸を回転させる
        sun.transform.eulerAngles = new Vector3(sunPos, 30, 0);
    }

    private void SetSkyboxTextures(Texture2D current, Texture2D next, float blend)
    {
        skyboxMaterial.SetTexture("_MainTex", current);
        skyboxMaterial.SetTexture("_NextTex", next);
        skyboxMaterial.SetFloat("_Blend", blend);
    }
}
