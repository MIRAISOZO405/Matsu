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
    [Header("���i�^�C�v"), SerializeField]
    private GoodsType goodsType;

    [Header("���i"), SerializeField]
    private int price;

    [Header("�݌�"), SerializeField]
    private int stock;

    [Header("�I�𒆂��ǂ���"), SerializeField]
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
        if (!animator) Debug.LogError("animator���A�^�b�`����Ă��܂���");
        if (!text) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!levelManager) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!moneyManager) Debug.LogError("�A�^�b�`����Ă��܂���");

        if (!goodsImage) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!soldOut) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!soldOutBlur) Debug.LogError("�A�^�b�`����Ă��܂���");

        text.text = "��" + FormatMoney(price);
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
                    Debug.LogError("TimeManager��������܂���");
                break;
            case GoodsType.B:
                StockCheck(1);
                // moneyManager.AddMoney(100000);
                break;

        }

       // uiController = GameObject.FindGameObjectWithTag("Player").GetComponent<UIController>();    
       // if (!uiController) Debug.LogError("Player��������܂���");
  
       // uiController.OnQuit();
        moneyManager.AddMoney(-price);   
    }

    private void StockCheck(int add)
    {
        stock-=add;

        if (stock <= 0)
        {
            // �X�^���v�A�j���[�V����
            soldOut.color = new Color(soldOut.color.r, soldOut.color.g, soldOut.color.b, 1f);
            StartStampScaleShow(soldOut.transform);
            StartStampAlphaShow(soldOutBlur.transform);

            // �O���[�X�P�[��
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