using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using PlayerEnums;

public class PlayerController : MonoBehaviour
{
    // debug
    public Image image;
    public GameObject boostEffect;
    public GameObject successEffect;
    public GameObject amazingEffect;

    [SerializeField] private PlayerLevel playerLevel;
    [Header("現在速度"), SerializeField] public float currentSpeed = 0f;
    [Header("最大速度"), SerializeField] public float maxSpeed = 15.0f;
    [Header("初速度"), SerializeField] private float startSpeed = 1.0f;
    private bool isDash = false;

    // ブレーキ関連
    private ParticleSystem brakeSmoke; // ブレーキ時のパーティクルシステム
    private ParticleSystem.EmissionModule brakeSmokeEmission; // ブレーキパーティクルのエミッションモジュール
    public float brakeSmokeRate = 8f; // Emissionモジュール RateDistanceの値
    private bool isBraking = false;
    public float brakeForce = 5.0f; // ブレーキの強さを表すパラメータ
    private float brakeTimer = 0.0f;
    private float brakeInterval = 0.2f; // 連打判定のための間隔（例: 0.5秒）

    public bool isBoost = false;
    public float boostForce = 1.0f; // ブレーキの強さを表すパラメータ
    private float boostTimer = 0.0f;
    private float boostInterval = 0.2f; // 連打判定のための間隔（例: 0.5秒）

    private float turnVelocity;

    public float acceleration = 1.5f;   // アクセル
    public float deceleration = 2.0f;   // 減速

    public float minTurnSpeed = 0.2f;
    public float maxTurnSpeed = 1.5f;
    public float driftTurnSpeed = 1.0f;

    public float minDampTime = 0.1f;
    public float maxDampTime = 1.0f;
    public float driftDampTime = 0.2f;

    public float currentDampTime;
    public float currentTurnSpeed;
    public float transitionSpeed = 0.5f; // 補間の速度

    private Vector3 currentDirection = Vector3.forward;
    private Vector2 inputMove;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;    // バーチャルカメラ
    private Camera mainCamera;
    private Animator animator;
    private CharacterController characterController;
    private RenderChange renderChange;

    [Space]
    private bool isGround;
    public int conveyorCount = 0;   // 接地しているベルトコンベア数

    // ショップ関連
    [Space]
    [SerializeField] private bool isShop = false; // shopに触れているか
    private Image bButton; // isShopと連動させる
    private PostProcessManager postprocessManager; // postprocessの操作csを取得
    private CanvasManager canvasManager;
    private Canvas shopCanvas;

    // スタン関連
    public GameObject clashEffect;
    public GameObject stunEffect;
    private bool isStun = false;
    [Header("スタン時間"), SerializeField] private float stunnedTime = 3f;
    private bool isBounce = false;
    private float bounceTimer;       // 跳ね返りの持続時間を計測するタイマー
    private Vector3 reflectVector;  // 反射ベクトル
    private InputManager inputManager;

    // オーディオ関連
    private AudioSource audioSource;
    public AudioClip stepSE;
    public AudioClip brakeSE;
    public AudioClip openSE;
    public AudioClip closeSE;
    public AudioClip clashSE;
    public AudioClip stunSE;


