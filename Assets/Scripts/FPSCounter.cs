using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [Header("フレームレート"), SerializeField]
    private int frameRate = 60;

    [SerializeField, Range(0.1f, 1.0f)]
    private float everyCalcurationTime = 0.5f;

    public float Fps { get; private set; }

    // These are used to calculate the FPS
    private int frameCount;
    private float prevTime;
    private Text text;

    private void Awake()
    {
        Application.targetFrameRate = frameRate; // Consider moving this elsewhere if not core to the FPS counter
    }

    private void Start()
    {
        text = GetComponent<Text>();
        ResetCounters();
    }

    private void Update()
    {
        CalculateFPS();
    }

    private void CalculateFPS()
    {
        frameCount++;
        float currentTime = Time.realtimeSinceStartup;
        float elapsed = currentTime - prevTime;

        if (elapsed >= everyCalcurationTime)
        {
            Fps = frameCount / elapsed;
            text.text = $"FPS : {Fps:F3}"; // Display up to 3 decimal places

            ResetCounters();
            prevTime = currentTime;
        }
    }

    private void ResetCounters()
    {
        frameCount = 0;
        prevTime = Time.realtimeSinceStartup;
    }
}
