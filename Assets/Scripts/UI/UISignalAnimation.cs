using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI; // UI�G�������g�p

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

    public AnimationCurve movementInCurve; // �G�f�B�^�Őݒ肷��ړ��J�[�u
    public AnimationCurve movementOutCurve; // �G�f�B�^�Őݒ肷��ړ��J�[�u
    public float moveDistance = 5.0f;    // �ړ����鋗��
    public float duration = 0.5f;        // �ړ��ɂ����鎞��

    private Vector3 startPosition;
    private float elapsedTime = 0.0f;
    private int missionNo = 0;
    private int point;

    private void Start()
    {
        // �����ʒu���L�^
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
                // �o�ߎ��Ԃ��X�V
                elapsedTime += Time.deltaTime;

                // �J�[�u���g�p���Ĉړ��̐i�s�x���v�Z
                float progress = movementInCurve.Evaluate(elapsedTime / duration);

                // �V�����ʒu���v�Z
                Vector3 newPosition = startPosition + new Vector3(moveDistance * progress, 0, 0);

                // �I�u�W�F�N�g�̈ʒu���X�V
                transform.position = newPosition;

                // �ړ����Ԃ��o�߂������~
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
                // �o�ߎ��Ԃ��X�V
                elapsedTime += Time.deltaTime;

                // �J�[�u���g�p���Ĉړ��̐i�s�x���v�Z
                float progress1 = movementOutCurve.Evaluate(elapsedTime / duration);

                // �V�����ʒu���v�Z
                Vector3 newPosition1 = startPosition + new Vector3(moveDistance * progress1, 0, 0);

                // �I�u�W�F�N�g�̈ʒu���X�V
                transform.position = newPosition1;

                // �ړ����Ԃ��o�߂������~
                if (elapsedTime >= duration)
                {
                    // 1��̐e����MissionDisplay�Ƃ������O�̃I�u�W�F�N�g���擾
                    Transform parentTransform = transform.parent;
                    if (parentTransform)
                    {
                        MissionManager missionManager = parentTransform.GetComponent<MissionManager>();

                        if (missionManager)
                        {
                            missionManager.FailureMission(missionNo);  // FailureMission�֐����Ăяo��
                            state = State.Stay;
                        }
                    }
                }
                break;
            case State.Clear:
                // �o�ߎ��Ԃ��X�V
                elapsedTime += Time.deltaTime;

                // �J�[�u���g�p���Ĉړ��̐i�s�x���v�Z
                float progress2 = movementOutCurve.Evaluate(elapsedTime / duration);

                // �V�����ʒu���v�Z
                Vector3 newPosition2 = startPosition + new Vector3(moveDistance * progress2, 0, 0);

                // �I�u�W�F�N�g�̈ʒu���X�V
                transform.position = newPosition2;

                // �ړ����Ԃ��o�߂������~
                if (elapsedTime >= duration)
                {
                    // 1��̐e����MissionDisplay�Ƃ������O�̃I�u�W�F�N�g���擾
                    Transform parentTransform = transform.parent;
                    if (parentTransform)
                    {
                        MissionManager missionManager = parentTransform.GetComponent<MissionManager>();

                        if (missionManager)
                        {
                            missionManager.ClearMission(missionNo,point);  // FailureMission�֐����Ăяo��
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
