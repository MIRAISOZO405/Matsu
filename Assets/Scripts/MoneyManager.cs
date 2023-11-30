using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoneyManager : MonoBehaviour
{
    [Header("所持金"), SerializeField] private int money;
    [Header("アニメーション持続時間"), SerializeField] private float durationTime = 1f;
    [SerializeField] private MoneyReflection shopDisplay;    // shop用の金表示
    private Text text;
    private int targetMoney;                // 目標金額を保持するための変数
    private bool fluctuation = false;       // お金が変動中かどうか

    private void Start()
    {
        money = 0;
        text = GetComponentInChildren<Text>();
        if (!text) Debug.LogError("子のTextが見つかりません");

        if (!shopDisplay) Debug.LogError("アタッチされていません");

        //UpdateMoneyDisplay();
    }

    public void AddMoney(int amount)
    {
        if (fluctuation)
            return;

        fluctuation = true;

        targetMoney += amount;

        if (targetMoney < 0)
            targetMoney = 0;

        DOTween.To(() => money, x => money = x, targetMoney, durationTime) // 1.0fはアニメーション時間です。必要に応じて変更してください
           .OnUpdate(() =>
           {
               text.text = "" + FormatMoney(money);
               shopDisplay.CopyText(text.text);
           })
            .OnComplete(() =>
            {
                fluctuation = false;               
            });
    }

    private void Update()
    {
        // あとで消す（モデルチェンジ）
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddMoney(10000000);
        }
    }

    // 3桁区切り
    private string FormatMoney(int amount)
    {
        return string.Format("{0:N0}", amount);
    }

    private void UpdateMoneyDisplay()
    {
        text.text = FormatMoney(money);

        if (shopDisplay)
        {
            shopDisplay.CopyText(text.text);
        }
    }

    public int GetMoney()
    {
        return money;
    }

}


