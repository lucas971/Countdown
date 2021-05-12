using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanismTimer : MonoBehaviour
{
    [SerializeField] private int Timer;
    [SerializeField] private int StartTimer;
    [SerializeField] private Mechanism ToActivate;

    private bool sequenceStarted;
    private int currentTimer;
    private float chrono;

    public void InitiateSequence()
    {
        sequenceStarted = true;
        chrono = StartTimer;
        currentTimer = StartTimer;
    }

    public void Stop()
    {
        sequenceStarted = false;
        ToActivate.Disable();
    }

    protected void Update()
    {
        if (!sequenceStarted || chrono > Timer)
            return;

        chrono += Time.deltaTime;
        if (Mathf.FloorToInt(chrono) > currentTimer)
            TimerChanged();
    }

    protected void TimerChanged()
    {
        currentTimer++;
        if (currentTimer == Timer)
            ToActivate.Activate();
    }

}
