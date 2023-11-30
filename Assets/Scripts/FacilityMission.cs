using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerEnums;

public class FacilityMission : MonoBehaviour
{
    [Header("�~�b�V�����v���n�u"), SerializeField]
    public GameObject missionPrefab;

    [Header("�~�b�V����������"), SerializeField]
    public bool missionFlg = false;

    [Header("�~�b�V�����ԍ�"), SerializeField]
    public int missionNo = 0;

    [Header("��Փx"), SerializeField]
    private ScoreEnum scoreEnum;

    private LevelManager levelManager;

    private void Start()
    {
        if (!levelManager) levelManager = FindObjectOfType<LevelManager>();
    
        if (!levelManager) Debug.LogError("LevelManager.cs��������܂���");
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

            // ��v����~�b�V����������
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
