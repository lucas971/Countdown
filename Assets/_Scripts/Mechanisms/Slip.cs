using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slip : MonoBehaviour
{
    protected BoomObject boom;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out boom))
            return;

        Debug.Log("test");
        boom.StartSlip();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out boom))
            return;
        boom.StopSlip();
    }
}
