using UnityEngine;
using PlayerEnums;

public class ShopManager : MonoBehaviour
{
    private int goodsSelect; // 0.Apart 1.Mansion 2.A 3.B

    [SerializeField] private GameObject shopDisplay;

    [SerializeField] private GameObject apart;
    [SerializeField] private GameObject mansion;
    [SerializeField] private GameObject a;
    [SerializeField] private GameObject b;
    private GameObject currentSelect;

    [SerializeField] private RenderChange renderChange;

    private AudioSource audioSource;
    public AudioClip selectSE;
    public AudioClip buySE;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!shopDisplay) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!apart || !mansion || !a || !b ) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!renderChange) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!audioSource) Debug.LogError("AudioSouce���A�^�b�`����Ă��܂���");
        if (!selectSE || !buySE) Debug.LogError("SE���A�^�b�`����Ă��܂���");
    }


    // ���邽�я����ʒu��
    public void GoodsReset()
    {
        // �V�[�����̂��ׂĂ�GoodsPrice�R���|�[�l���g���擾
        GoodsPrice[] allGoodsPrices = FindObjectsOfType<GoodsPrice>();

        // �eGoodsPrice�R���|�[�l���g��SetSelect�֐����Ăяo��
        foreach (GoodsPrice goodsAnim in allGoodsPrices)
        {
            goodsAnim.SetSelect(false);
        }

        // �O�b�Y�Z���N�g�������ʒu�ɖ߂�
        goodsSelect = 0;
        currentSelect = apart;
        renderChange.ModelChange(PlayerLevel.Apart);
        currentSelect.GetComponent<GoodsPrice>().SetSelect(true);
        //Change(); // SE��炳�Ȃ����ߕs�̗p
    }

    public void GoodsChange(Vector2 direction)
    {
        if (direction.x > 0.5f)
        {
            if (goodsSelect == 3)
                goodsSelect = 0;
            else
                goodsSelect += 1;
        }
        else if (direction.x < -0.5f)
        {
            if (goodsSelect == 0)
                goodsSelect = 3;
            else
                goodsSelect -= 1;
        }
        else
        {
            return;
        }

        Change();    

    }

    private void Change()
    {
        switch (goodsSelect)
        {
            case 0:
                currentSelect = apart;
                renderChange.ModelChange(PlayerLevel.Apart);

                break;
            case 1:
                currentSelect = mansion;
                renderChange.ModelChange(PlayerLevel.Mansion);
                break;
            case 2:
                currentSelect = a;
                renderChange.CurrentModelChange();
                break;
            case 3:
                currentSelect = b;
                renderChange.CurrentModelChange();
                break;
            default:
                Debug.LogError("�\�����ʃG���[");
                break;
        }

        // �V�[�����̂��ׂĂ�GoodsPrice�R���|�[�l���g���擾
        GoodsPrice[] allGoodsPrices = FindObjectsOfType<GoodsPrice>();

        // �eGoodsPrice�R���|�[�l���g��SetSelect�֐����Ăяo��
        foreach (GoodsPrice goodsAnim in allGoodsPrices)
        {
            goodsAnim.SetSelect(false);
        }

        currentSelect.GetComponent<GoodsPrice>().SetSelect(true);
        audioSource.PlayOneShot(selectSE);

    }

    public void GoodsConfirm()
    {
        if (!currentSelect.GetComponent<GoodsPrice>().ChackBuy())
        {
            currentSelect.GetComponent<GoodsPrice>().Buy();
            audioSource.PlayOneShot(buySE);
        }
        else
        {
            Debug.Log("�w�����s�̏���");
        }

    }

}
