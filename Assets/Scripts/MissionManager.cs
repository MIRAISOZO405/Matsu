using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI; // �K�v�ȃ��C�u�������C���|�[�g
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

    [Header("�V�O�i���v���n�u")]
    public GameObject signalPrefab_Easy;
    public GameObject signalPrefab_Normal;
    public GameObject signalPrefab_Hard;

    private GameObject missionPrefab; // �~�b�V�����p�̃v���n�u���i�[����ϐ�
    private int giveMissionNo; // �~�b�V�����̔ԍ�������U��


    // �~�b�V���������i�[����N���X�̒�`
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

    // ��Ԉړ������邽�߂̍\����
    private struct MovingMission
    {
        public GameObject missionObject;
        public Vector3 targetPosition;
        public Vector3 startPosition;
        public float timeToMove;
        public float elapsedTime;
    }
    private List<MovingMission> movingMissions = new List<MovingMission>();

    [Header("Text�̏����ʒu")] public Vector3 initialPosition = new Vector3(0, 0, 0); // �����ʒu
    [Header("Text�̊Ԋu")] public float interval = -50f; // Y���W�̊Ԋu
    [Header("��Ԉړ��̑��x"), Tooltip("�������قǑ���")] public float interpolationSpeed = 0.5f;  // 0.5�b�i�f�t�H���g�j�ňړ�����
    [Header("�~�b�V�����o����")] public int maxMissions = 5;
    [Header("��Փx�̏o���m��")]
    [Range(0, 100)] public int probabilityEasy = 50;
    [Range(0, 100)] public int probabilityNormal = 30;
    [Range(0, 100)] public int probabilityHard = 20;  // ����͎����I�Ɍv�Z�����

    [Space, Header("�~�b�V�����ꗗ"), SerializeField]
    private List<MissionInfo> missions = new List<MissionInfo>();   // �~�b�V���������i�[���郊�X�g

    private ScoreEnum[] lastSelectedScores = new ScoreEnum[5]; // ���߂̑I����ێ�����z��
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

        // �m���𒲐�
        probabilityEasy = 100 - probabilityHard - probabilityNormal;
        probabilityEasy = Mathf.Clamp(probabilityEasy, 0, 100); // �m����0�����ɂȂ�Ȃ��悤�ɃN�����v

        giveMissionNo = 1;

        for (int i = 0; i < maxMissions; i++)
        {
            CreateRandomSignal();
        }

        if (!scoreScript) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!parentCanvas) Debug.LogError("�A�^�b�`����Ă��܂���");
    }

    public void CreateRandomSignal()
    {
        // �����_����ScoreEnum��I��
        ScoreEnum scoreEnum = GetRandomScoreEnum();

        // Build�^�O�̃I�u�W�F�N�g�����ׂĎ擾
        GameObject[] builds = GameObject.FindGameObjectsWithTag("SpecialBuild");
        if (builds.Length == 0)
        {
            Debug.Log("SpecialBuild�^�O��������܂���");
            return;
        }

        // �I�΂ꂽScoreEnum�������Ă���I�u�W�F�N�g�̃��X�g���쐬
        List<GameObject> matchingBuilds = new List<GameObject>();
        foreach (var build in builds)
        {
            FacilityMission facilityMission = build.GetComponent<FacilityMission>();

            // ScoreEnum����v�B����missionFlg��false�̃I�u�W�F�N�g��S�Ď擾
            if (facilityMission && facilityMission.GetScoreEnum() == scoreEnum && !facilityMission.missionFlg)
            {
                matchingBuilds.Add(build);
            }
        }

        if (matchingBuilds.Count == 0)
        {
            Debug.LogError("�I�����ꂽScoreEnum�Ɉ�v����Build�I�u�W�F�N�g��������܂���ł����B");
            return;
        }

        // ���̃��X�g���烉���_���ɃI�u�W�F�N�g��I��
        GameObject randomBuild = matchingBuilds[Random.Range(0, matchingBuilds.Count)];

        // ��������
        FacilityMission selectedFacilityMission = randomBuild.GetComponent<FacilityMission>();
        if (selectedFacilityMission)
        {
            missionPrefab = selectedFacilityMission.missionPrefab;
            selectedFacilityMission.missionFlg = true;
            selectedFacilityMission.missionNo = giveMissionNo;
        }

        // ������signalInstance���C���X�^���X�����ĐF��ύX���܂�
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

        int randomValue = Random.Range(0, 100); // 0 - 99 �͈̔͂Ń����_���Ȑ����擾

        if (randomValue < probabilityEasy)
            selectedScore = ScoreEnum.Easy;
        else if (randomValue < probabilityEasy + probabilityNormal)
            selectedScore = ScoreEnum.Normal;
        else
            selectedScore = ScoreEnum.Hard;

        // ���߂�4��Hard�ȊO�̏ꍇ�͋����I��selectedScore��Hard��ݒ�(5��ȓ��ɂ͕K��Hard������)
        if (lastSelectedScores[0] != ScoreEnum.Hard && lastSelectedScores[1] != ScoreEnum.Hard && lastSelectedScores[2] != ScoreEnum.Hard && lastSelectedScores[3] != ScoreEnum.Hard)
        {
            selectedScore = ScoreEnum.Hard;
        }

        // 4�A��Easy�͖����ɂ���
        if (selectedScore == ScoreEnum.Easy && lastSelectedScores[0] == ScoreEnum.Easy && lastSelectedScores[1] == ScoreEnum.Easy && lastSelectedScores[2] == ScoreEnum.Easy)
        {
            int randomValue2 = Random.Range(probabilityEasy, 100);

            if (randomValue2 < probabilityHard)
                selectedScore = ScoreEnum.Normal;
            else
                selectedScore = ScoreEnum.Hard;
        }

        // ���߂̑I�����X�V
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
            signalPoint.missionNo = giveMissionNo; // �V�O�i���̔ԍ���ݒ�
        }
        else
        {
            Debug.LogError("SignalPoint��������܂���");
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
            // �w�肵���ʒu�Ńv���n�u���C���X�^���X�����A���̎Q�Ƃ�MissionInfo�Ɋi�[
            GameObject instance = Instantiate(missionPrefab, transform);

            // �������ꂽ�C���X�^���X����Mission�X�N���v�g���擾���AmissionNo��ݒ�
            UISignalLimit missionScript = instance.GetComponent<UISignalLimit>();
            if (missionScript)
                missionScript.missionNo = missionNo;

            MissionInfo newMission = new MissionInfo(missionNo, missionPrefab);
            newMission.missionInstance = instance;
            missions.Add(newMission);

            // �V�����~�b�V�����𒼐ږړI�̈ʒu�ɔz�u
            Vector3 targetPosition = initialPosition + new Vector3(0, interval * (missions.Count - 1), 0); // �V�����~�b�V�����̈ʒu
            newMission.missionInstance.transform.localPosition = targetPosition;

            return instance; // �������ꂽ�~�b�V�����̃C���X�^���X��Ԃ�
        }
        else
        {
            Debug.LogError("missionPrefab��������܂���");
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
                //Debug.Log("movingMission.missionObject��������܂���");
            }

            if (ratio >= 1)
            {
                movingMissions.RemoveAt(i);
                i--;  // ���X�g�̗v�f���폜�������߁A�C���f�b�N�X���f�N�������g

                if (missions.Count < maxMissions)
                {
                    CreateRandomSignal();  // �ړ����I��������ŐV�����I�u�W�F�N�g�𐶐�
                }
            }
            else
            {
                movingMissions[i] = movingMission;
            }
        }

        // ���}���u(�o�O�̌����ɂȂ邩��)
        if (missions.Count == 0)
        {
           for (int i = 0; i < maxMissions;i++)
            {
                CreateRandomSignal();  // �ړ����I��������ŐV�����I�u�W�F�N�g�𐶐�
            }
        }

    }

    private void UpdateMissionPositions()
    {
        for (int i = 0; i < missions.Count; i++)
        {
            MissionInfo mission = missions[i];

            // �~�b�V�������ړ��\���`�F�b�N
            if (mission.missionInstance.GetComponent<UISignalAnimation>().CheckMove())
            {
                // ���݂�X����Z���̈ʒu��ێ�
                float currentX = mission.missionInstance.transform.localPosition.x;
                float currentZ = mission.missionInstance.transform.localPosition.z;

                // Y���̂ݍX�V
                Vector3 targetPosition = new Vector3(currentX, initialPosition.y + interval * i, currentZ);

                // ��Ԉړ��̂��߂̏���ݒ�
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

        // ���ׂẴ~�b�V�������폜
        List<int> missionsToRemove = new List<int>(); // �폜����~�b�V�����̃��X�g

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


    // �J���[�֘A
    private void SetMissionDisplayColor(Transform missionDisplayTransform, GameObject signalInstance)
    {
        if (missionDisplayTransform)
        {
            GameObject missionInstance = AddMission(missionPrefab, giveMissionNo);
            Transform missionIconTransform = missionInstance.transform.Find("missionFrame");
            if (missionIconTransform)
            {
                // �V�O�i���C���X�^���X����F���擾
                Image childImage = signalInstance.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != signalInstance);
                if (childImage)
                {
                    // missionFrame��Image�R���|�[�l���g�̐F��ݒ�
                    Image missionIcon = missionIconTransform.GetComponent<Image>();
                    missionIcon.color = childImage.color;

                    // missionFrame�̎q����Image�R���|�[�l���g�̐F���ݒ�
                    foreach (Image img in missionIconTransform.GetComponentsInChildren<Image>())
                    {
                        if (img.gameObject != missionIconTransform.gameObject) // missionFrame���g�����O
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
        Color targetColor = Color.white; // �f�t�H���g�̐F

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

        // ���ׂĂ̎q��Image��Text�R���|�[�l���g�̐F��ύX
        Transform signalFrame = signalInstance.transform.Find("SignalFrame");

        if (signalFrame != null)
        {
            // "SignalFrame" �Ƃ��̎q���� Image �R���|�[�l���g���������F��ύX
            foreach (Image img in signalFrame.GetComponentsInChildren<Image>())
            {
                img.color = targetColor;
            }
        }

        Transform signalArrow = signalInstance.transform.Find("SignalArrow");
        signalArrow.GetComponent<Image>().color = targetColor;

        foreach (Text txt in signalInstance.GetComponentsInChildren<Text>())
        {
            if (txt.gameObject != signalInstance)  // �e�I�u�W�F�N�g���X�L�b�v
            {
                txt.color = targetColor;
            }
        }

        colorType = AddColor(colorType);
    }

    private ColorType AddColor(ColorType color)
    {
        ColorType type = ColorType.Red;

        // �Ώۂ�ColorType�Ɋ�Â��ĐF��ݒ�
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
