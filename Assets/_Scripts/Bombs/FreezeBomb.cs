using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeBomb : Bomb
{
    protected override void Explode()
    {
        BoomObject b;
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(transform.position, ExplosionRadius, Vector2.zero, 0, BoomObjectMask))
        {
            hit.transform.TryGetComponent(out b);
            if (b)
                inRadius.Add(b);
        }
        foreach (BoomObject boomObject in inRadius)
        {
            Vector2 dir = boomObject.transform.position - transform.position;
            boomObject.Freeze(ExplosionForce);
        }

        inRadius.Clear();
        GameObject vfx = Instantiate(ExplosionVFX);
        vfx.transform.position = transform.position;
        Destroy(vfx, .5f);

        gameObject.SetActive(false);
    }
}
