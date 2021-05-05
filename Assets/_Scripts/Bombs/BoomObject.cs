using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoomObject : MonoBehaviour
{
    [SerializeField] private float torqueMultiplier = 1f;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        BoomManager.Instance.AddBoomObject(this);
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void Boom(Vector2 boomVector)
    {
        rb.AddForce(boomVector);
        float torque = Vector2.Angle(boomVector, Vector2.up) * torqueMultiplier;
        rb.AddTorque(torque);
    }
}
