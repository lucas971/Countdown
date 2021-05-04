using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float impactRadius;
    [SerializeField] private float force;
    [Range(0,10)]
    [SerializeField] private int timer;
    [SerializeField] private TMPro.TextMeshPro countdown;

    private float chrono;
    private void Start()
    {
        chrono = 0f;
        countdown.text = timer.ToString();
    }
    private void Update()
    {
        chrono += Time.deltaTime;
        if (chrono >= 1)
        {
            chrono = 0;
            timer--;
            countdown.text = timer.ToString();
            if (timer == 0)
                Explode();
        }
    }

    private void Explode()
    {
        foreach (BoomObject boomObject in BoomManager.Instance.GetObjectsInExplosion((Vector2)transform.position, impactRadius))
        {
            Vector2 dir = boomObject.transform.position - transform.position;
            boomObject.Boom(dir * force);
        }
        Destroy(gameObject);
    }
}
