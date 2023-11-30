using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerEnums;

public class FacilityMission : MonoBehaviour
{
    [Header("ミッションプレハブ"), SerializeField]
    public GameObject missionPrefab;

    [Header("ミッション発生中"), SerializeField]
    public bool missionFlg = false;

    [Header("ミッション番号"), SerializeField]
    public int missionNo = 0;

    [Header("難易度"), SerializeField]
    private ScoreEnum scoreEnum;

    private LevelManager levelManager;

    private void Start()
    {
        if (!levelManager) levelManager = FindObjectOfType<LevelManager>();
    
        if (!levelManager) Debug.LogError("LevelManager.csが見つかりません");
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!missionFlg)
            return;

        if (col.gameObject.tag == "Player")
        {
            missionFlg = false;

            GameObject[] missionObjects = GameObject.FindGameObjectsWithTag("Mission");
            List<GameObject> matchingMissions = new List<GameObject>();

            foreach (GameObject missionObject in missionObjects)
            {
                UISignalLimit missionComponent = missionObject.GetComponent<UISignalLimit>();
                if (missionComponent != null && missionComponent.missionNo == missionNo)
                {
                    matchingMissions.Add(missionObject);
                }
            }

            // 一致するミッションを処理
            foreach (GameObject matchedMission in matchingMissions)
            {
                col.GetComponent<PlayerController>().SuccessMission();
                int point = levelManager.GetScore(scoreEnum);

                UISignalAnimation anim = matchedMission.GetComponent<UISignalAnimation>();

                if (anim)
                {
                    anim.ClearMission(missionNo,point);
                }
            }
    

            missionNo = 0;
        }
    }

    public ScoreEnum GetScoreEnum()
    {
        return scoreEnum;
    }
}
