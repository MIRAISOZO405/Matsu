using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    private Transform player;
    public float resetTime = 2.5f;
    public float interpolationSpeed = 1.0f; // 補間速度

    private float inputTimer = 0.0f;
    private bool viewInput; // 視点入力

    private void Awake()
    {
        // vcamが設定されていなければ、名前で検索して割り当てる
        if (!vcam)
        {
            GameObject vcamGameObject = GameObject.Find("VirtualCamera");
            if (vcamGameObject != null)
            {
                vcam = vcamGameObject.GetComponent<CinemachineVirtualCamera>();
            }
        }

        // プレイヤーのTransformを設定
        player = this.transform;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed)
        {
            viewInput = true;
        }
        // 入力がキャンセルされたか、終了したらfalseに設定
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
        // 目的の回転を計算（プレイヤーの後ろの方向）
        Quaternion targetRotation = Quaternion.Euler(0, player.eulerAngles.y, 0);

        // Cinemachine POVコンポーネントを取得
        var pov = vcam.GetCinemachineComponent<CinemachinePOV>();

        // 現在の水平軸の値を取得
        float currentHorizontalValue = pov.m_HorizontalAxis.Value;

        // 目的の水平軸の値を計算
        float targetHorizontalValue = targetRotation.eulerAngles.y;

        // 補間を使用して水平軸の値を滑らかに変更
        pov.m_HorizontalAxis.Value = Mathf.LerpAngle(currentHorizontalValue, targetHorizontalValue, interpolationSpeed * Time.deltaTime);
    }
}
