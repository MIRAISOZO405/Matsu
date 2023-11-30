//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI; // 必要なライブラリをインポート

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//public class MissionManagerOld : MonoBehaviour
//{
//    public enum MissionType
//    {
//        CountRandom,        // 秒数指定　建物ランダム
//        CountSelect,        // 秒数指定　建物指定
//        ScheduleRandom,     // 日時指定  建物ランダム
//        ScheduleSelect,     // 日時指定  建物指定
//    }
//    [Header("ミッションタイプ"), SerializeField] private MissionType missionType;

//    public enum ColorType
//    {
//        Red,
//        Blue,
//        Green,
//        Yellow,
//    }
//    [Header("カラー"), SerializeField] private ColorType colorType;

//    [Header("シグナルプレハブ")] public GameObject signalPrefab;
//    [Header("ミッションの発生時間"), SerializeField] private List<float> invokeTimes = new List<float>();

//    [System.Serializable]
//    public class MissionData
//    {
//        public float invokeTime; // ミッションの発生時間
//        public GameObject targetPoint; // 目的地となるゲームオブジェクト
//    }

//    [System.Serializable]
//    public class ScheduleSelectMissionData
//    {
//        public int day;
//        public int hour;
//        public GameObject targetPoint;
//        public bool hasBeenExecuted = false;  // このミッションが実行されたかどうかのフラグ
//    }

//    [System.Serializable]
//    public class ScheduleRandomMissionData
//    {
//        public int day;
//        public int hour;
//        public bool hasBeenExecuted = false;  // このミッションが実行されたかどうかのフラグ
//    }

//    [Header("ミッションデータ"), SerializeField]
//    private List<MissionData> missions = new List<MissionData>();

//    [Header("ScheduleSelectdミッションデータ"), SerializeField]
//    private List<ScheduleSelectMissionData> scheduledMissions = new List<ScheduleSelectMissionData>();

//    [Header("ScheduleRandomミッションデータ"), SerializeField]
//    private List<ScheduleRandomMissionData> scheduledRandomMissions = new List<ScheduleRandomMissionData>();

//    private GameObject missionPrefab; // ミッション用のプレハブを格納する変数
//    private int missionNo; // ミッションの番号を割り振る
//    private Transform parentCanvas;
//    private TimeManager timeManager;  // アナログクロックの参照を保存するための変数

//    private void Start()
//    {
//        missionNo = 1;
//        colorType = ColorType.Red;
//        parentCanvas = GameObject.Find("MissionCanvas").transform;

//        switch (missionType)
//        {
//            case MissionType.CountRandom:
//                foreach (float time in invokeTimes)
//                {
//                    Invoke("CreateCountRandomSignal", time);
//                }
//                break;
//            case MissionType.CountSelect:
//                StartCoroutine(CountSelectMissions());
//                break;
//            case MissionType.ScheduleRandom:
//                timeManager = FindObjectOfType<TimeManager>();
//                if (!timeManager)
//                {
//                    Debug.LogError("TimeManager.csが見つかりません");
//                    return;
//                }
//                StartCoroutine(ScheduleRandomMissions());
//                break;
//            case MissionType.ScheduleSelect:
//                timeManager = FindObjectOfType<TimeManager>();  // アナログクロックのインスタンスを探す
//                if (!timeManager)
//                {
//                    Debug.LogError("TimeManager.csが見つかりません");
//                    return;
//                }
//                StartCoroutine(ScheduleSelectMissions());
//                break;
//        }
//    }

//    private IEnumerator CountSelectMissions()
//    {
//        float previousInvokeTime = 0.0f;

//        for (int i = 0; i < missions.Count; i++)
//        {
//            float waitTime = missions[i].invokeTime - previousInvokeTime;
//            yield return new WaitForSeconds(waitTime);
//            CreateScheduleSelectdSignal(missions[i].targetPoint);
//            previousInvokeTime = missions[i].invokeTime;
//        }
//    }

