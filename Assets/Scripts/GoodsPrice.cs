using UnityEngine;
using UnityEngine.UI;
using PlayerEnums;
using System.Collections;

public class GoodsPrice : MonoBehaviour
{
    private enum GoodsType
    {
        Apart,
        Mansion,
        A,
        B,
    }
    [Header("商品タイプ"), SerializeField]
    private GoodsType goodsType;

    [Header("価格"), SerializeField]
    private int price;

    [Header("在庫"), SerializeField]
    private int stock;

    [Header("選択中かどうか"), SerializeField]
    private bool isSelect = false;

    [SerializeField]private LevelManager levelManager;
    [SerializeField] private MoneyManager moneyManager;
    private UIController uiController;
    private Animator animator;
    [SerializeField] private Text text;
    private bool isSoldout = false;

    public Image goodsImage;
    public Image soldOut;
    public Image soldOutBlur;

    public AnimationCurve stampScaleShowCurve;
    public AnimationCurve stampAlphaShowCurve;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        if (!animator) Debug.LogError("animatorがアタッチされていません");
        if (!text) Debug.LogError("アタッチされていません");
        if (!levelManager) Debug.LogError("アタッチされていません");
        if (!moneyManager) Debug.LogError("アタッチされていません");

        if (!goodsImage) Debug.LogError("アタッチされていません");
        if (!soldOut) Debug.LogError("アタッチされていません");
        if (!soldOutBlur) Debug.LogError("アタッチされていません");

        text.text = "￥" + FormatMoney(price);
        AdjustTextWidth();

        StockCheck(0);
    }

    public void SetSelect(bool select)
    {
        isSelect = select;
        animator.SetBool("isSelect", select);
   
    }

    private string FormatMoney(int amount)
    {
        return amount.ToString("N0").Replace(",", ".");
    }

    private void AdjustTextWidth()
    {
        float newWidth = text.preferredWidth;
        text.rectTransform.sizeDelta = new Vector2(newWidth, text.rectTransform.sizeDelta.y);
    }

    public bool ChackBuy()
    {
        if (moneyManager.GetMoney() < price)
            return true;

        return isSoldout;
    }

    public void Buy()
    {
        switch(goodsType)
        {
            case GoodsType.Apart:
                StockCheck(1);
                levelManager.SetLevel(PlayerLevel.Apart);
                break;
            case GoodsType.Mansion:
                StockCheck(1);
                levelManager.SetLevel(PlayerLevel.Mansion);
                break;
            case GoodsType.A:
                StockCheck(1);
                TimeManager time = FindObjectOfType<TimeManager>();
                if (time)
                    time.SetLimitDays(1);
                else
                    Debug.LogError("TimeManagerが見つかりません");
                break;
            case GoodsType.B:
                StockCheck(1);
                // moneyManager.AddMoney(100000);
                break;

        }

       // uiController = GameObject.FindGameObjectWithTag("Player").GetComponent<UIController>();    
       // if (!uiController) Debug.LogError("Playerが見つかりません");
  
       // uiController.OnQuit();
        moneyManager.AddMoney(-price);   
    }

    private void StockCheck(int add)
    {
        stock-=add;

        if (stock <= 0)
        {
            // スタンプアニメーション
            soldOut.color = new Color(soldOut.color.r, soldOut.color.g, soldOut.color.b, 1f);
            StartStampScaleShow(soldOut.transform);
            StartStampAlphaShow(soldOutBlur.transform);

            // グレースケール
            GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            goodsImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            text.text = "SOLD OUT";
            AdjustTextWidth();
            isSoldout = true;
        }
    }

    public void StartStampScaleShow(Transform trs)
    {
        StartCoroutine(StampScaleShow(trs));
    }

    public void StartStampAlphaShow(Transform trs)
    {
        Graphic uiElement = trs.GetComponent<Graphic>();
        if (uiElement != null)
        {
            StartCoroutine(StampAlphaShow(uiElement));
        }
    }

    public IEnumerator StampScaleShow(Transform trs)
    {
        float timeCnt = 0;
        float aniSpd = 2f;
        while (timeCnt <= 1f)
        {
            Vector3 scl = Vector3.one;
            scl *= stampScaleShowCurve.Evaluate(timeCnt);

            if (trs)
                trs.localScale = scl;

            timeCnt += aniSpd * 1 / 60f;

            yield return null;
        }
    }

    public IEnumerator StampAlphaShow(Graphic uiElement)
    {
        float timeCnt = 0;
        float aniSpd = 2f;
        while (timeCnt <= 1f)
        {
            float alpha = stampAlphaShowCurve.Evaluate(timeCnt);
            Color color = uiElement.color;
            color.a = alpha;
            uiElement.color = color;

            timeCnt += aniSpd * 1 / 60f;
            yield return null;
        }
    }
}