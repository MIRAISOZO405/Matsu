using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorAuto : MonoBehaviour
{
    private Transform elevator;
    private Transform pos1;
    private Transform pos2;

    [Header("移動スピード"), SerializeField]
    private float speed = 2.0f; // 移動速度
    private float journeyLength; // 移動距離
    private float startTime; // 移動開始時刻
    private bool movingToPos2 = true; // pos2に向かっているかのフラグ

    private Vector3 lastElevatorPosition;

    [Header("エレベーター停止時間"), SerializeField]
    private float waitTime = 2.0f; // 追加: 停止時間を2秒に設定
    private float currentWaitTime = 0.0f; // 追加: 現在の停止時間

    void Start()
    {
        elevator = transform.Find("Elevator").gameObject.transform;
        pos1 = transform.Find("pos1");
        pos2 = transform.Find("pos2");

        if (elevator == null || pos1 == null || pos2 == null)
            Debug.LogWarning("子オブジェクトが見つらない");

        elevator.position = pos1.position; // 追加: Erevatorの位置をpos1の位置に設定

        pos1.gameObject.SetActive(false);
        pos2.gameObject.SetActive(false);

        startTime = Time.time;
        journeyLength = Vector3.Distance(pos1.position, pos2.position);

        lastElevatorPosition = elevator.position;
    }

    void Update()
    {
        Vector3 currentErevatorPosition = elevator.position;
        Vector3 erevatorMovement = currentErevatorPosition - lastElevatorPosition;

        // エレベーターが停止中の場合、停止時間を減少させる
        if (currentWaitTime > 0)
        {
            currentWaitTime -= Time.deltaTime;
            lastElevatorPosition = elevator.position;
            return; // ここでUpdateを終了
        }

        // 移動の進行率を計算
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;

        if (movingToPos2)
        {
            elevator.position = Vector3.Lerp(pos1.position, pos2.position, fractionOfJourney);

            if (fractionOfJourney >= 1)
            {
                movingToPos2 = false;
                startTime = Time.time + waitTime; // 追加: 次の動きを待ち時間だけ遅らせる
                currentWaitTime = waitTime; // 追加: 停止時間をリセット
            }
        }
        else
        {
            elevator.position = Vector3.Lerp(pos2.position, pos1.position, fractionOfJourney);

            if (fractionOfJourney >= 1)
            {
                movingToPos2 = true;
                startTime = Time.time + waitTime; // 追加: 次の動きを待ち時間だけ遅らせる
                currentWaitTime = waitTime; // 追加: 停止時間をリセット
            }
        }

        lastElevatorPosition = elevator.position;
    }
}
