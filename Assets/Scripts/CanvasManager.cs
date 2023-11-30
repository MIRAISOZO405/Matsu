using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public Canvas missionCanvas;
    public Canvas scoreCanvas;
    public Canvas shopCanvas;
    public Canvas guideCanvas;
    public Canvas effectCanvas;


    private void Start()
    {
        if (!missionCanvas) Debug.LogError("�A�^�b�`����Ă��܂���");
        if (!scoreCanvas) Debug.LogError("�A�^�b�`����Ă��܂���B");
        if (!shopCanvas) Debug.LogError("�A�^�b�`����Ă��܂���B");
        if (!guideCanvas) Debug.LogError("�A�^�b�`����Ă��܂���B");
        if (!effectCanvas) Debug.LogError("�A�^�b�`����Ă��܂���B");

        DisableAllCanvases();
    }

    private void CheckAndSetCanvas(Canvas canvas, bool state)
    {
        if (canvas)
            canvas.enabled = state;
        else
            Debug.Log($"{canvas.name}���A�^�b�`����ĂȂ�");
    }

    public void DisableAllCanvases()
    {
        SetAllCanvasesState(false);
    }

    public void EnableAllCanvases()
    {
        SetAllCanvasesState(true);
    }

    public void EnableOnlyThisCanvas(Canvas targetCanvas)
    {
        SetOnlyThisCanvasState(targetCanvas, true);
    }

    public void DisableOnlyThisCanvas(Canvas targetCanvas)
    {
        SetOnlyThisCanvasState(targetCanvas, false);
    }

    private void SetAllCanvasesState(bool state)
    {
        CheckAndSetCanvas(missionCanvas, state);
        CheckAndSetCanvas(scoreCanvas, state);
        CheckAndSetCanvas(shopCanvas, state);
        CheckAndSetCanvas(guideCanvas, state);
        CheckAndSetCanvas(effectCanvas, state);

        if (state)
        {
            missionCanvas.gameObject.GetComponent<CanvasFade>().FadeInAlpha();
            scoreCanvas.gameObject.GetComponent<CanvasFade>().FadeInAlpha();
            shopCanvas.gameObject.GetComponent<CanvasFade>().FadeInAlpha();
            guideCanvas.gameObject.GetComponent<CanvasFade>().FadeInAlpha();
            effectCanvas.gameObject.GetComponent<CanvasFade>().FadeInAlpha();
        }
    }

    private void SetOnlyThisCanvasState(Canvas targetCanvas, bool state)
    {
        if(state)
        {
            SetAllCanvasesState(!state);
            targetCanvas.enabled = state;
            targetCanvas.gameObject.GetComponent<CanvasFade>().FadeInAlpha();
        }
        else
        {
            SetAllCanvasesState(!state);
            targetCanvas.enabled = state;  
        }
    }
}
