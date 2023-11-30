using UnityEngine;
using UnityEngine.Video;
using MovieClipEnums;

public class MovieChange : MonoBehaviour
{
    public VideoPlayer videoPlayer; // VideoPlayer�R���|�[�l���g�ւ̎Q��
    public VideoClip movieClip1;  // �؂�ւ���VideoClip�̔z��
    public VideoClip movieClip2;  // �؂�ւ���VideoClip�̔z��

    private void Awake()
    {
        if (!videoPlayer) GetComponent<VideoPlayer>();
    }
    private void Start()
    {
        if (!videoPlayer) Debug.LogError("videoPlayer��������܂���");
    }

    void Update()
    {
        // Space�L�[�������ꂽ�ꍇ�A�r�f�I�N���b�v��؂�ւ���
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeClip(MovieEnum.Clip1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeClip(MovieEnum.Clip2);
        }
    }

    // ����̃N���b�v�ɐ؂�ւ��郁�\�b�h
    public void ChangeClip(MovieEnum clip)
    {
        switch(clip)
        {
            case MovieEnum.Clip1:
                videoPlayer.clip = movieClip1;
                break;
            case MovieEnum.Clip2:
                videoPlayer.clip = movieClip2;
                break;
        }
    }
}
