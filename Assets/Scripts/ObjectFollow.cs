using UnityEngine;

public class ObjectFollow : MonoBehaviour
{
    private GameObject player;
    [Header("ëŒè€Ç∆ÇÃãóó£")] public float offsetY = 0.1f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
    }
    void LateUpdate()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").gameObject;
        }

        Vector3 targetPosition = new Vector3(player.transform.position.x , player.transform.position.y + offsetY, player.transform.position.z);
        transform.position = targetPosition;
    }
}