//    private IEnumerator ScheduleSelectMissions()
//    {
//        while (true)
//        {
//            foreach (ScheduleSelectMissionData mission in scheduledMissions)
//            {
//                if (!mission.hasBeenExecuted && timeManager.GetDays() == mission.day && timeManager.GetHour() == mission.hour)
//                {
//                    CreateScheduleSelectdSignal(mission.targetPoint);
//                    mission.hasBeenExecuted = true;  // ミッションが実行されたことを記録
//                }
//            }
//            yield return new WaitForSeconds(1);  // 毎秒チェック

//            配列順に検索する(処理を軽くしたい場合こちらを採用)
//            for (int i = scheduledMissions.Count - 1; i >= 0; i--)
//            {
//                var mission = scheduledMissions[i];
//                if (clock.GetDays() == mission.day && clock.GetHour() == mission.hour)
//                {
//                    CreateScheduleSelectdSignal(mission.targetPoint);
//                    scheduledMissions.RemoveAt(i);
//                }
//            }
//            yield return new WaitForSeconds(1);  // 毎秒チェック
//        }
//    }

//    private IEnumerator ScheduleRandomMissions()
//    {
//        while (true)
//        {
//            foreach (ScheduleRandomMissionData mission in scheduledRandomMissions)
//            {
//                if (!mission.hasBeenExecuted && timeManager.GetDays() == mission.day && timeManager.GetHour() == mission.hour)
//                {
//                    CreateScheduledRandomSignal();
//                    mission.hasBeenExecuted = true;
//                }
//            }
//            yield return new WaitForSeconds(1);
//        }
//    }

//    public void CreateCountRandomSignal()
//    {
//        GameObject signalInstance = Instantiate(signalPrefab, parentCanvas);
//        ChangeColor(signalInstance);
//        GameObject[] builds = GameObject.FindGameObjectsWithTag("Build");
//        if (builds.Length == 0)
//        {
//            Debug.Log("Buildタグが見つかりません");
//            return;
//        }
//        GameObject randomBuild = builds[Random.Range(0, builds.Length)];
//        FacilityMission facilityMission = randomBuild.GetComponent<FacilityMission>();
//        if (facilityMission)
//        {
//            missionPrefab = facilityMission.missionPrefab;
//            facilityMission.missionFlg = true;
//            facilityMission.missionNo = missionNo;
//        }
//        SetSignal(signalInstance, randomBuild);
//    }

//    private void CreateScheduledRandomSignal()
//    {
//        GameObject signalInstance = Instantiate(signalPrefab, parentCanvas);
//        ChangeColor(signalInstance);
//        GameObject[] builds = GameObject.FindGameObjectsWithTag("Build");
//        if (builds.Length == 0)
//        {
//            Debug.LogError("「build」タグのオブジェクトが見つかりません");
//            return;
//        }

//        GameObject randomBuild = builds[Random.Range(0, builds.Length)];
//        FacilityMission facilityMission = randomBuild.GetComponent<FacilityMission>();
//        if (facilityMission)
//        {
//            missionPrefab = facilityMission.missionPrefab;
//            facilityMission.missionFlg = true;
//            facilityMission.missionNo = missionNo;
//        }
//        SetSignal(signalInstance, randomBuild);
//    }

//    public void CreateScheduleSelectdSignal(GameObject targetObject)
//    {
//        GameObject signalInstance = Instantiate(signalPrefab, parentCanvas);
//        ChangeColor(signalInstance);
//        FacilityMission facilityMission = targetObject.GetComponent<FacilityMission>();
//        if (facilityMission)
//        {
//            missionPrefab = facilityMission.missionPrefab;
//            facilityMission.missionFlg = true;
//            facilityMission.missionNo = missionNo;
//        }
//        SetSignal(signalInstance, targetObject);
//    }

//    private void SetSignal(GameObject signalInstance, GameObject targetObject)
//    {
//        SignalPoint signalPoint = signalInstance.GetComponent<SignalPoint>();
//        if (signalPoint)
//        {
//            signalPoint.SetSignal(targetObject);
//            signalPoint.missionNo = missionNo; // シグナルの番号を設定
//        }
//        else
//        {
//            Debug.LogError("SignalPointが見つかりません");
//        }

