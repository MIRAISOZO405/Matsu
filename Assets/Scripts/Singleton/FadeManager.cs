using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class FadeManager : MonoBehaviour
{

    #region Singleton

    private static FadeManager instance;

    public static FadeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (FadeManager)FindObjectOfType(typeof(FadeManager));

                if (instance == null)
                {
                    Debug.LogError(typeof(FadeManager) + "is nothing");
                }
            }

            return instance;
        }
    }

    #endregion Singleton

  
    // フェード中の透明度
    public float fadeAlpha = 0;
    // フェード中かどうか
    private bool isFading = false;
    // フェード色
    public Color fadeColor = Color.black;
    // sceneを切り替えたかどうか
    private bool isChange = false;
    // フェード速度
    public float interval;

    // volumeが0になったときの待機時間
    public float volumeTime = 0.5f; 

    public AudioClip titleBGM;
    public AudioClip gameBGM;
    public AudioClip resultBGM;
    private AudioSource audioSource;

    enum BGMState
    {
        Title,
        Game,
        Result,
    }
    private BGMState bgmState;

    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        audioSource = GetComponent<AudioSource>();

        string sceneName = SceneManager.GetActiveScene().name;
        SceneNameCheck(sceneName);
    }

    public void OnGUI()
    {
        // Fade .
        if (this.isFading)
        {
            //色と透明度を更新して白テクスチャを描画 .
            this.fadeColor.a = this.fadeAlpha;
            GUI.color = this.fadeColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(TransScene(scene, interval));   
    }

    private IEnumerator TransScene(string scene, float interval)
    {
        //だんだん暗く .
        this.isFading = true;
        float time = 0;
        while (time <= interval)
        {
            this.fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.deltaTime;
            audioSource.volume = 1f - fadeAlpha;
            yield return 0;
        }

        //シーン切替 .
        if (this.isChange == false)
        {
            SceneManager.LoadScene(scene);
            this.isChange = true;

            audioSource.Stop();
            SceneNameCheck(scene);
        }
        //だんだん明るく .
        time = 0;
        while (time <= interval)
        {
            this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            audioSource.volume = 1f - fadeAlpha;
            yield return 0;
        }
        audioSource.volume = 1f;

        this.isFading = false;
        this.isChange = false;
    }

    private void SceneNameCheck(string scene)
    {
        if (scene == "GameScene")
        {
            audioSource.PlayOneShot(titleBGM);
        }
        else if (scene == "ResultScene")
        {
            audioSource.PlayOneShot(resultBGM);
        }
        else
        {
            audioSource.PlayOneShot(titleBGM);
            Debug.LogError("Scene名が不明");
        }
    }

    public void GameStartBGM()
    {
        StartCoroutine(ChangeBGMWithFade(gameBGM, interval));
    }

    // フェードアウトしてから新しいBGMにフェードインする
    private IEnumerator ChangeBGMWithFade(AudioClip newClip, float fadeDuration)
    {
        yield return StartCoroutine(VolumeOut(fadeDuration));
        yield return StartCoroutine(VolumeIn(newClip, fadeDuration));
    }

    // BGMのフェードアウトを行うコルーチン
    private IEnumerator VolumeOut(float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        yield return new WaitForSeconds(volumeTime); // 1秒待機
        audioSource.volume = startVolume;
    }

    // BGMのフェードインを行うコルーチン
    private IEnumerator VolumeIn(AudioClip newClip, float duration)
    {
        audioSource.clip = newClip;
        audioSource.Play();

        audioSource.volume = 0f;
        float targetVolume = 1f;

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / duration;
            yield return null;
        }
    }
}