using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class PostProcessManager : MonoBehaviour
{
    private VolumeProfile profile;
    private DepthOfField depthOfField;
    private Vignette vignette;

    public float fadeTime = 1.5f;

    private void Start()
    {
        Volume volume = GetComponent<Volume>();

        if (volume == null)
        {
            Debug.LogError("Volume��������Ȃ�");
            return;
        }

        profile = volume.profile;
        if (profile == null)
        {
            Debug.LogError("Profile���ݒ肳��Ă��Ȃ�");
            return;
        }

        if (!profile.TryGet<DepthOfField>(out depthOfField))
        {
            Debug.LogError("DepthOfField��Profile�ɑ��݂��Ȃ�");
        }
        else
        {
            // ������Ԃ��I�t�ɐݒ�
            depthOfField.active = false;
        }

        if (!profile.TryGet<Vignette>(out vignette)) Debug.LogError("Vignette��Profile�ɑ��݂��Ȃ�");

    }

    public void ActiveDepthOfField(bool active)
    {
        if (depthOfField != null)
        {
            depthOfField.active = active;
        }
    }

    public void FadeVignette(float targetIntensity)
    {
        if (vignette != null)
        {
            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, targetIntensity, fadeTime);
        }
    }

}
