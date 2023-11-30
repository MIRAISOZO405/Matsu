using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public InputActionAsset actionAsset; // Inspector����A�T�C������

    private InputActionMap playerMap;
    private InputActionMap uiMap;

    public InputActionMap currentMap;


    void Awake()
    {
        if (actionAsset == null)
        {
            Debug.LogError("�A�^�b�`����Ă��܂���");
            return;
        }

        // ActionMap�̎擾
        playerMap = actionAsset.FindActionMap("Player");
        uiMap = actionAsset.FindActionMap("UI");

        //string scene = SceneManager.GetActiveScene().name;

        //if (scene == "TitleScene")
        //{
        //    SwitchToUI();
        //}
        //else if (scene == "GameScene")
        //{
        //    SwitchToUI();
        //}
        //else if (scene == "ResultScene")
        //{
        //    SwitchToUI();
        //}
        //else
        //{
        //    Debug.LogError("Scene�����s��");
        //}
        SwitchToUI();
    }

    public void SwitchToPlayer()
    {
        // UI��ActionMap�𖳌���
        uiMap.Disable();

        // Gameplay��ActionMap��L����
        playerMap.Enable();

        currentMap = playerMap;

    }

    public void SwitchToUI()
    {
        // Gameplay��ActionMap�𖳌���
        playerMap.Disable();

        // UI��ActionMap��L����
        uiMap.Enable();

        currentMap = uiMap;
    }

    public InputActionMap GetMap()
    {
        return currentMap;
    }

    public void SetMap(InputActionMap map)
    {
        currentMap = map;

        if (map == playerMap)
            SwitchToPlayer();
        else
            SwitchToUI();
    }
}
