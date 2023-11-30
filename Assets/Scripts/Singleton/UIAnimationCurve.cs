using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI�G�������g�p

public class UIAnimationCurve : SingletonMonoBehaviour<UIAnimationCurve>
{
    public AnimationCurve scaleShowCurve;
    public AnimationCurve rotateShowCurve;
    public AnimationCurve alphaShowCurve; // �A���t�@�l�p�̃J�[�u
    public AnimationCurve alphaShowLoopCurve; // �A���t�@�l�p�̃��[�v�J�[�u
    public AnimationCurve testShowLoopCurve; // �A���t�@�l�p�̃��[�v�J�[�u

    private Coroutine alphaShowLoopCoroutine;
    private Coroutine testShowLoopCoroutine;

    // �I�[�f�B�I�֘A
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (!audioSource) Debug.LogError("�A�^�b�`����Ă��܂���");
    }


    public void StartRotateShow(Transform trs)
    {
        StartCoroutine(RotateShow(trs));
    }

    public void StartScaleShow(Transform trs)
    {
        StartCoroutine(ScaleShow(trs));
    }

    public void StartAlphaShow(Transform trs)
    {
        Graphic uiElement = trs.GetComponent<Graphic>();
        if (uiElement != null)
        {
            StartCoroutine(AlphaShow(uiElement));
        }
    }

    public void StartAlphaShowLoop(Transform trs)
    {
        if (alphaShowLoopCoroutine != null)
        {
            StopCoroutine(alphaShowLoopCoroutine);
        }
        alphaShowLoopCoroutine = StartCoroutine(AlphaShowLoop(trs.GetComponent<Graphic>()));
    }

    public void StartTestShowLoop(Transform trs)
    {
        if (testShowLoopCoroutine != null)
        {
            StopCoroutine(testShowLoopCoroutine);
        }
        testShowLoopCoroutine = StartCoroutine(TestShowLoop(trs));
    }


    public IEnumerator RotateShow(Transform trs)
    {
        float timeCnt = 0;
        float aniSpd = 2f;
        while (timeCnt <= 1f)
        {
            Vector3 rot = Vector3.zero;
            rot.z = rotateShowCurve.Evaluate(timeCnt);

            if (trs)
                trs.localEulerAngles = rot;

            timeCnt += aniSpd * 1 / 60f;// Time.deltaTime;

            yield return null;
        }
    }

    public IEnumerator ScaleShow(Transform trs)
    {
        float timeCnt = 0;
        float aniSpd = 2f;
        while (timeCnt <= 1f)
        {
            Vector3 scl = Vector3.one;
            scl *= scaleShowCurve.Evaluate(timeCnt);

            if (trs)
                trs.localScale = scl;

            timeCnt += aniSpd * 1 / 60f;

            yield return null;
        }
    }

    public IEnumerator AlphaShow(Graphic uiElement)
    {
        float timeCnt = 0;
        float aniSpd = 2f;
        while (timeCnt <= 1f)
        {
            float alpha = alphaShowCurve.Evaluate(timeCnt);
            uiElement.canvasRenderer.SetAlpha(alpha);

            timeCnt += aniSpd * 1 / 60f;
            yield return null;
        }
    }

    
    public IEnumerator AlphaShowLoop(Graphic uiElement)
    {
        float aniSpd = 1f;

        while (true) // �������[�v
        {
            float timeCnt = 0;

            while (timeCnt <= 1.5f)
            {
                float alpha = alphaShowLoopCurve.Evaluate(timeCnt);
                if (uiElement)
                    uiElement.canvasRenderer.SetAlpha(alpha);

                timeCnt += aniSpd * 1 / 60f;
                yield return null;
            }
        }
    }

    public IEnumerator TestShowLoop(Transform trs)
    {
  
        float aniSpd = 2f;

        while (true) // �������[�v
        {
            float timeCnt = 0;

            while (timeCnt <= 1f)
            {
                Vector3 scl = Vector3.one;
                scl *= scaleShowCurve.Evaluate(timeCnt);

                if (trs)
                    trs.localScale = scl;

                timeCnt += aniSpd * 1 / 60f;

                yield return null;
            }
        }
    }

    // �A���t�@�l�A�j���[�V�������[�v��~
    public void StopAlphaShowLoop()
    {
        if (alphaShowLoopCoroutine != null)
        {
            StopCoroutine(alphaShowLoopCoroutine);
        }
    }

    public void StopTestShowLoop()
    {
        if (testShowLoopCoroutine != null)
        {
            StopCoroutine(testShowLoopCoroutine);
        }
    }

    public void PlaySE2D(AudioClip audio)
    {
        audioSource.PlayOneShot(audio);
    }
}
