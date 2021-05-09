using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        BoomObject b;
        if (collision.gameObject.TryGetComponent(out b))
        {
            b.Death();
        }
    }
}
