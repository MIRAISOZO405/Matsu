using System;
using System.Collections.Generic;
using UnityEngine;
using PlayerEnums;
using UnityEngine.InputSystem;

[Serializable]
public class LevelData
{
    public GameObject model;    // ���f��
    public int rent;       // �ƒ�
    public int maxScore;    // �ő�X�R�A
    public int maxMissions; // �~�b�V�����o����

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
    [Header("�v���C���[Lv"), SerializeField] private PlayerLevel playerLevel;
    [Header("���G�t�F�N�g"), SerializeField] private GameObject changeEffect; // �L�����������ɏo��G�t�F�N�g(�v���n�u)
    [Header("�L�����v���n�u"), SerializeField] private LevelData[] levelData;
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
            Debug.LogError("�A�^�b�`����Ă��܂���");
        }
        else
        {
            playerLevel = PlayerLevel.House;
            rentalIncome.SetRent(levelData[(int)playerLevel].rent);
        }    

        if (!missionManager)
        {
            Debug.LogError("�A�^�b�`����Ă��܂���");
        }
        else
        {
            missionManager.SetMaxMissions(levelData[(int)playerLevel].maxMissions);
        }

        if (!scoreManager) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!inputManager) Debug.LogError("�A�^�b�`����Ă��܂���");
    }

    public void SetLevel(PlayerLevel lv)
    {
        playerLevel = lv;

        // ���݂̃v���C���[�I�u�W�F�N�g�̎擾
        GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");

        // InputActionMap���擾
        InputActionMap currentMap = inputManager.GetMap();

        // ���݂̃v���C���[�̈ʒu�Ɖ�]�̎擾
        Vector3 playerPosition = currentPlayer.transform.position;
        Quaternion playerRotation = Quaternion.Euler(0, currentPlayer.transform.rotation.eulerAngles.y, 0);

        // �v���C���[�I�u�W�F�N�g�̍폜
        Destroy(currentPlayer);

        // �V�����v���C���[���f���̐���
        GameObject newPlayer = Instantiate(levelData[(int)playerLevel].model, playerPosition, playerRotation);

        // �V�����v���C���[��InputActionMap��ݒ�
        InputManager newInputManager = newPlayer.GetComponent<InputManager>();
        newInputManager.SetMap(currentMap);

        // �����^�������ƃX�R�A�̍ő�l�̐ݒ�
        rentalIncome.SetRent(levelData[(int)playerLevel].rent);
        scoreManager.SetMaxScore(levelData[(int)playerLevel].maxScore);

        // �G�t�F�N�g�̐����ƃX�P�[���̐ݒ�
        GameObject instantiatedPrefab = Instantiate(changeEffect, playerPosition, Quaternion.identity) as GameObject;
        instantiatedPrefab.transform.localScale = Vector3.one / 2;

        // ���x���ƃX�R�A�ɑΉ����鐔���̎擾
        LevelData.DifficultyScore stats = levelData[(int)playerLevel].difficultyScore;
        int easyValue = stats.Easy;
        int normalValue = stats.Normal;
        int hardValue = stats.Hard;

        // �~�b�V�����}�l�[�W���[�̐ݒ�
        missionManager.SetMaxMissions(levelData[(int)playerLevel].maxMissions);
    }

    public PlayerLevel GetLevel()
    {
        return playerLevel;
    }

    public int GetScore(ScoreEnum score)
    {
        // ���݂̃v���C���[���x���ɑΉ����� LevelData ���擾
        LevelData currentLevelData = levelData[(int)playerLevel];

        // ������ score �ɉ����ēK�؂ȃX�R�A���擾
        switch (score)
        {
            case ScoreEnum.Easy:
                return currentLevelData.difficultyScore.Easy;
            case ScoreEnum.Normal:
                return currentLevelData.difficultyScore.Normal;
            case ScoreEnum.Hard:
                return currentLevelData.difficultyScore.Hard;
            default:
                return 0; // �s���ȃX�R�A�̏ꍇ�A0 ��Ԃ��ȂǓK�؂ȏ������s���Ă�������
        }
    }
}