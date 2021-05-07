using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField] private float bounceForce;
    [SerializeField] private Transform direction;

    private Vector2 repelVector;

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, direction.position - transform.position);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        repelVector = (direction.position - transform.position).normalized;
        BoomObject boom;
        if (collision.transform.TryGetComponent(out boom))
            boom.Boom(repelVector * bounceForce);
    }
}
