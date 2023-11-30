using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAnim : MonoBehaviour
{
    private GameObject switchButton;
    private Animation anim;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 targetPosition;
    private Vector3 arcHeightVector;

    public float maxArcHeight = 5f;
    public float minArcHeight = 2f;
    public float speed = 10f;
    private float time = 0f;

    private CharacterController charController; // キャラクターコントローラー
    private GameObject objectToMove; // 飛ばすオブジェクト
    private bool isMoving = false;
    public bool isRiding = false;
    public bool isLock = false;

    public SwitchAnim otherSwitch; // 他のスイッチへの参照

    void Start()
    {
        // 子供のSwitchButtonを取得
        switchButton = transform.Find("SwitchButton").gameObject;
        anim = gameObject.GetComponent<Animation>();

        objectToMove = GameObject.FindWithTag("Player");
        charController = objectToMove.GetComponent<CharacterController>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isLock)
                return;

            isRiding = true;
            anim.Play();      
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isRiding = false;

            if (isLock)
                isLock = false;
        }
    }


    public void OnLaunch()
    {
        //アニメーション終了時の処理
        if (!isRiding)
            return;

        //objectToMove = GameObject.FindWithTag("Player");
        //charController = objectToMove.GetComponent<CharacterController>();

        if (objectToMove == null)
        {
            objectToMove = GameObject.FindWithTag("Player");
            charController = objectToMove.GetComponent<CharacterController>();
        }

        time = 0;
        startPosition = objectToMove.transform.position;

        // 3次元空間での距離を計算します。
        float distance = Vector3.Distance(startPosition, targetPosition);

        // distanceの値が0から150の範囲に収まるようにします。
        distance = Mathf.Clamp(distance, 0, 100);

        // distanceを0から150の範囲に基づいてarcHeightVectorをminArcVectorからmaxArcVectorに線形補間します。
        float lerpAmount = distance / 100f;
        arcHeightVector = Vector3.up * Mathf.Lerp(maxArcHeight, minArcHeight, lerpAmount);

        isMoving = true; // 移動を開始
        otherSwitch.isLock = true;
       
    }

    public void SetTargetPosition(GameObject target)
    {
        otherSwitch = target.GetComponent<SwitchAnim>();
        targetPosition = target.transform.position;
    }

    private void Update()
    {
        if (isMoving && charController)
        {
            time += Time.deltaTime;

            float distanceCovered = time * speed;
            Vector3 direction = (targetPosition - startPosition).normalized;
            Vector3 nextPos = startPosition + direction * distanceCovered;
            nextPos.y += arcHeightVector.y * Mathf.Sin(Mathf.Clamp01(distanceCovered / Vector3.Distance(startPosition, targetPosition)) * Mathf.PI);

            // キャラクターコントローラを使用してプレイヤーを移動
            Vector3 moveVector = nextPos - objectToMove.transform.position;
            charController.Move(moveVector);

            // 目標位置に到達したら、移動を終了
            if (Vector3.Distance(objectToMove.transform.position, targetPosition) <= 1.0f)
            {
                isMoving = false;
                //charController = null;
                //objectToMove = null;
            }

        }
    }
}
