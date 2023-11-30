using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    [Header("fillAmountの値"), SerializeField] private float fill = 0f;
    [Header("現在スコア"), SerializeField] private int currentScore = 0;
    [Header("最大スコア"), SerializeField] private int maxScore = 100;
    [Header("アニメーション持続時間"), SerializeField] private float durationTime = 1f;
    private int displayCurrentScore;
    private int displayMaxScore;

    [SerializeField] private Text text;


    private void Start()
    {
        if (text)
            text.text = currentScore + " / " + maxScore;
        else
            Debug.LogError("アタッチされていません");

    }

    void Update()
    {
        // あとで消す（モデルチェンジ）
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddScore(10);
        }
    }

    public void AddScore(int score)
    {
        currentScore += score;  // スコアを追加

        // スコアが最大
        if (currentScore >= maxScore)
        {
            currentScore = maxScore;
        }

        // スコアが0未満にならないようにする
        if (currentScore < 0)
            currentScore = 0;

        // 現在のスコアから最終的なスコアまでの数字をアニメーションさせる
        DOTween.To(() => displayCurrentScore, x =>
        {
            displayCurrentScore = x; // 表示用数値を更新
            text.text = displayCurrentScore + " / " + maxScore; // テキストを更新

        }, currentScore, durationTime);
    }

    public void SetMaxScore(int max)
    {
        maxScore = max;

        DOTween.To(() => displayMaxScore, x =>
        {
            displayMaxScore = x;
            text.text = currentScore + " / " + displayMaxScore;

        }, maxScore, durationTime);
    }

    public int GetScore()
    {
        return currentScore;
    }
}
