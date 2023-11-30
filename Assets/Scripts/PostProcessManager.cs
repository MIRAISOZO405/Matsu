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
            Debug.LogError("Volume‚ªŒ©‚Â‚©‚ç‚È‚¢");
            return;
        }

        profile = volume.profile;
        if (profile == null)
        {
            Debug.LogError("Profile‚ªİ’è‚³‚ê‚Ä‚¢‚È‚¢");
            return;
        }

        if (!profile.TryGet<DepthOfField>(out depthOfField))
        {
            Debug.LogError("DepthOfField‚ªProfile‚É‘¶İ‚µ‚È‚¢");
        }
        else
        {
            // ‰Šúó‘Ô‚ğƒIƒt‚Éİ’è
            depthOfField.active = false;
        }

        if (!profile.TryGet<Vignette>(out vignette)) Debug.LogError("Vignette‚ªProfile‚É‘¶İ‚µ‚È‚¢");

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
