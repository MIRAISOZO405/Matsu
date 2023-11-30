using System;
using UnityEngine;
using UnityEngine.UI;

public class UISignalLimit : MonoBehaviour
{  
    [Header("fillAmountの値"), SerializeField] private float fill = 1f;
    [Header("制限時間"), SerializeField] private float timeLimit;
    [Header("残り時間"), SerializeField] private float currentTimeLimit;  // timeLimitの初期値を記録しておく変数
    [SerializeField]private Image icon;
    public int missionNo;           // 受け取る用

    private TimeManager timeManager; // TimeManagerスクリプトへの参照
    private float dayDurationInMinutes; // TimeManager.csから引き継ぐ
    private float timeMultiplier; 
    private float startTimeInMinutes; 

    void Start()
    {
        if (!icon) Debug.LogError("アタッチされていません");
        icon.fillAmount = fill;
        currentTimeLimit = timeLimit; // 初期値を記録

        timeManager = FindObjectOfType<TimeManager>(); // TimeManagerスクリプトを検索
        dayDurationInMinutes = timeManager.dayDurationInMinutes;
        timeMultiplier = 24 * 60 / dayDurationInMinutes;
        startTimeInMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute + DateTime.Now.Second / 60f + DateTime.Now.Millisecond / 60000f;
    }

    void Update()
    {
        // アナログクロックと同じように時間を計測
        float currentRealMinutesPassed = DateTime.Now.Hour * 60 + DateTime.Now.Minute + DateTime.Now.Second / 60f + DateTime.Now.Millisecond / 60000f;
        float elapsedRealMinutes = currentRealMinutesPassed - startTimeInMinutes;
        float gameMinutesPassed =  (elapsedRealMinutes * timeMultiplier);
        
        // fillの値を更新
        currentTimeLimit = timeLimit - gameMinutesPassed / 60;
        fill = currentTimeLimit / timeLimit;
        icon.fillAmount = fill;

        // timeLimitの値を減少させる
        if (timeLimit <= gameMinutesPassed / 60)
        {
            GetComponent<UISignalAnimation>().FadeOutMission(missionNo);
            this.enabled = false;
        }
        
    }
}
