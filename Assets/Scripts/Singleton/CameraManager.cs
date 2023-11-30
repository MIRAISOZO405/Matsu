using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : SingletonMonoBehaviour<CameraManager>
{

    [Header("U“®‚ÌÅ‘å’l"), SerializeField] public float maxAmplitude = 3.0f;
    [Header("U“®‚ÌŒ¸­—Ê"), SerializeField] public float downShake = 0.05f;

    private CinemachineVirtualCamera virtualCamera;
    private float minAmplitude = 0.0f;  // U“®‚ÌÅ¬’l
    private float currentAmplitude;     // Œ»İ‚ÌU“®—Ê
    private bool shake = false;         // U“®‚µ‚Ä‚¢‚é‚©‚Ç‚¤‚©

    private CinemachineBasicMultiChannelPerlin noise;

    private void Start()
    {
        currentAmplitude = maxAmplitude;
        virtualCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();

        // Virtual Camera‚©‚çCinemachineBasicMultiChannelPerlin‚ğæ“¾
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (shake)
        {
            currentAmplitude -= downShake;
            ChangeAmplitude(currentAmplitude);

            if (currentAmplitude <= minAmplitude)
            {
                shake = false;
                currentAmplitude = maxAmplitude;
            }
        }
    }

    public void ShakeCamera()
    {
        if (shake)
            return;

        shake = true;
        ChangeAmplitude(maxAmplitude);
    }

    private void ChangeAmplitude(float amplitude)
    {
        // U•‚Ì”ÍˆÍ‚ğ0.0‚©‚ç1.0‚É³‹K‰»
        float normalizedAmplitude = Mathf.Lerp(minAmplitude, maxAmplitude, amplitude);

        // Virtual Camera‚ÌNoise‚ÌU•‚ğİ’è
        noise.m_AmplitudeGain = normalizedAmplitude;
    }
}
