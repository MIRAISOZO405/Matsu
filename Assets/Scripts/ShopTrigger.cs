using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(!player)
                player = GameObject.FindGameObjectWithTag("Player").gameObject;

            player.GetComponent<PlayerController>().EnterShop(true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!player)
                player = GameObject.FindGameObjectWithTag("Player").gameObject;

            player.GetComponent<PlayerController>().EnterShop(false);


        }
    }
}
