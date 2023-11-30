using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SignalPoint : MonoBehaviour
{
    [Header("シグナル発生源のオブジェクト"), SerializeField] public Transform signalPoint;  //ターゲット   
    public int missionNo = 0;
    private Image signalArrow; //画像
    private Camera mainCamera;  //対象カメラ
    private RectTransform rectTransform;
    private RectTransform textRectTransform;

    private Transform player;   // Playerの位置
    private Text distanceText; // 距離を表示するUIテキスト

    private GameObject uiElement; // 切り替えたいUI要素
    private GameObject targetObj;
    private Vector3 textPos;

    public float radius = 10.0f; //円の半径

    private void Start()
    {
        signalArrow = transform.Find("SignalArrow").GetComponent<Image>();  // 子のimageを取得
        targetObj = transform.Find("SignalArrow").gameObject;
        distanceText = transform.Find("distanceText").GetComponent<Text>(); // 子のTextを取得

        if(!signalArrow || !targetObj || !distanceText)
        {
            Debug.LogError("見つかりません");
        }

        // 都度切り替える必要あり
        player = GameObject.FindGameObjectWithTag("Player").transform;   // Plyerを取得
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        rectTransform = GetComponent<RectTransform>();
        textRectTransform = distanceText.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // オブジェクト間の距離を計算
        float distance = Vector3.Distance(player.position, signalPoint.position);

        // 距離をUIテキストに表示
        distanceText.text = distance.ToString("F2") + "M"; // 2桁の小数点まで表示
    }


    private void LateUpdate()
    {
        // Playerモデルがチェンジして、Playerを見失ったとき
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;   // Plyerを取得
        }

        if (!signalPoint)
        {
            return;
        }

        int aspect = Screen.width / Screen.height;
        // ルート（Canvas）のスケール値を取得する
        float canvasScale = transform.root.localScale.z;//root=一番上のtransfoorm　なった場合自身のtransform　nullは返さない
        var center = 0.5f * new Vector3(Screen.width, Screen.height);//canvasの中点

        // （画面中心を原点(0,0)とした）ターゲットのスクリーン座標を求める
        var pos = mainCamera.WorldToScreenPoint(signalPoint.position) - center;
        if (pos.z < 0f)//ターゲットがカメラの後方にあったら
        {
            pos.x = -pos.x; // カメラ後方にあるターゲットのスクリーン座標は、画面中心に対する点対称の座標にする
            pos.y = -pos.y;

            // カメラと水平なターゲットのスクリーン座標を補正する
            if (Mathf.Approximately(pos.y, 0f))//2つの float を比較し、それらが互いに小さな値 (Epsilon) の範囲内にある場合 true を返します。
            {
                pos.y = -center.y;
            }
        }

        //画面端の表示位置をUIのサイズの半分だけ画面中心側に寄せて、画面端のUIが見切れないようにします。
        var halfSize = 1.0f * canvasScale * rectTransform.sizeDelta; // UI座標系の値をスクリーン座標系の値に変換する

        float d = Mathf.Max( Mathf.Abs(pos.x / (center.x - halfSize.x)),Mathf.Abs(pos.y / (center.y - halfSize.y)));//2 つ以上の値から最大値を返します。

        // ターゲットのスクリーン座標が画面外なら、画面端になるよう調整する(円形にしたいならここを調整する)
        bool isOffscreen = (pos.z < 0f || d > 1f);//d > 1=画面外 d=スクリーン座標
        if (isOffscreen)
        {
            //ここでやっていることはUIの位置（pos.xとpos.yをスクリーンの外に行かないように値を制限している
            //円形にするにはpos.xとpos.yの値を円を描くように制限すればよい
            // Mathf.Pow(pos.x - (Screen.width / 2),2) + Mathf.Pow(pos.y - (Screen.height / 2),2) = Mathf.Pow(radius, 2); 

            pos.x /= d;//pos はスクリーン座標 UIの座標
            pos.y /= d;//pos.xとpos.yは画面外に行くと困るのでｄで割りつつけて同じ値を維持する。（if文で端に行ったら数値を固定するデモいけそう？）
            float dx = (pos.x / (center.x - halfSize.x));
            float dy = (pos.y / (center.y - halfSize.y));

            if (dx >= 1) // 右
            {
                textPos.x = -61;
                textPos.y = -37;
            }
            if (dx <= -1) // 左
            {
                textPos.x = 65;
                textPos.y = -33;
            }
            if (dy >= 1.0) // 上
            {
                textPos.x = 4;
                textPos.y = -60;
            }
            if (dy < -1) // 下
            {
                textPos.x = 3;
                textPos.y = -8;
            }

            targetObj.SetActive(true);
        }
        else
        {
            textPos.x = 20;
            textPos.y = -9;
            targetObj.SetActive(false); // 表示/非表示を切り替える
        }

        SetTextPosition();

        // スクリン座標系の値をUI座標系の値に変換する
        rectTransform.anchoredPosition = pos / canvasScale;
    }

    private void SetTextPosition()
    {
        float canvasScale = transform.root.localScale.z;    // root=一番上のtransfoorm　なった場合自身のtransform　nullは返さない
        textRectTransform.localPosition = textPos;  // canvasScale;
    }

    public void SetSignal(GameObject target)
    {
        signalPoint = target.transform;
    }
}
