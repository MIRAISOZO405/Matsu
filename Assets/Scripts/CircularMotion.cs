using UnityEngine;

public class CircularMotion : MonoBehaviour
{
    public Transform centerPoint; // 中心点
    public float rotationSpeed = 1.0f; // 回転速度
    public float radius = 1.0f; // 半径

    public float angleOffset = 0.0f; // 角度のオフセット
    private float currentAngle = 0.0f; // 現在の角度

    private void Start()
    {
        currentAngle = angleOffset; // 初期角度をオフセットで設定
    }

    private void Update()
    {
        currentAngle += rotationSpeed * Time.deltaTime; // 角度を時間によって更新
        float x = Mathf.Cos(currentAngle) * radius; // X座標
        float z = Mathf.Sin(currentAngle) * radius; // Y座標
        transform.position = new Vector3(x, 0, z) + centerPoint.position; // 位置を更新
    }
}
