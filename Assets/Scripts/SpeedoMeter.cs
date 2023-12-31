using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpeedoMeter : MonoBehaviour
{
    public Image needle;    // メーターの針
    public PlayerController playerController;
    private float maxSpeed;
    private float currentSpeed;
    private float needleRotationZ; // 針の現在の回転角度

    private void Start()
    {
        if (!playerController)
            GetPlayerScript();

        if (!needle) Debug.LogError("アタッチされていません");
    }

    private void Update()
    {
        if (!playerController)
            GetPlayerScript();

        // 針の目的の角度を計算
        float targetRotationZ = CalculateRotationZ(playerController.currentSpeed);

        // 針の回転を滑らかに補間
        DOTween.To(() => needleRotationZ, x => needleRotationZ = x, targetRotationZ, 0.5f)
               .OnUpdate(() => needle.transform.rotation = Quaternion.Euler(0, 0, -needleRotationZ));
    }

    private float CalculateRotationZ(float speed)
    {
        // 最大速度で180度回転するように計算
        return (speed / maxSpeed) * 180f;
    }

    private void GetPlayerScript()
    {
        playerController = FindObjectOfType<PlayerController>();

        maxSpeed = playerController.maxSpeed;
        currentSpeed = playerController.currentSpeed;
    }
}