//        if (parentCanvas)
//        {
//            Transform missionDisplayTransform = parentCanvas.Find("MissionDisplay");
//            SetMissionDisplayColor(missionDisplayTransform, signalInstance);
//        }

//        missionNo++;
//    }

//    private void SetMissionDisplayColor(Transform missionDisplayTransform, GameObject signalInstance)
//    {
//        if (missionDisplayTransform)
//        {
//            MissionDisplay missionDisplay = missionDisplayTransform.GetComponent<MissionDisplay>();
//            if (missionDisplay)
//            {
//                GameObject missionInstance = missionDisplay.AddMission(missionPrefab, missionNo);
//                Transform missionIconTransform = missionInstance.transform.Find("missionIcon");
//                if (missionIconTransform)
//                {
//                    Image missionIcon = missionIconTransform.GetComponent<Image>();
//                    if (missionIcon)
//                    {
//                        missionIcon.color = signalInstance.GetComponentInChildren<Image>().color;
//                    }
//                    else
//                    {
//                        Debug.LogError("Image component not found on the missionIcon!");
//                    }
//                }
//                else
//                {
//                    Debug.LogError("missionIcon child not found under the missionInstance!");
//                }
//            }
//            else
//            {
//                Debug.LogError("MissionDisplay component not found on the MissionDisplay object!");
//            }
//        }
//        else
//        {
//            Debug.LogError("MissionDisplay child not found under MissionCanvas!");
//        }
//    }

//    private void ChangeColor(GameObject signalInstance)
//    {
//        Color targetColor = Color.white; // デフォルトの色

//        対象のColorTypeに基づいて色を設定
//        switch (colorType)
//        {
//            case ColorType.Red:
//                targetColor = Color.red;
//                colorType = ColorType.Blue;
//                break;
//            case ColorType.Blue:
//                targetColor = Color.blue;
//                colorType = ColorType.Green;
//                break;
//            case ColorType.Green:
//                targetColor = Color.green;
//                colorType = ColorType.Yellow;
//                break;
//            case ColorType.Yellow:
//                targetColor = Color.yellow;
//                colorType = ColorType.Red;
//                break;
//        }

//        ColorList
//        black, clear, cyan, gray, mazenta, white

//         すべての子のImageとTextコンポーネントの色を変更
//        foreach (Image img in signalInstance.GetComponentsInChildren<Image>())
//        {
//            img.color = targetColor;
//        }

//        foreach (Text txt in signalInstance.GetComponentsInChildren<Text>())
//        {
//            txt.color = targetColor;
//        }
//    }

//#if UNITY_EDITOR
//    [CustomEditor(typeof(MissionManager))]
//    private class MissionManagerEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            MissionManager myTarget = (MissionManager)target;

//             ミッションタイプのドロップダウンを表示
//            myTarget.missionType = (MissionManager.MissionType)EditorGUILayout.EnumPopup("ミッションタイプ", myTarget.missionType);

//             シグナルプレハブのフィールドを表示
//        myTarget.signalPrefab = (GameObject)EditorGUILayout.ObjectField("Signal Prefab", myTarget.signalPrefab, typeof(GameObject), false);

//             ミッションタイプがCountRandomの場合のみ、invokeTimesリストを表示
//            if (myTarget.missionType == MissionManager.MissionType.CountRandom)
//            {
//                EditorGUILayout.PropertyField(serializedObject.FindProperty("invokeTimes"), true);
//            }
//             else if(myTarget.missionType == MissionManager.MissionType.ScheduleSelect)
//            {
//                EditorGUILayout.PropertyField(serializedObject.FindProperty("scheduledMissions"), true);
//            }
//            else if (myTarget.missionType == MissionManager.MissionType.ScheduleRandom)
//            {
//                EditorGUILayout.PropertyField(serializedObject.FindProperty("scheduledRandomMissions"), true);
//               }
//            else // CountSelectの場合は、ミッションデータリストを表示
//            {
//                EditorGUILayout.PropertyField(serializedObject.FindProperty("missions"), true);
//            }
           

//            serializedObject.ApplyModifiedProperties();  // 変更を保存
//        }
//    }
//#endif
//}

