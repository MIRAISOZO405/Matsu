using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private Transform elevator;
    private Transform pos1;
    private Transform pos2;

    [Header("移動スピード"), SerializeField]
    private float speed = 2.0f;
    private float journeyLength;
    private float startTime;
    private bool movingToPos2 = true;

    private Vector3 lastElevatorPosition;

    [Header("エレベーター開始待機時間"), SerializeField]
    private float startDelay = 1.0f; // 開始前の待機時間
    private float currentStartDelay = 0.0f;

    private float currentWaitTime = 0.0f;

    private bool playerOnElevator = false;

    void Start()
    {
        elevator = transform.Find("Elevator").gameObject.transform;
        pos1 = transform.Find("pos1");
        pos2 = transform.Find("pos2");

        if (elevator == null || pos1 == null || pos2 == null)
            Debug.LogWarning("子オブジェクトが見つらない");

        elevator.position = pos1.position;

        pos1.gameObject.SetActive(false);
        pos2.gameObject.SetActive(false);

        startTime = Time.time;
        journeyLength = Vector3.Distance(pos1.position, pos2.position);

        lastElevatorPosition = elevator.position;
    }

    void Update()
    {
        if (currentWaitTime > 0)
        {
            currentWaitTime -= Time.deltaTime;
            return;
        }

        if (playerOnElevator && currentStartDelay <= 0) // プレイヤーがエレベーターに乗っていて、開始待機時間が経過した場合
        {
            float distanceCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distanceCovered / journeyLength;

            if (movingToPos2)
            {
                elevator.position = Vector3.Lerp(pos1.position, pos2.position, fractionOfJourney);

                if (fractionOfJourney >= 1)
                {
                    movingToPos2 = false;
                    playerOnElevator = false;
                    startTime = Time.time;
                }
            }
            else
            {
                elevator.position = Vector3.Lerp(pos2.position, pos1.position, fractionOfJourney);

                if (fractionOfJourney >= 1)
                {
                    movingToPos2 = true;
                    playerOnElevator = false;
                    startTime = Time.time;
                }
            }
        }
        else if (playerOnElevator) // プレイヤーがエレベーターに乗っているが、開始待機時間がまだ経過していない場合
        {
            currentStartDelay -= Time.deltaTime;
            if (currentStartDelay <= 0)
            {
                startTime = Time.time; // 開始待機時間が経過したので、移動を開始する
            }
        }
    }

    public void PlayerEnteredElevator()
    {
        if (!playerOnElevator)
        {
            playerOnElevator = true;
            currentStartDelay = startDelay; // 開始待機時間を設定
        }
    }
}
