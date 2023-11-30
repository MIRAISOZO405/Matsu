using UnityEngine;

public class CircularMotion : MonoBehaviour
{
    public Transform centerPoint; // ���S�_
    public float rotationSpeed = 1.0f; // ��]���x
    public float radius = 1.0f; // ���a

    public float angleOffset = 0.0f; // �p�x�̃I�t�Z�b�g
    private float currentAngle = 0.0f; // ���݂̊p�x

    private void Start()
    {
        currentAngle = angleOffset; // �����p�x���I�t�Z�b�g�Őݒ�
    }

    private void Update()
    {
        currentAngle += rotationSpeed * Time.deltaTime; // �p�x�����Ԃɂ���čX�V
        float x = Mathf.Cos(currentAngle) * radius; // X���W
        float z = Mathf.Sin(currentAngle) * radius; // Y���W
        transform.position = new Vector3(x, 0, z) + centerPoint.position; // �ʒu���X�V
    }
}
