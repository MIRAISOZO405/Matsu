using UnityEngine;
using UnityEngine.Video;
using MovieClipEnums;

public class MovieChange : MonoBehaviour
{
    public VideoPlayer videoPlayer; // VideoPlayerコンポーネントへの参照
    public VideoClip movieClip1;  // 切り替えるVideoClipの配列
    public VideoClip movieClip2;  // 切り替えるVideoClipの配列

    private void Awake()
    {
        if (!videoPlayer) GetComponent<VideoPlayer>();
    }
    private void Start()
    {
        if (!videoPlayer) Debug.LogError("videoPlayerが見つかりません");
    }

    void Update()
    {
        // Spaceキーが押された場合、ビデオクリップを切り替える
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeClip(MovieEnum.Clip1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeClip(MovieEnum.Clip2);
        }
    }

    // 特定のクリップに切り替えるメソッド
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
