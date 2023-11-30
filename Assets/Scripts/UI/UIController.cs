using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
    public bool isStart = false;
    public ShopManager shopManager;
    private bool isReadyForNewInput = true;

    private void Awake()
    {
        if (!shopManager) shopManager = FindObjectOfType<ShopManager>();
    }

    private void Start()
    {
        if (!shopManager) Debug.LogError("shopManagerが見つかりません");
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!isStart)
            return;

        if (!shopManager)
        {
            Debug.LogError("ShopManagerが見つかりません");
            return;
        }

        Vector2 direction = context.ReadValue<Vector2>();

        // デッドゾーンの設定
        float deadZone = 0.2f;
        if (direction.magnitude < deadZone)
        {
            isReadyForNewInput = true;
            return;
        }

        // 新しい入力が可能な状態かどうかをチェック
        if (isReadyForNewInput)
        {
            shopManager.GoodsChange(direction);
            isReadyForNewInput = false;
        }
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (!isStart)
            return;

        if (context.started)
        {
            shopManager.GoodsConfirm();
        }
    }

    public void OnQuit()
    {
        if (!isStart)
            return;

        // shopCanvasのみを非表示
        GetComponent<PlayerController>().OnReturnMap();
        shopManager.GoodsReset();
    }
}
