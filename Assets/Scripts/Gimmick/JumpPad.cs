using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private SwitchAnim pad1;
    private SwitchAnim pad2;

    private void Start()
    {
        pad1 = transform.Find("JumpPad1").GetComponent<SwitchAnim>();
        pad2 = transform.Find("JumpPad2").GetComponent<SwitchAnim>();

        pad1.SetTargetPosition(pad2.gameObject);
        pad2.SetTargetPosition(pad1.gameObject);
    }
}
