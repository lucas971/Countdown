using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoomObject : MonoBehaviour
{
    [SerializeField] private float torqueMultiplier = 1f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    public void Boom(Vector2 boomVector)
    {
        rb.AddForce(boomVector);
        float torque = Vector2.Angle(boomVector, Vector2.up) * torqueMultiplier;
        rb.AddTorque(torque);
    }
}
