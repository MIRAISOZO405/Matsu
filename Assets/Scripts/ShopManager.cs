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
        if (!shopDisplay) Debug.LogError("アタッチされていません");
        if (!apart || !mansion || !a || !b ) Debug.LogError("アタッチされていません");
        if (!renderChange) Debug.LogError("アタッチされていません");
        if (!audioSource) Debug.LogError("AudioSouceがアタッチされていません");
        if (!selectSE || !buySE) Debug.LogError("SEがアタッチされていません");
    }


    // 閉じるたび初期位置に
    public void GoodsReset()
    {
        // シーン内のすべてのGoodsPriceコンポーネントを取得
        GoodsPrice[] allGoodsPrices = FindObjectsOfType<GoodsPrice>();

        // 各GoodsPriceコンポーネントでSetSelect関数を呼び出す
        foreach (GoodsPrice goodsAnim in allGoodsPrices)
        {
            goodsAnim.SetSelect(false);
        }

        // グッズセレクトを初期位置に戻す
        goodsSelect = 0;
        currentSelect = apart;
        renderChange.ModelChange(PlayerLevel.Apart);
        currentSelect.GetComponent<GoodsPrice>().SetSelect(true);
        //Change(); // SEを鳴らさないため不採用
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
                Debug.LogError("予期せぬエラー");
                break;
        }

        // シーン内のすべてのGoodsPriceコンポーネントを取得
        GoodsPrice[] allGoodsPrices = FindObjectsOfType<GoodsPrice>();

        // 各GoodsPriceコンポーネントでSetSelect関数を呼び出す
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
            Debug.Log("購入失敗の処理");
        }

    }

}
