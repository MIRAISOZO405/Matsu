using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : SingletonMonoBehaviour<ControllerManager>
{
    private Gamepad gamepad;    // �Q�[���p�b�h
    public float shakeTime = 0.5f;

    public void ShakeController()
    {
        gamepad = Gamepad.current;

        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0.5f, 0.5f);

            // 0.5�b��ɐU�����X�g�b�v
            Invoke("ShakeStop", shakeTime);

        }
    }

    private void ShakeStop()
    {
        gamepad = Gamepad.current;

        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0.0f, 0.0f);
        }
    }

    // �Q�[���I�����ɐU�����I��
    private void OnApplicationQuit()
    {
        ShakeStop();
    }
}
