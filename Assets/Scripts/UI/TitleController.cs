using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.InputSystem;

public class TitleController : MonoBehaviour
{
    // title�p
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
        if (!startBtn) Debug.LogError("�A�^�b�`����Ă��܂���");
        else
        {
            UIAnimationCurve.Instance.StartAlphaShowLoop(startBtn.transform);
        }
        if (!inputManager) Debug.LogError("inputManager��������܂���");
        if (!titleCamera) Debug.LogError("titleCamera��������܂���");
        if (!virtualCamera) Debug.LogError("virtualCamera��������܂���");
        if (!canvasManager) Debug.LogError("canvasManager��������܂���");
        if (!timeManager) Debug.LogError("timeManager��������܂���");
        if (!missionManager) Debug.LogError("missionManager��������܂���");
        if (!shopCanvas) Debug.LogError("shopCanvas��������܂���");
        if (!startSE) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!postprocessManager) Debug.LogError("�A�^�b�`����Ă��܂���");

        if (!uiController)
            Debug.LogError("uiController��������܂���");
        else  
            uiController.isStart = false; // UIController �𖳌���

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

            // Vignette��intensity��0��
            postprocessManager.FadeVignette(0f);
            Invoke("GameStart", 1.5f);
        }
    }

    private void GameStart()
    {
        // UIController ��L����
        uiController.isStart = true;
        
        virtualCamera.GetComponent<CinemachineInputProvider>().enabled = true;

        inputManager.SwitchToPlayer();
        canvasManager.DisableOnlyThisCanvas(shopCanvas);    // shopCanvas�݂̂��\��
        timeManager.OnStart();
        missionManager.OnStart();

        this.enabled = false;
    }
}