    private void Awake()
    {
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        //virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if(!virtualCamera) virtualCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        inputManager = FindObjectOfType<InputManager>();
        audioSource = GetComponent<AudioSource>();
        brakeSmoke = GameObject.Find("EF_BrakeSmoke").GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        if (!clashEffect) Debug.LogError("clashEffectアタッチされていません");

        if (!stunEffect)
        {
            Debug.LogError("stunEffectアタッチされていません");
        }
        else
        {
            stunEffect.SetActive(false);
        }

        if (!boostEffect)
        {
            Debug.LogError("boostEffectアタッチされていません");
        }
        else
        {
            EffectStop(boostEffect);
        }

        if (!successEffect)
        {
            Debug.LogError("successエフェクトがアタッチされていません");
        }
        else
        {
            EffectStop(successEffect);
        }

        if (!amazingEffect)
        {
            Debug.LogError("amazingエフェクトがアタッチされていません");
        }
        else
        {
            EffectStop(amazingEffect);
        }

        bButton = GameObject.Find("bButton").GetComponent<Image>();
        shopCanvas = GameObject.Find("ShopCanvas").GetComponent<Canvas>();
        if (brakeSmoke)
        {
            brakeSmokeEmission = brakeSmoke.emission;
            brakeSmokeEmission.rateOverDistance = 0f; // 初期状態ではパーティクルを生成しない
        }
        else
        {
            Debug.LogError("BrakeSmokeが見つかりません");
        }

        if (!audioSource) Debug.LogError("AudioSouceがアタッチされていません");
        if (!stepSE || !openSE || !closeSE) Debug.LogError("SEがアタッチされていません");

        if (!shopCanvas)
            Debug.LogError("shopCanvasが見つからない");

        // postprocessのモザイクを無効化
        postprocessManager = FindObjectOfType<PostProcessManager>();
        if (!postprocessManager)
        {
            Debug.LogError("PostProcessが見つからない");
        }

        canvasManager = FindObjectOfType<CanvasManager>();
        if (!canvasManager)
        {
            Debug.LogError("CanvasManagerが見つからない");
        }

        if (bButton)
        {
            bButton.enabled = isShop;
        }
        else
        {
            Debug.LogError("bButtonが見つからない");
        }

        renderChange = GameObject.Find("RenderCamera_Player").GetComponent<RenderChange>();
        if (!renderChange)
        {
            Debug.LogError("renderChangeコンポーネントが見つかりません");
        }

        if (virtualCamera)
        {
            virtualCamera.Follow = this.transform.Find("LookPos").gameObject.transform;
            virtualCamera.LookAt = this.transform.Find("LookPos").gameObject.transform;
        }
        else
        {
            Debug.LogError("virtualCameraコンポーネントが見つかりません");
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.3f);
    }

    // ボタンUI表示
    public void EnterShop(bool front)
    {
        isShop = front;
        bButton.enabled = front;
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (isShop)
        {
            inputManager.SwitchToUI(); // ActionMap切り替え
            virtualCamera.GetComponent<CinemachineInputProvider>().enabled = false;  // カメラ止める

            currentSpeed = 0f;

            if (inputMove != Vector2.zero)
                inputMove = Vector2.zero;

            postprocessManager.ActiveDepthOfField(true);
            canvasManager.EnableOnlyThisCanvas(shopCanvas); // shopCanvasのみを表示
            shopCanvas.GetComponent<ShopManager>().GoodsReset();    // goodsのanimationのため

            renderChange.CurrentModelChange();

            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(openSE);
        }
        else
        {
            // ブースト
            if (context.started) // ボタンが押された瞬間
            {
                isBoost = true;
                boostTimer = boostInterval; // タイマーをリセット

                boostEffect.SetActive(true);
                // boostEffectとその子にあるすべてのParticleSystemコンポーネントを取得
                ParticleSystem[] childParticleSystems = boostEffect.GetComponentsInChildren<ParticleSystem>();

                // それぞれのパーティクルシステムを再生
                foreach (var ps in childParticleSystems)
                {
                    ps.Play();
                }

                // あとで消す
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
            }
            else if (context.canceled) // ボタンが離された瞬間(あとで消す)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.3f);
            }
        }
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        if (context.started) // ボタンが押された瞬間
        {
            isBraking = true;
            brakeTimer = brakeInterval; // タイマーをリセット

            audioSource.pitch = 1f;
            audioSource.PlayOneShot(brakeSE);

            if (!animator.GetBool("isBrake"))
            {
                animator.SetBool("isBrake", true);
            }

            // あとで消す
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);

