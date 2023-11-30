using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI; // UIエレメント用

public class UISignalAnimation : MonoBehaviour
{
    enum State
    {
        In,
        Stay,
        Out,
        Clear
    }
    private State state = State.In;

    public AnimationCurve movementInCurve; // エディタで設定する移動カーブ
    public AnimationCurve movementOutCurve; // エディタで設定する移動カーブ
    public float moveDistance = 5.0f;    // 移動する距離
    public float duration = 0.5f;        // 移動にかかる時間

    private Vector3 startPosition;
    private float elapsedTime = 0.0f;
    private int missionNo = 0;
    private int point;

    private void Start()
    {
        // 初期位置を記録
        startPosition = transform.position;
    }

    public void FadeOutMission(int no)
    {
        missionNo = no;
        startPosition = transform.position;
        state = State.Out;
        elapsedTime = 0;
    }

    public void ClearMission(int no,int p)
    {
        missionNo = no;
        point = p;
        startPosition = transform.position;
        state = State.Clear;
        elapsedTime = 0;
        UIAnimationCurve.Instance.StartRotateShow(transform);
    }

    private void Update()
    {
        switch(state)
        {
            case State.In:
                // 経過時間を更新
                elapsedTime += Time.deltaTime;

                // カーブを使用して移動の進行度を計算
                float progress = movementInCurve.Evaluate(elapsedTime / duration);

                // 新しい位置を計算
                Vector3 newPosition = startPosition + new Vector3(moveDistance * progress, 0, 0);

                // オブジェクトの位置を更新
                transform.position = newPosition;

                // 移動時間が経過したら停止
                if (elapsedTime >= duration)
                {
                    state = State.Stay;
                    //Vector3 goalPosition = new Vector3(startPosition.x + moveDistance,startPosition.y,startPosition.z);
                    //transform.position = goalPosition;

                }
                break;
            case State.Stay:
                break;
            case State.Out:
                // 経過時間を更新
                elapsedTime += Time.deltaTime;

                // カーブを使用して移動の進行度を計算
                float progress1 = movementOutCurve.Evaluate(elapsedTime / duration);

                // 新しい位置を計算
                Vector3 newPosition1 = startPosition + new Vector3(moveDistance * progress1, 0, 0);

                // オブジェクトの位置を更新
                transform.position = newPosition1;

                // 移動時間が経過したら停止
                if (elapsedTime >= duration)
                {
                    // 1つ上の親からMissionDisplayという名前のオブジェクトを取得
                    Transform parentTransform = transform.parent;
                    if (parentTransform)
                    {
                        MissionManager missionManager = parentTransform.GetComponent<MissionManager>();

                        if (missionManager)
                        {
                            missionManager.FailureMission(missionNo);  // FailureMission関数を呼び出す
                            state = State.Stay;
                        }
                    }
                }
                break;
            case State.Clear:
                // 経過時間を更新
                elapsedTime += Time.deltaTime;

                // カーブを使用して移動の進行度を計算
                float progress2 = movementOutCurve.Evaluate(elapsedTime / duration);

                // 新しい位置を計算
                Vector3 newPosition2 = startPosition + new Vector3(moveDistance * progress2, 0, 0);

                // オブジェクトの位置を更新
                transform.position = newPosition2;

                // 移動時間が経過したら停止
                if (elapsedTime >= duration)
                {
                    // 1つ上の親からMissionDisplayという名前のオブジェクトを取得
                    Transform parentTransform = transform.parent;
                    if (parentTransform)
                    {
                        MissionManager missionManager = parentTransform.GetComponent<MissionManager>();

                        if (missionManager)
                        {
                            missionManager.ClearMission(missionNo,point);  // FailureMission関数を呼び出す
                            state = State.Stay;
                        }
                    }
                }
                break;

        }
        
    }

    public bool CheckMove()
    {
        if (state == State.Stay)
            return true;

        return false;
    }
}
