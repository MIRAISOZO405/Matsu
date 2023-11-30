using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltConveyor : MonoBehaviour
{
    private CharacterController characterController;
    public float moveSpeed;
    private bool isRiding; // èÊÇ¡ÇƒÇ¢ÇÈÇ©Ç«Ç§Ç©


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
 
            other.gameObject.GetComponent<PlayerController>().conveyorCount++;
            isRiding = true;
            characterController = other.gameObject.GetComponent<CharacterController>();


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().conveyorCount--;
            isRiding = false;
            characterController = null;
        }
    }

    private void Update()
    {
        ScrollUV();

        if (!isRiding)
            return;

        int cnt = characterController.gameObject.GetComponent<PlayerController>().conveyorCount;

        Vector3 localMovement = new Vector3(0, 0, moveSpeed);
        Vector3 worldMovement = transform.TransformDirection(localMovement) / cnt;
        characterController.Move(worldMovement * Time.deltaTime);
    }

    void ScrollUV()
    {
        var material = GetComponent<Renderer>().material;
        Vector2 offset = material.mainTextureOffset;
        offset += Vector2.up * moveSpeed * Time.deltaTime;
        material.mainTextureOffset = offset;
    }
}