            // ブレーキパーティクルを開始
            if (brakeSmoke)
            {
                brakeSmokeEmission.rateOverDistance = brakeSmokeRate; // 生成率を増加（適切な値に調整）
            }
        }
        else if (context.canceled) // ボタンが離された瞬間(あとで消す)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.3f);
        }
    }

    public void OnReturnMap()
    {
        inputManager.SwitchToPlayer(); // ActionMap切り替え
        virtualCamera.GetComponent<CinemachineInputProvider>().enabled = true;  // カメラ動かす
        postprocessManager.ActiveDepthOfField(false);
        canvasManager.DisableOnlyThisCanvas(shopCanvas);    // shopCanvasのみを非表示

        audioSource.pitch = 1.0f;
        audioSource.PlayOneShot(closeSE);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // 入力値を保持しておく
        inputMove = context.ReadValue<Vector2>();
    }



    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 接地判定
        if (hit.gameObject.tag == "Block")
        {
            //animator.SetBool("isJump", false);
            isGround = true;
        }
        else
        {
            isGround = false;
        }

        if (hit.gameObject.tag == "Build" || hit.gameObject.tag == "SpecialBuild" || hit.gameObject.tag == "Shop")
        {
            if (isDash)
            {
                // 移動ベクトルを取得
                Vector3 moveDirection = hit.moveDirection;
                // 法線ベクトルを取得
                Vector3 normalVector = hit.normal;

                // 入射角を計算する
                float angle = Vector3.Angle(moveDirection, -normalVector);

                if (angle < 60f && !isStun && !isBounce)
                {
                    // 反射ベクトルを計算する
                    reflectVector = -moveDirection;
                    reflectVector.y = 0f;

                    bounceTimer = 0.5f; // 跳ね返り持続時間を設定
                    currentSpeed = 0f;
                    isBounce = true;
                    isDash = false;

                    // 振動を与える
                    CameraManager.Instance.ShakeCamera();
                    ControllerManager.Instance.ShakeController();

                    // 衝突エフェクト生成
                    Instantiate(clashEffect, transform.position, Quaternion.identity);

                    audioSource.pitch = 1f;
                    audioSource.PlayOneShot(clashSE);

                    if (!animator.GetBool("isStun"))
                    {
                        animator.SetBool("isStun", true);
                    }

                }
            }
        }

        if (hit.gameObject.tag == "Nature")
        {
            // 移動ベクトルを取得
            Vector3 moveDirection = hit.moveDirection;

            // 法線ベクトルを取得
            Vector3 normalVector = hit.normal;

            // 入射角を計算する
            float angle = Vector3.Angle(moveDirection, -normalVector);

            if (angle < 60f)
            {
                // 勢いをしっかり殺す
                currentSpeed *= 0.95f;
            }

        }

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
            FadeManager.Instance.LoadScene("TitleScene");

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SuccessMission();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            AmazingMission();

        if (isBounce)
        {
            UpdateBounce();
        }
        else
        {
            UpdateMove();
        }
    }

    private void UpdateBounce()
    {
        // キャラクターを跳ね返らせる
        float bounceForce = bounceTimer * 20;   // 20は適当
        characterController.Move(reflectVector * bounceForce * Time.deltaTime);

        bounceTimer -= Time.deltaTime;

        if (bounceTimer <= 0.0f)
        {
            isBounce = false;
            isStun = true;
            
            Invoke("GetUp", stunnedTime);
            stunEffect.SetActive(true);

            audioSource.pitch = 1f;
            audioSource.PlayOneShot(stunSE);
        }
    }

    private void GetUp()
    {
        isStun = false;
        stunEffect.SetActive(false);
        if (animator.GetBool("isStun"))
        {
            animator.SetBool("isStun", false);
        }
    }

    private void UpdateMove()
    {
        if(isStun)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(stunSE);
            }
            return;
        }

        var cameraAngleY = mainCamera.transform.eulerAngles.y;
        Vector3 targetDirection = currentDirection;

        if (inputMove != Vector2.zero)
        {
            if (!animator.GetBool("isDash") && !isBraking)
            {
                animator.SetBool("isDash", true);
            }

            // 入力に基づいて新しい方向を計算
            Vector3 inputDirection = new Vector3(inputMove.x, 0, inputMove.y).normalized;
            targetDirection = Quaternion.Euler(0, cameraAngleY, 0) * inputDirection;

            // スティック入力方向と車の向きが一致しているかチェック
            if (Vector3.Angle(transform.forward, currentDirection) < 45.0f) // 45度以内の場合
            {
                // 初速を設定
                if (currentSpeed < startSpeed)
                {
                    currentSpeed += startSpeed;
                }
                else
                {
                    if (isBoost)
                    {
                        currentSpeed += (acceleration + boostForce) * Time.deltaTime;
                    }
                    else
                    {
                        currentSpeed += acceleration * Time.deltaTime;
                    }
                }
            }

            // brake中であればブレーキの抵抗も加える
            if (isBraking)
                currentSpeed -= brakeForce * Time.deltaTime;

            float targetDampTime;
            // currentSpeed が maxSpeed の半分以上かどうかに基づいて dampTime を設定
            DashChack();
            if (isDash)
            {
                if (isBraking)
                    targetDampTime = driftDampTime;
                else
                    targetDampTime = maxDampTime;
            }
            else
            {
                targetDampTime = minDampTime;      
            }

            currentDampTime = Mathf.Lerp(currentDampTime, targetDampTime, transitionSpeed);

            var targetAngleY = -Mathf.Atan2(inputMove.y, inputMove.x) * Mathf.Rad2Deg + 90;
            targetAngleY += cameraAngleY;
            var angleY = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, targetAngleY, ref turnVelocity, currentDampTime);
            this.transform.rotation = Quaternion.Euler(0, angleY, 0);

        }
        else
        {
            if (isBraking)
            {
                currentSpeed -= brakeForce * Time.deltaTime;
            }
            else
            {
                currentSpeed -= deceleration * Time.deltaTime;
            }
        }

        // 速度を最大速度と0の間に制限
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

        // ダッシュ終了
        if(currentSpeed <= 0.3f)
        {
            if (animator.GetBool("isDash"))
            {
                animator.SetBool("isDash", false);
                audioSource.Stop();
            }
        }

        // ダッシュアニメーションのスピードを操作(デフォルト値にcurrentSpeedを10で割った値を乗算)
        animator.SetFloat("DashSpeed", currentSpeed / 10f);

        // 足音のピッチを変更
        if (!audioSource.isPlaying && animator.GetBool("isDash"))
        {
            if (!animator.GetBool("isBrake"))
            {
                // 0.5は最低値
                audioSource.pitch = 0.5f + currentSpeed / 10f;
                audioSource.PlayOneShot(stepSE);
            }
        }

        // FOVを計算: currentSpeedが0の場合は90、maxSpeedの場合は110
        //float targetFOV = Mathf.Lerp(90, 110, currentSpeed / maxSpeed);
        //virtualCamera.m_Lens.FieldOfView = targetFOV;

        float targetTurnSpeed;

        // currentSpeed が maxSpeed の半分以上かどうかに基づいて turnSpeed を設定
        DashChack();
        if (isDash)
        {
            if (isBraking)
                targetTurnSpeed = driftTurnSpeed;
            else
                targetTurnSpeed = minTurnSpeed;
        }
        else
        {
            targetTurnSpeed = maxTurnSpeed;
        }

        currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, targetTurnSpeed, transitionSpeed);
        // 現在の方向から目標の方向へと徐々に転換
        currentDirection = Vector3.Lerp(currentDirection, targetDirection, currentTurnSpeed * Time.deltaTime);

        // キャラクターコントローラーを使用して移動
        characterController.Move(currentDirection * currentSpeed * Time.deltaTime);

        // ブレーキタイマーを更新
        if (isBraking)
        {
            brakeTimer -= Time.deltaTime;
            if (brakeTimer <= 0.0f)
            {
                isBraking = false; // タイマーが経過したらブレーキを解除

                if (animator.GetBool("isBrake"))
                {
                    animator.SetBool("isBrake", false);
                }

                // ブレーキが解除されたとき、パーティクルを停止
                if (brakeSmoke)
                {
                    brakeSmokeEmission.rateOverDistance = 0f;
                }
            }
        }

        // ブレーキタイマーを更新
        if (isBoost)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0.0f)
            {
                isBoost = false; // タイマーが経過したらブーストを解除
            }
        }
    }

    private void DashChack()
    {
        if (currentSpeed >= maxSpeed / 2)
        {
            isDash = true;
        }
        else
        {
            isDash = false;
        }
    }

    public void SuccessMission()
    {
        ParticleSystem[] childParticleSystems = successEffect.GetComponentsInChildren<ParticleSystem>();

        // それぞれのパーティクルシステムを再生
        foreach (var ps in childParticleSystems)
        {
            if (!ps.isPlaying)
            {
                ps.Play();
            }
  
        }
    }

    public void AmazingMission()
    {
        // boostEffectとその子にあるすべてのParticleSystemコンポーネントを取得
        ParticleSystem[] childParticleSystems = amazingEffect.GetComponentsInChildren<ParticleSystem>();

        // それぞれのパーティクルシステムを再生
        foreach (var ps in childParticleSystems)
        {
            if (!ps.isPlaying)
            {
                ps.Play();
            }
        }
    }

    private void EffectStop(GameObject effectObj)
    {
        // boostEffectとその子にあるすべてのParticleSystemコンポーネントを取得
        ParticleSystem[] childParticleSystems = effectObj.GetComponentsInChildren<ParticleSystem>();

        // それぞれのパーティクルシステムを再生
        foreach (var ps in childParticleSystems)
        {
            ps.Stop();
        }
    }
}