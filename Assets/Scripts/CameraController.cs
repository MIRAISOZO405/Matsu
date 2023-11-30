using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    private Transform player;
    public float resetTime = 2.5f;
    public float interpolationSpeed = 1.0f; // ��ԑ��x

    private float inputTimer = 0.0f;
    private bool viewInput; // ���_����

    private void Awake()
    {
        // vcam���ݒ肳��Ă��Ȃ���΁A���O�Ō������Ċ��蓖�Ă�
        if (!vcam)
        {
            GameObject vcamGameObject = GameObject.Find("VirtualCamera");
            if (vcamGameObject != null)
            {
                vcam = vcamGameObject.GetComponent<CinemachineVirtualCamera>();
            }
        }

        // �v���C���[��Transform��ݒ�
        player = this.transform;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed)
        {
            viewInput = true;
        }
        // ���͂��L�����Z�����ꂽ���A�I��������false�ɐݒ�
        else if (context.phase == InputActionPhase.Canceled)
        {
            viewInput = false;
        }
    }

    private void Update()
    {
        if (!viewInput)
        {
            inputTimer += Time.deltaTime;
            if (inputTimer > resetTime)
            {
                SmoothlyResetCameraPosition();
            }
        }
        else
        {
            inputTimer = 0.0f;
        }
    }

    private void SmoothlyResetCameraPosition()
    {
        // �ړI�̉�]���v�Z�i�v���C���[�̌��̕����j
        Quaternion targetRotation = Quaternion.Euler(0, player.eulerAngles.y, 0);

        // Cinemachine POV�R���|�[�l���g���擾
        var pov = vcam.GetCinemachineComponent<CinemachinePOV>();

        // ���݂̐������̒l���擾
        float currentHorizontalValue = pov.m_HorizontalAxis.Value;

        // �ړI�̐������̒l���v�Z
        float targetHorizontalValue = targetRotation.eulerAngles.y;

        // ��Ԃ��g�p���Đ������̒l�����炩�ɕύX
        pov.m_HorizontalAxis.Value = Mathf.LerpAngle(currentHorizontalValue, targetHorizontalValue, interpolationSpeed * Time.deltaTime);
    }
}
