using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoomObject : MonoBehaviour
{
    #region EDITOR FIELDS
    [Header("Helper")]
    [SerializeField] private TrajectoryHelper Helper;
    [SerializeField] private float HelpersThreshold = 2f;

    [Space]
    [Header("Physics")]
    [SerializeField] private float TorqueMultiplier = 1f;
    [SerializeField] private float MaxAngularVelocity = 20f;
    #endregion

    #region COMPONENTS
    private Rigidbody2D rb;
    #endregion

    #region PRIVATE VARIABLES
    //HELPERS
    private List<TrajectoryHelper> helpers;
    private TrajectoryHelper lastHelper;

    //TRAIL
    private TrailRenderer trailRenderer;

    //INITIAL VALUES
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    //CHRONO
    private float chrono;
    private int currentTime;

    //STATES
    private bool started = false;

    //RIGIDBODY
    private float bounciness;
    private float friction;

    #endregion

    #region INITIALIZATION
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        helpers = new List<TrajectoryHelper>();
    }

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        bounciness = rb.sharedMaterial.bounciness;
        friction = rb.sharedMaterial.friction;
    }
    #endregion

    #region STATE CHANGEMENT
    public void InitiateSequence()
    {
        //Remove all the displayed helpers
        foreach (TrajectoryHelper helperGo in helpers)
            Destroy(helperGo.gameObject);
        helpers.Clear();

        //Clear the trail
        trailRenderer.Clear();
        trailRenderer.emitting = true;

        //Reset the timer
        chrono = 0f;
        currentTime = 0;

        //Go to started state
        started = true;
    }

    public void Stop()
    {
        //End started state
        started = false;

        //Stop trailrenderer emission
        trailRenderer.emitting = false;
        trailRenderer.AddPosition(transform.position); // NEW CODE

        //Reset rigidbody and transform
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
    #endregion

    #region UPDATE
    private void Update()
    {
        if (!started || Helper == null)
            return;

        chrono += Time.deltaTime;
        if (chrono >= 1)
        {
            chrono = 0;
            currentTime++;
            if (lastHelper && Vector3.Distance(transform.position, lastHelper.transform.position) <= HelpersThreshold)
            {
                lastHelper.AddFork(currentTime);
                return;
            }

            if (Vector3.Distance(transform.position, initialPosition) < HelpersThreshold)
                return;

            TrajectoryHelper helperGo = Instantiate(Helper, transform);
            helperGo.transform.SetParent(null);
            helperGo.transform.rotation = Quaternion.identity;
            helperGo.Setup(currentTime);
            helpers.Add(helperGo);
            lastHelper = helperGo;
        }
    }
    #endregion

    #region BOOM
    public void Boom(Vector2 boomVector)
    {
        rb.velocity = rb.velocity / 4f;
        rb.AddForce(boomVector);
        float torque = Vector2.Angle(boomVector, Vector2.up) * TorqueMultiplier;
        rb.AddTorque(torque);
    }
    #endregion

    #region MECHANISMS
    public void Freeze(float time)
    {
        StartCoroutine(FreezeRoutine(time));
    }

    IEnumerator FreezeRoutine(float time)
    {
        float torqueAfterFreeze = rb.angularVelocity / 4;
        RigidbodyConstraints2D backupConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        float t = 0f;
        while (t < time && started)
        {
            t += Time.deltaTime;
            yield return null;
        }

        rb.constraints = backupConstraints;
        rb.velocity = Vector2.down;
        rb.angularVelocity = torqueAfterFreeze;
    }
    public void Bouncer(float strength)
    {
        rb.velocity *= strength;
    }

    public void Attractor(Vector2 attraction)
    {
        rb.AddForce(attraction);
    }

    public void StartSlip()
    {
        rb.sharedMaterial.friction = 0;
        rb.sharedMaterial.bounciness = 0;
    }

    public void StopSlip()
    {
        rb.sharedMaterial.friction = friction;
        rb.sharedMaterial.bounciness = bounciness;
    }
    #endregion

    #region DEATH
    public virtual void Death()
    {
        UIManager.Instance.RetryLevel();
    }
    #endregion

    private void FixedUpdate()
    {
        if (rb.angularVelocity > MaxAngularVelocity)
            rb.angularVelocity = MaxAngularVelocity;
    }
}
