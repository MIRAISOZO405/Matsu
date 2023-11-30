using System;
using UnityEngine;
using UnityEngine.UI;

public class UISignalLimit : MonoBehaviour
{  
    [Header("fillAmount�̒l"), SerializeField] private float fill = 1f;
    [Header("��������"), SerializeField] private float timeLimit;
    [Header("�c�莞��"), SerializeField] private float currentTimeLimit;  // timeLimit�̏����l���L�^���Ă����ϐ�
    [SerializeField]private Image icon;
    public int missionNo;           // �󂯎��p

    private TimeManager timeManager; // TimeManager�X�N���v�g�ւ̎Q��
    private float dayDurationInMinutes; // TimeManager.cs��������p��
    private float timeMultiplier; 
    private float startTimeInMinutes; 

    void Start()
    {
        if (!icon) Debug.LogError("�A�^�b�`����Ă��܂���");
        icon.fillAmount = fill;
        currentTimeLimit = timeLimit; // �����l���L�^

        timeManager = FindObjectOfType<TimeManager>(); // TimeManager�X�N���v�g������
        dayDurationInMinutes = timeManager.dayDurationInMinutes;
        timeMultiplier = 24 * 60 / dayDurationInMinutes;
        startTimeInMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute + DateTime.Now.Second / 60f + DateTime.Now.Millisecond / 60000f;
    }

    void Update()
    {
        // �A�i���O�N���b�N�Ɠ����悤�Ɏ��Ԃ��v��
        float currentRealMinutesPassed = DateTime.Now.Hour * 60 + DateTime.Now.Minute + DateTime.Now.Second / 60f + DateTime.Now.Millisecond / 60000f;
        float elapsedRealMinutes = currentRealMinutesPassed - startTimeInMinutes;
        float gameMinutesPassed =  (elapsedRealMinutes * timeMultiplier);
        
        // fill�̒l���X�V
        currentTimeLimit = timeLimit - gameMinutesPassed / 60;
        fill = currentTimeLimit / timeLimit;
        icon.fillAmount = fill;

        // timeLimit�̒l������������
        if (timeLimit <= gameMinutesPassed / 60)
        {
            GetComponent<UISignalAnimation>().FadeOutMission(missionNo);
            this.enabled = false;
        }
        
    }
}
