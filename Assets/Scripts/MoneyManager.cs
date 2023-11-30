using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoneyManager : MonoBehaviour
{
    [Header("������"), SerializeField] private int money;
    [Header("�A�j���[�V������������"), SerializeField] private float durationTime = 1f;
    [SerializeField] private MoneyReflection shopDisplay;    // shop�p�̋��\��
    private Text text;
    private int targetMoney;                // �ڕW���z��ێ����邽�߂̕ϐ�
    private bool fluctuation = false;       // �������ϓ������ǂ���

    private void Start()
    {
        money = 0;
        text = GetComponentInChildren<Text>();
        if (!text) Debug.LogError("�q��Text��������܂���");

        if (!shopDisplay) Debug.LogError("�A�^�b�`����Ă��܂���");

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

        DOTween.To(() => money, x => money = x, targetMoney, durationTime) // 1.0f�̓A�j���[�V�������Ԃł��B�K�v�ɉ����ĕύX���Ă�������
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
        // ���Ƃŏ����i���f���`�F���W�j
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddMoney(10000000);
        }
    }

    // 3����؂�
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


