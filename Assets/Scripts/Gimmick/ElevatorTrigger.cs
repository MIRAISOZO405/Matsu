using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private Elevator elevatorScript;

    private void Start()
    {
        elevatorScript = GetComponentInParent<Elevator>(); // 親オブジェクトのErevatorスクリプトを取得
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elevatorScript.PlayerEnteredElevator(); // プレイヤーが乗ったのを検知したので、メインのスクリプトのメソッドを呼び出す
        }
    }
}
