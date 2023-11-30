using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI; // 必要なライブラリをインポート
using PlayerEnums;

public class MissionManager : MonoBehaviour
{
    public enum ColorType
    {
        Red,
        Blue,
        Green,
        Yellow,
    }
    private ColorType colorType = ColorType.Red;

    [Header("シグナルプレハブ")]
    public GameObject signalPrefab_Easy;
    public GameObject signalPrefab_Normal;
    public GameObject signalPrefab_Hard;

    private GameObject missionPrefab; // ミッション用のプレハブを格納する変数
    private int giveMissionNo; // ミッションの番号を割り振る


    // ミッション情報を格納するクラスの定義
    [System.Serializable]
    public class MissionInfo
    {
        public int missionNo;
        public GameObject missionPrefab;
        public GameObject missionInstance;

        public MissionInfo(int no, GameObject prefab)
        {
            missionNo = no;
            missionPrefab = prefab;
            missionInstance = null;
        }
    }

    // 補間移動させるための構造体
    private struct MovingMission
    {
        public GameObject missionObject;
        public Vector3 targetPosition;
        public Vector3 startPosition;
        public float timeToMove;
        public float elapsedTime;
    }
    private List<MovingMission> movingMissions = new List<MovingMission>();

    [Header("Textの初期位置")] public Vector3 initialPosition = new Vector3(0, 0, 0); // 初期位置
    [Header("Textの間隔")] public float interval = -50f; // Y座標の間隔
    [Header("補間移動の速度"), Tooltip("小さいほど速い")] public float interpolationSpeed = 0.5f;  // 0.5秒（デフォルト）で移動する
    [Header("ミッション出現数")] public int maxMissions = 5;
    [Header("難易度の出現確率")]
    [Range(0, 100)] public int probabilityEasy = 50;
    [Range(0, 100)] public int probabilityNormal = 30;
    [Range(0, 100)] public int probabilityHard = 20;  // これは自動的に計算される

    [Space, Header("ミッション一覧"), SerializeField]
    private List<MissionInfo> missions = new List<MissionInfo>();   // ミッション情報を格納するリスト

    private ScoreEnum[] lastSelectedScores = new ScoreEnum[5]; // 直近の選択を保持する配列
    [SerializeField] private ScoreManager scoreScript;
    [SerializeField] private Transform parentCanvas;

    private bool isStart = false;

    public void OnStart()
    {
        isStart = true;

        for (int i = 0; i < lastSelectedScores.Length; i++)
        {
            lastSelectedScores[i] = ScoreEnum.Easy;
        }

        // 確率を調整
        probabilityEasy = 100 - probabilityHard - probabilityNormal;
        probabilityEasy = Mathf.Clamp(probabilityEasy, 0, 100); // 確率が0未満にならないようにクランプ

        giveMissionNo = 1;

        for (int i = 0; i < maxMissions; i++)
        {
            CreateRandomSignal();
        }

        if (!scoreScript) Debug.LogError("アタッチされていません");
        if (!parentCanvas) Debug.LogError("アタッチされていません");
    }

