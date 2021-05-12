using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Mechanism
{
    [SerializeField] private GameObject LaserContainer;
    [SerializeField] private MechanismTimer Timer;
    [SerializeField] private int laserDuration;

    private Coroutine laserRoutine;
    private void Start()
    {
        LaserContainer.SetActive(false);
    }

    public override void Activate()
    {
        laserRoutine = StartCoroutine(LaserRoutine());
    }

    public override void Disable()
    {
        StopCoroutine(laserRoutine);
        LaserContainer.SetActive(false);
    }

    protected IEnumerator LaserRoutine()
    {
        LaserContainer.SetActive(true);
        yield return new WaitForSeconds(laserDuration);
        LaserContainer.SetActive(false);
        Timer.InitiateSequence();
    }
}
