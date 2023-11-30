using System;
using System.Collections.Generic;
using UnityEngine;
using PlayerEnums;
using UnityEngine.InputSystem;

[Serializable]
public class LevelData
{
    public GameObject model;    // モデル
    public int rent;       // 家賃
    public int maxScore;    // 最大スコア
    public int maxMissions; // ミッション出現数

    [Serializable]
    public class DifficultyScore
    {
        public int Easy;
        public int Normal;
        public int Hard;
    }

    public DifficultyScore difficultyScore = new DifficultyScore();
}

public class LevelManager : MonoBehaviour
{
    [Header("プレイヤーLv"), SerializeField] private PlayerLevel playerLevel;
    [Header("交代エフェクト"), SerializeField] private GameObject changeEffect; // キャラ交換時に出るエフェクト(プレハブ)
    [Header("キャラプレハブ"), SerializeField] private LevelData[] levelData;
    private Transform playerTransform;

    [SerializeField] private RentalIncome rentalIncome;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private InputManager inputManager;

    private void Awake()
    {
        if (!inputManager) inputManager = FindObjectOfType<InputManager>();
        if (!rentalIncome) rentalIncome = FindObjectOfType<RentalIncome>();
        if (!missionManager) missionManager = FindObjectOfType<MissionManager>();
        if (!scoreManager) scoreManager = FindObjectOfType<ScoreManager>();
    }

    private void Start()
    {
        if (!rentalIncome)
        {
            Debug.LogError("アタッチされていません");
        }
        else
        {
            playerLevel = PlayerLevel.House;
            rentalIncome.SetRent(levelData[(int)playerLevel].rent);
        }    

        if (!missionManager)
        {
            Debug.LogError("アタッチされていません");
        }
        else
        {
            missionManager.SetMaxMissions(levelData[(int)playerLevel].maxMissions);
        }

        if (!scoreManager) Debug.LogError("アタッチされていません");
        if (!inputManager) Debug.LogError("アタッチされていません");
    }

    public void SetLevel(PlayerLevel lv)
    {
        playerLevel = lv;

        // 現在のプレイヤーオブジェクトの取得
        GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");

        // InputActionMapを取得
        InputActionMap currentMap = inputManager.GetMap();

        // 現在のプレイヤーの位置と回転の取得
        Vector3 playerPosition = currentPlayer.transform.position;
        Quaternion playerRotation = Quaternion.Euler(0, currentPlayer.transform.rotation.eulerAngles.y, 0);

        // プレイヤーオブジェクトの削除
        Destroy(currentPlayer);

        // 新しいプレイヤーモデルの生成
        GameObject newPlayer = Instantiate(levelData[(int)playerLevel].model, playerPosition, playerRotation);

        // 新しいプレイヤーにInputActionMapを設定
        InputManager newInputManager = newPlayer.GetComponent<InputManager>();
        newInputManager.SetMap(currentMap);

        // レンタル収入とスコアの最大値の設定
        rentalIncome.SetRent(levelData[(int)playerLevel].rent);
        scoreManager.SetMaxScore(levelData[(int)playerLevel].maxScore);

        // エフェクトの生成とスケールの設定
        GameObject instantiatedPrefab = Instantiate(changeEffect, playerPosition, Quaternion.identity) as GameObject;
        instantiatedPrefab.transform.localScale = Vector3.one / 2;

        // レベルとスコアに対応する数字の取得
        LevelData.DifficultyScore stats = levelData[(int)playerLevel].difficultyScore;
        int easyValue = stats.Easy;
        int normalValue = stats.Normal;
        int hardValue = stats.Hard;

        // ミッションマネージャーの設定
        missionManager.SetMaxMissions(levelData[(int)playerLevel].maxMissions);
    }

    public PlayerLevel GetLevel()
    {
        return playerLevel;
    }

    public int GetScore(ScoreEnum score)
    {
        // 現在のプレイヤーレベルに対応する LevelData を取得
        LevelData currentLevelData = levelData[(int)playerLevel];

        // 引数の score に応じて適切なスコアを取得
        switch (score)
        {
            case ScoreEnum.Easy:
                return currentLevelData.difficultyScore.Easy;
            case ScoreEnum.Normal:
                return currentLevelData.difficultyScore.Normal;
            case ScoreEnum.Hard:
                return currentLevelData.difficultyScore.Hard;
            default:
                return 0; // 不明なスコアの場合、0 を返すなど適切な処理を行ってください
        }
    }
}