    public void CreateRandomSignal()
    {
        // ランダムにScoreEnumを選ぶ
        ScoreEnum scoreEnum = GetRandomScoreEnum();

        // Buildタグのオブジェクトをすべて取得
        GameObject[] builds = GameObject.FindGameObjectsWithTag("SpecialBuild");
        if (builds.Length == 0)
        {
            Debug.Log("SpecialBuildタグが見つかりません");
            return;
        }

        // 選ばれたScoreEnumを持っているオブジェクトのリストを作成
        List<GameObject> matchingBuilds = new List<GameObject>();
        foreach (var build in builds)
        {
            FacilityMission facilityMission = build.GetComponent<FacilityMission>();

            // ScoreEnumが一致。且つmissionFlgがfalseのオブジェクトを全て取得
            if (facilityMission && facilityMission.GetScoreEnum() == scoreEnum && !facilityMission.missionFlg)
            {
                matchingBuilds.Add(build);
            }
        }

        if (matchingBuilds.Count == 0)
        {
            Debug.LogError("選択されたScoreEnumに一致するBuildオブジェクトが見つかりませんでした。");
            return;
        }

        // そのリストからランダムにオブジェクトを選択
        GameObject randomBuild = matchingBuilds[Random.Range(0, matchingBuilds.Count)];

        // 続く処理
        FacilityMission selectedFacilityMission = randomBuild.GetComponent<FacilityMission>();
        if (selectedFacilityMission)
        {
            missionPrefab = selectedFacilityMission.missionPrefab;
            selectedFacilityMission.missionFlg = true;
            selectedFacilityMission.missionNo = giveMissionNo;
        }

        // ここでsignalInstanceをインスタンス化して色を変更します
        GameObject signalPrefab = null;

        switch (scoreEnum)
        {
            case ScoreEnum.Easy:
                signalPrefab = signalPrefab_Easy;
                break;
            case ScoreEnum.Normal:
                signalPrefab = signalPrefab_Normal;
                break;
            case ScoreEnum.Hard:
                signalPrefab = signalPrefab_Hard;
                break;
        }

        GameObject signalInstance = Instantiate(signalPrefab, parentCanvas);
        ChangeColor(signalInstance, scoreEnum);

        SetSignal(signalInstance, randomBuild);
    }

    public ScoreEnum GetRandomScoreEnum()
    {
        ScoreEnum selectedScore;

        int randomValue = Random.Range(0, 100); // 0 - 99 の範囲でランダムな数を取得

        if (randomValue < probabilityEasy)
            selectedScore = ScoreEnum.Easy;
        else if (randomValue < probabilityEasy + probabilityNormal)
            selectedScore = ScoreEnum.Normal;
        else
            selectedScore = ScoreEnum.Hard;

        // 直近の4回がHard以外の場合は強制的にselectedScoreにHardを設定(5回以内には必ずHardが来る)
        if (lastSelectedScores[0] != ScoreEnum.Hard && lastSelectedScores[1] != ScoreEnum.Hard && lastSelectedScores[2] != ScoreEnum.Hard && lastSelectedScores[3] != ScoreEnum.Hard)
        {
            selectedScore = ScoreEnum.Hard;
        }

        // 4連続Easyは無効にする
        if (selectedScore == ScoreEnum.Easy && lastSelectedScores[0] == ScoreEnum.Easy && lastSelectedScores[1] == ScoreEnum.Easy && lastSelectedScores[2] == ScoreEnum.Easy)
        {
            int randomValue2 = Random.Range(probabilityEasy, 100);

            if (randomValue2 < probabilityHard)
                selectedScore = ScoreEnum.Normal;
            else
                selectedScore = ScoreEnum.Hard;
        }

        // 直近の選択を更新
        lastSelectedScores[4] = lastSelectedScores[3];
        lastSelectedScores[3] = lastSelectedScores[2];
        lastSelectedScores[2] = lastSelectedScores[1];
        lastSelectedScores[1] = lastSelectedScores[0];
        lastSelectedScores[0] = selectedScore;

        return selectedScore;
    }

    private void SetSignal(GameObject signalInstance, GameObject targetObject)
    {
        SignalPoint signalPoint = signalInstance.GetComponent<SignalPoint>();
        if (signalPoint)
        {
            signalPoint.SetSignal(targetObject);
            signalPoint.missionNo = giveMissionNo; // シグナルの番号を設定
        }
        else
        {
            Debug.LogError("SignalPointが見つかりません");
        }

        if (parentCanvas)
        {
            Transform missionDisplayTransform = parentCanvas.Find("MissionManager");
            SetMissionDisplayColor(missionDisplayTransform, signalInstance);
        }

        giveMissionNo++;

    }



