using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpeedoMeter : MonoBehaviour
{
    public Image needle;    // ÉÅÅ[É^Å[ÇÃêj
    public PlayerController playerController;
    private float maxSpeed;
    private float currentSpeed;
    private float needleRotationZ; // êjÇÃåªç›ÇÃâÒì]äpìx

    private void Start()
    {
        if (!playerController)
            GetPlayerScript();

        if (!needle) Debug.LogError("ÉAÉ^ÉbÉ`Ç≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
    }

    private void Update()
    {
        if (!playerController)
            GetPlayerScript();

        // êjÇÃñ⁄ìIÇÃäpìxÇåvéZ
        float targetRotationZ = CalculateRotationZ(playerController.currentSpeed);

        // êjÇÃâÒì]ÇääÇÁÇ©Ç…ï‚ä‘
        DOTween.To(() => needleRotationZ, x => needleRotationZ = x, targetRotationZ, 0.5f)
               .OnUpdate(() => needle.transform.rotation = Quaternion.Euler(0, 0, -needleRotationZ));
    }

    private float CalculateRotationZ(float speed)
    {
        // ç≈ëÂë¨ìxÇ≈180ìxâÒì]Ç∑ÇÈÇÊÇ§Ç…åvéZ
        return (speed / maxSpeed) * 180f;
    }

    private void GetPlayerScript()
    {
        playerController = FindObjectOfType<PlayerController>();

        maxSpeed = playerController.maxSpeed;
        currentSpeed = playerController.currentSpeed;
    }
}
