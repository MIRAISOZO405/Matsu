using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    [Header("fillAmount�̒l"), SerializeField] private float fill = 0f;
    [Header("���݃X�R�A"), SerializeField] private int currentScore = 0;
    [Header("�ő�X�R�A"), SerializeField] private int maxScore = 100;
    [Header("�A�j���[�V������������"), SerializeField] private float durationTime = 1f;
    private int displayCurrentScore;
    private int displayMaxScore;

    [SerializeField] private Text text;


    private void Start()
    {
        if (text)
            text.text = currentScore + " / " + maxScore;
        else
            Debug.LogError("�A�^�b�`����Ă��܂���");

    }

    void Update()
    {
        // ���Ƃŏ����i���f���`�F���W�j
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddScore(10);
        }
    }

    public void AddScore(int score)
    {
        currentScore += score;  // �X�R�A��ǉ�

        // �X�R�A���ő�
        if (currentScore >= maxScore)
        {
            currentScore = maxScore;
        }

        // �X�R�A��0�����ɂȂ�Ȃ��悤�ɂ���
        if (currentScore < 0)
            currentScore = 0;

        // ���݂̃X�R�A����ŏI�I�ȃX�R�A�܂ł̐������A�j���[�V����������
        DOTween.To(() => displayCurrentScore, x =>
        {
            displayCurrentScore = x; // �\���p���l���X�V
            text.text = displayCurrentScore + " / " + maxScore; // �e�L�X�g���X�V

        }, currentScore, durationTime);
    }

    public void SetMaxScore(int max)
    {
        maxScore = max;

        DOTween.To(() => displayMaxScore, x =>
        {
            displayMaxScore = x;
            text.text = currentScore + " / " + displayMaxScore;

        }, maxScore, durationTime);
    }

    public int GetScore()
    {
        return currentScore;
    }
}