    public GameObject AddMission(GameObject missionPrefab, int missionNo)
    {
        if (missionPrefab)
        {
            // 指定した位置でプレハブをインスタンス化し、その参照をMissionInfoに格納
            GameObject instance = Instantiate(missionPrefab, transform);

            // 生成されたインスタンスからMissionスクリプトを取得し、missionNoを設定
            UISignalLimit missionScript = instance.GetComponent<UISignalLimit>();
            if (missionScript)
                missionScript.missionNo = missionNo;

            MissionInfo newMission = new MissionInfo(missionNo, missionPrefab);
            newMission.missionInstance = instance;
            missions.Add(newMission);

            // 新しいミッションを直接目的の位置に配置
            Vector3 targetPosition = initialPosition + new Vector3(0, interval * (missions.Count - 1), 0); // 新しいミッションの位置
            newMission.missionInstance.transform.localPosition = targetPosition;

            return instance; // 生成されたミッションのインスタンスを返す
        }
        else
        {
            Debug.LogError("missionPrefabが見つかりません");
            return null;
        }
    }

    private void Update()
    {
        if (!isStart)
            return;

        for (int i = 0; i < movingMissions.Count; i++)
        {
            var movingMission = movingMissions[i];
            movingMission.elapsedTime += Time.deltaTime;
            float ratio = Mathf.Clamp01(movingMission.elapsedTime / movingMission.timeToMove);

            if (movingMission.missionObject != null)
            {
                movingMission.missionObject.transform.localPosition = Vector3.Lerp(movingMission.startPosition, movingMission.targetPosition, ratio);
            }
            else
            {
                //Debug.Log("movingMission.missionObjectが見つかりません");
            }

            if (ratio >= 1)
            {
                movingMissions.RemoveAt(i);
                i--;  // リストの要素を削除したため、インデックスをデクリメント

                if (missions.Count < maxMissions)
                {
                    CreateRandomSignal();  // 移動が終了した後で新しいオブジェクトを生成
                }
            }
            else
            {
                movingMissions[i] = movingMission;
            }
        }

        // 応急処置(バグの原因になるかも)
        if (missions.Count == 0)
        {
           for (int i = 0; i < maxMissions;i++)
            {
                CreateRandomSignal();  // 移動が終了した後で新しいオブジェクトを生成
            }
        }

    }

    private void UpdateMissionPositions()
    {
        for (int i = 0; i < missions.Count; i++)
        {
            MissionInfo mission = missions[i];

            // ミッションが移動可能かチェック
            if (mission.missionInstance.GetComponent<UISignalAnimation>().CheckMove())
            {
                // 現在のX軸とZ軸の位置を保持
                float currentX = mission.missionInstance.transform.localPosition.x;
                float currentZ = mission.missionInstance.transform.localPosition.z;

                // Y軸のみ更新
                Vector3 targetPosition = new Vector3(currentX, initialPosition.y + interval * i, currentZ);

                // 補間移動のための情報を設定
                MovingMission movingMission = new MovingMission
                {
                    missionObject = mission.missionInstance,
                    startPosition = mission.missionInstance.transform.localPosition,
                    targetPosition = targetPosition,
                    timeToMove = interpolationSpeed,
                    elapsedTime = 0
                };
                movingMissions.Add(movingMission);
            }
        }
    }

    public void ClearMission(int missionNo, int point)
    {
        if (RemoveMission(missionNo))
        {
            if (scoreScript)
                scoreScript.AddScore(point);
        }
    }

    public void FailureMission(int missionNo)
    {
        if (RemoveMission(missionNo))
        {
            GameObject[] builds = GameObject.FindGameObjectsWithTag("SpecialBuild");
            foreach (GameObject build in builds)
            {
                FacilityMission facility = build.GetComponent<FacilityMission>();
                if (facility != null && facility.missionNo == missionNo)
                {
                    facility.missionFlg = false;
                }
            }
        }
    }

