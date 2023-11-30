using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        Vector3 p = mainCamera.transform.position;
        transform.LookAt(p);
    }
}
