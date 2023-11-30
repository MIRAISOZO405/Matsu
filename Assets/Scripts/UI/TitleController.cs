using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.InputSystem;

public class TitleController : MonoBehaviour
{
    // title用
    public UIController uiController;
    public Image startBtn;
    public InputManager inputManager;
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineVirtualCamera titleCamera;
    public CanvasManager canvasManager;
    public TimeManager timeManager;
    public MissionManager missionManager;
    public Canvas shopCanvas;

    public PostProcessManager postprocessManager;

    private bool isStart = false;

    public AudioClip startSE;

    private void Awake()
    {
        if (!inputManager) inputManager = FindObjectOfType<InputManager>();
        if (!titleCamera) titleCamera = GameObject.Find("TitleCamera").GetComponent<CinemachineVirtualCamera>();
        if (!virtualCamera) virtualCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        if (!canvasManager) canvasManager = FindObjectOfType<CanvasManager>();
        if (!timeManager) timeManager = FindObjectOfType<TimeManager>();
        if (!missionManager) missionManager = FindObjectOfType<MissionManager>();
        if (!uiController) uiController = FindObjectOfType<UIController>();
        if (!shopCanvas) GameObject.Find("ShopCanvas").GetComponent<Canvas>();
        if (!postprocessManager) postprocessManager = FindObjectOfType<PostProcessManager>();
    }

    private void Start()
    {
        if (!startBtn) Debug.LogError("アタッチされていません");
        else
        {
            UIAnimationCurve.Instance.StartAlphaShowLoop(startBtn.transform);
        }
        if (!inputManager) Debug.LogError("inputManagerが見つかりません");
        if (!titleCamera) Debug.LogError("titleCameraが見つかりません");
        if (!virtualCamera) Debug.LogError("virtualCameraが見つかりません");
        if (!canvasManager) Debug.LogError("canvasManagerが見つかりません");
        if (!timeManager) Debug.LogError("timeManagerが見つかりません");
        if (!missionManager) Debug.LogError("missionManagerが見つかりません");
        if (!shopCanvas) Debug.LogError("shopCanvasが見つかりません");
        if (!startSE) Debug.LogError("アタッチされていません");
        if (!postprocessManager) Debug.LogError("アタッチされていません");

        if (!uiController)
            Debug.LogError("uiControllerが見つかりません");
        else  
            uiController.isStart = false; // UIController を無効化

        virtualCamera.GetComponent<CinemachineInputProvider>().enabled = false;
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (isStart)
            return;

        if (context.started)
        {
            isStart = true;
            UIAnimationCurve.Instance.StopAlphaShowLoop();
            UIAnimationCurve.Instance.PlaySE2D(startSE);
            UIAnimationCurve.Instance.StartScaleShow(startBtn.transform);
            UIAnimationCurve.Instance.StartAlphaShow(startBtn.transform);
            FadeManager.Instance.GameStartBGM();
            titleCamera.Priority = 1;

            // Vignetteのintensityを0に
            postprocessManager.FadeVignette(0f);
            Invoke("GameStart", 1.5f);
        }
    }

    private void GameStart()
    {
        // UIController を有効化
        uiController.isStart = true;
        
        virtualCamera.GetComponent<CinemachineInputProvider>().enabled = true;

        inputManager.SwitchToPlayer();
        canvasManager.DisableOnlyThisCanvas(shopCanvas);    // shopCanvasのみを非表示
        timeManager.OnStart();
        missionManager.OnStart();

        this.enabled = false;
    }
}
