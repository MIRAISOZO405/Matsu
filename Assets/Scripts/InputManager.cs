using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public InputActionAsset actionAsset; // Inspectorからアサインする

    private InputActionMap playerMap;
    private InputActionMap uiMap;

    public InputActionMap currentMap;


    void Awake()
    {
        if (actionAsset == null)
        {
            Debug.LogError("アタッチされていません");
            return;
        }

        // ActionMapの取得
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
        //    Debug.LogError("Scene名が不明");
        //}
        SwitchToUI();
    }

    public void SwitchToPlayer()
    {
        // UIのActionMapを無効化
        uiMap.Disable();

        // GameplayのActionMapを有効化
        playerMap.Enable();

        currentMap = playerMap;

    }

    public void SwitchToUI()
    {
        // GameplayのActionMapを無効化
        playerMap.Disable();

        // UIのActionMapを有効化
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
