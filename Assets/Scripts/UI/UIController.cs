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
        if (!shopManager) Debug.LogError("shopManager��������܂���");
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!isStart)
            return;

        if (!shopManager)
        {
            Debug.LogError("ShopManager��������܂���");
            return;
        }

        Vector2 direction = context.ReadValue<Vector2>();

        // �f�b�h�]�[���̐ݒ�
        float deadZone = 0.2f;
        if (direction.magnitude < deadZone)
        {
            isReadyForNewInput = true;
            return;
        }

        // �V�������͂��\�ȏ�Ԃ��ǂ������`�F�b�N
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

        // shopCanvas�݂̂��\��
        GetComponent<PlayerController>().OnReturnMap();
        shopManager.GoodsReset();
    }
}