    private bool RemoveMission(int missionNo)
    {
        MissionInfo missionToRemove = missions.Find(m => m.missionNo == missionNo);

        if (missionToRemove == null)
        {
            Debug.LogWarning($"Mission with number {missionNo} not found in the list!");
            return false;
        }

        missions.Remove(missionToRemove);

        if (missionToRemove.missionInstance != null)
        {
            Destroy(missionToRemove.missionInstance);
        }

        GameObject[] signals = GameObject.FindGameObjectsWithTag("Signal");
        foreach (GameObject signal in signals)
        {
            SignalPoint signalPoint = signal.GetComponent<SignalPoint>();
            if (signalPoint != null && signalPoint.missionNo == missionNo)
            {
                Destroy(signal);
                UpdateMissionPositions();
                return true;
            }
        }
        return false;

    }

    public void SetMaxMissions(int max)
    {

        // すべてのミッションを削除
        List<int> missionsToRemove = new List<int>(); // 削除するミッションのリスト

        for (int i = 0; i < missions.Count; i++)
        {
            int missionNoToRemove = missions[i].missionNo;
            missionsToRemove.Add(missionNoToRemove);
        }

        foreach (int missionNoToRemove in missionsToRemove)
        {
            RemoveMission(missionNoToRemove);
        }

        maxMissions = max;
    }


    // カラー関連
    private void SetMissionDisplayColor(Transform missionDisplayTransform, GameObject signalInstance)
    {
        if (missionDisplayTransform)
        {
            GameObject missionInstance = AddMission(missionPrefab, giveMissionNo);
            Transform missionIconTransform = missionInstance.transform.Find("missionFrame");
            if (missionIconTransform)
            {
                // シグナルインスタンスから色を取得
                Image childImage = signalInstance.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != signalInstance);
                if (childImage)
                {
                    // missionFrameのImageコンポーネントの色を設定
                    Image missionIcon = missionIconTransform.GetComponent<Image>();
                    missionIcon.color = childImage.color;

                    // missionFrameの子供のImageコンポーネントの色も設定
                    foreach (Image img in missionIconTransform.GetComponentsInChildren<Image>())
                    {
                        if (img.gameObject != missionIconTransform.gameObject) // missionFrame自身を除外
                        {
                            img.color = childImage.color;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Image component not found on the missionIcon!");
                }
            }
            else
            {
                Debug.LogError("missionIcon child not found under the missionInstance!");
            }
        }
        else
        {
            Debug.LogError("MissionDisplay child not found under MissionCanvas!");
        }
    }


    private void ChangeColor(GameObject signalInstance, ScoreEnum scoreEnum)
    {
        Color targetColor = Color.white; // デフォルトの色

        switch (colorType)
        {
            case ColorType.Red:
                targetColor = Color.red;
                break;
            case ColorType.Blue:
                targetColor = Color.blue;
                break;
            case ColorType.Green:
                targetColor = Color.green;
                break;
            case ColorType.Yellow:
                targetColor = Color.yellow;
                break;
        }
        // ColorList
        // black,clear,cyan,gray,mazenta,white

        // すべての子のImageとTextコンポーネントの色を変更
        Transform signalFrame = signalInstance.transform.Find("SignalFrame");

        if (signalFrame != null)
        {
            // "SignalFrame" とその子供の Image コンポーネントを検索し色を変更
            foreach (Image img in signalFrame.GetComponentsInChildren<Image>())
            {
                img.color = targetColor;
            }
        }

        Transform signalArrow = signalInstance.transform.Find("SignalArrow");
        signalArrow.GetComponent<Image>().color = targetColor;

        foreach (Text txt in signalInstance.GetComponentsInChildren<Text>())
        {
            if (txt.gameObject != signalInstance)  // 親オブジェクトをスキップ
            {
                txt.color = targetColor;
            }
        }

        colorType = AddColor(colorType);
    }

    private ColorType AddColor(ColorType color)
    {
        ColorType type = ColorType.Red;

        // 対象のColorTypeに基づいて色を設定
        switch (color)
        {
            case ColorType.Red:
                type = ColorType.Blue;
                break;
            case ColorType.Blue:
                type = ColorType.Green;
                break;
            case ColorType.Green:
                type = ColorType.Yellow;
                break;
            case ColorType.Yellow:
                type = ColorType.Red;
                break;
        }

        return type;
    }
}
