using UnityEngine;
using UnityEngine.UI;

public class MoneyReflection : MonoBehaviour
{
    private Text text;

    private void Start()
    {
        text = GetComponentInChildren<Text>();
        if (!text) Debug.LogError("子のTextが見つかりません");
    }

    public void CopyText(string value)
    {
        text.text = value;
    }
}
