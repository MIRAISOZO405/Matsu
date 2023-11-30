using UnityEngine;

public class CanvasFade : MonoBehaviour
{
    private bool fadeIn = false;
    private CanvasGroup canvasGroup;
    
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (!canvasGroup) Debug.LogError("canvasGroup‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
    }

    private void Update()
    {
        if (!fadeIn)
            return;

        canvasGroup.alpha += 0.05f;

        if (canvasGroup.alpha >= 1.0f)
        {
            canvasGroup.alpha = 1.0f;
            fadeIn = false;
        }
    }

    public void FadeInAlpha()
    {
        fadeIn = true;
        canvasGroup.alpha = 0.0f;
    }
}
