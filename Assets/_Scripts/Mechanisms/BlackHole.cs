using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] private float AttractionRadius = 2f;
    [SerializeField] private float AttractionForce = 20f;
    [SerializeField] private float KillRadius = .1f;
    [SerializeField] protected LayerMask BoomObjectMask;

    private void FixedUpdate()
    {
        BoomObject b;
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(transform.position, AttractionRadius, Vector2.zero, 0, BoomObjectMask))
        {
            hit.transform.TryGetComponent(out b);
            if (b)
            {
               /*if ((transform.position - b.transform.position).magnitude < KillRadius)
                {
                    b.Death();
                    continue;
                }*/

                Vector2 attraction = AttractionForce * (transform.position - b.transform.position) / Mathf.Pow(AttractionRadius, 2);
                b.Attractor(attraction);
                
            }
        }
    }
}
