using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpeedoMeter : MonoBehaviour
{
    public Image needle;    // ���[�^�[�̐j
    public PlayerController playerController;
    private float maxSpeed;
    private float currentSpeed;
    private float needleRotationZ; // �j�̌��݂̉�]�p�x

    private void Start()
    {
        if (!playerController)
            GetPlayerScript();

        if (!needle) Debug.LogError("�A�^�b�`����Ă��܂���");
    }

    private void Update()
    {
        if (!playerController)
            GetPlayerScript();

        // �j�̖ړI�̊p�x���v�Z
        float targetRotationZ = CalculateRotationZ(playerController.currentSpeed);

        // �j�̉�]�����炩�ɕ��
        DOTween.To(() => needleRotationZ, x => needleRotationZ = x, targetRotationZ, 0.5f)
               .OnUpdate(() => needle.transform.rotation = Quaternion.Euler(0, 0, -needleRotationZ));
    }

    private float CalculateRotationZ(float speed)
    {
        // �ő呬�x��180�x��]����悤�Ɍv�Z
        return (speed / maxSpeed) * 180f;
    }

    private void GetPlayerScript()
    {
        playerController = FindObjectOfType<PlayerController>();

        maxSpeed = playerController.maxSpeed;
        currentSpeed = playerController.currentSpeed;
    }
}
