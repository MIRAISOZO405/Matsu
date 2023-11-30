using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("1日の長さ(分)"), Tooltip("Unity上の24時間を現実の何分に設定するか"), SerializeField]
    public float dayDurationInMinutes = 2f; // インスペクターから希望する1日の長さを分単位で設定
    [Header("日数制限"), Tooltip("何日でゲーム終了するか"), SerializeField]
    private int limitDays = 7;

    [Header("開始時刻")] public int startHour = 9; // インスペクターから0～23の間でゲーム開始時の時間を設定
    [Header("経過日数"), SerializeField] private int elapsedDays = 0;

    [Header("○時"), SerializeField] private int currentHour;
    [Header("○分"), SerializeField] private int currentMinute;

    [Space]
    public GameObject minute; // 分針用
    public GameObject hour;  // 時針用

    private float timeMultiplier; // 実時間と指定時間の変換率
    private float startTimeInMinutes; // ゲーム開始時の実際の時間

    private RentalIncome rentalIncome;    // 家賃収入用

    private bool isStart = false;

    public void OnStart()
    {
        if (isStart)
            return;

        isStart = true;

        // 1日の実時間（24時間）と指定時間との変換率を計算
        timeMultiplier = 24 * 60 / dayDurationInMinutes;

        // ゲーム開始時の実際の時間を保存
        startTimeInMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute + DateTime.Now.Second / 60f + DateTime.Now.Millisecond / 60000f;

        rentalIncome = FindObjectOfType<RentalIncome>();
    }

    private void Update()
    {
        if (!isStart)
            return;

        // 現在の経過時間を取得 (分単位)
        float currentRealMinutesPassed = DateTime.Now.Hour * 60 + DateTime.Now.Minute + DateTime.Now.Second / 60f + DateTime.Now.Millisecond / 60000f;

        // ゲーム開始時からの実時間の経過を計算
        float elapsedRealMinutes = currentRealMinutesPassed - startTimeInMinutes;

        // ゲーム内での経過時間を計算
        float gameMinutesPassed = (startHour * 60) + (elapsedRealMinutes * timeMultiplier);

        // dayDurationInMinutes以上現実の時間が経過した場合、経過日数のカウントアップ
        if (elapsedRealMinutes >= dayDurationInMinutes)
        {
            elapsedDays++;

            if (elapsedDays >= limitDays)
            {
                // ゲーム終了
                FadeManager.Instance.LoadScene("TitleScene");
                // FadeManager.Instance.LoadScene("TitleScene");
            }

            // 家賃決済処理
            rentalIncome.RentPayment();

            startTimeInMinutes = currentRealMinutesPassed;
            startHour = 9; // ゲームの開始時刻を9時にリセット
        }

        // 24時間のリセットのため
        gameMinutesPassed %= (24 * 60);

        // 現在のゲーム内の時刻を更新
        currentHour = Mathf.FloorToInt(gameMinutesPassed / 60);
        currentMinute = Mathf.FloorToInt(gameMinutesPassed % 60);

        // 針の角度を計算
        hour.transform.eulerAngles = new Vector3(0, 0, gameMinutesPassed / (12 * 60) * -360);
        minute.transform.eulerAngles = new Vector3(0, 0, gameMinutesPassed / 60 * -360);
    }

    public void SetLimitDays(int add)
    {
        limitDays += add;
    }

    public int GetDays()
    {
        return elapsedDays;
    }

    public int GetHour()
    {
        return currentHour;
    }

    public int GetMinute()
    {
        return currentMinute;
    }
}
