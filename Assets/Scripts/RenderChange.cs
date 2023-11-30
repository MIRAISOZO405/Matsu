using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerEnums;

public class RenderChange : MonoBehaviour
{
    [SerializeField] private Transform house;
    [SerializeField] private Transform apart;
    [SerializeField] private Transform mansion;

    private Transform currentTrans;
    [SerializeField] private GameObject levelManager;

    private void Start()
    {
        if (!house || !apart || !mansion)Debug.LogError("アタッチされていません");
        if (!levelManager) Debug.LogError("アタッチされていません");

        currentTrans = house;
    }

    public void ModelChange(PlayerLevel lv)
    {
        switch (lv)
        {
            case PlayerLevel.House:
                currentTrans = house;
                break;
            case PlayerLevel.Apart:
                currentTrans = apart;
                break;
            case PlayerLevel.Mansion:
                currentTrans = mansion;
                break;

        }
        this.transform.position = new Vector3(currentTrans.position.x, transform.position.y, transform.position.z);
    }

    public void CurrentModelChange()
    {
        PlayerLevel lv = levelManager.GetComponent<LevelManager>().GetLevel();
        ModelChange(lv);
    }
}
