using UnityEngine;
using UnityEngine.UI;

public class UIArrow : MonoBehaviour
{
    [SerializeField]private Transform signalPoint;    // 矢印ターゲット
    private Image signalArrow;    // 画像
    private Camera mainCamera;  // 対象カメラ

    private void Start()
    {
        if (!signalPoint) Debug.LogError("アタッチされていません");
        signalArrow = GetComponent<Image>(); // 自身のImageを取得
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        int aspect = Screen.width / Screen.height;  // ルート（Canvas）のスケール値を取得する
        float canvasScale = transform.root.localScale.z;    //root=一番上のtransfoorm　なった場合自身のtransform　nullは返さない
        var center = 0.5f * new Vector3(Screen.width, Screen.height);   //canvasの中点

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

        float d = Mathf.Max(Mathf.Abs(pos.x / center.x ), Mathf.Abs(pos.y / center.y));//2 つ以上の値から最大値を返します。

        // ターゲットのスクリーン座標が画面外なら、画面端になるよう調整する(円形にしたいならここを調整する)
        bool isOffscreen = (pos.z < 0f || d > 1f);//d > 1=画面外 d=スクリーン座標
        if (isOffscreen)
        {
            
            pos.x /= d; //pos はスクリーン座標 UIの座標
            pos.y /= d; //pos.xとpos.yは画面外に行くと困るのでｄで割りつつけて同じ値を維持する。（if文で端に行ったら数値を固定するデモいけそう？）
        }

        signalArrow.rectTransform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg);// ラジアンから度に変換する定数

    }
}