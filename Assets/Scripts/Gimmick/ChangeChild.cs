using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeChild : MonoBehaviour
{
    private Vector3 lastPosition;
    private CharacterController playerController;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (playerController != null)
        {
            Vector3 movementDelta = transform.position - lastPosition;
            playerController.Move(movementDelta);
        }

        lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Playerオブジェクトをエレベーターの子オブジェクトとして設定
            other.transform.parent = this.transform;

            // PlayerのCharacterControllerを取得
            playerController = other.gameObject.GetComponent<CharacterController>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Playerオブジェクトの親をnullに設定
            other.transform.parent = null;

            // PlayerのCharacterControllerの参照をリセット
            playerController = null;
        }
    }
}
