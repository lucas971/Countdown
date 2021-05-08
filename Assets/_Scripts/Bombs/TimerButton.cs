using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimerButton : MonoBehaviour
{
    [SerializeField] private bool Plus;
    [SerializeField] private Bomb Bomb;

    public void Use()
    {
        if (!Bomb.AllowTimerChange())
            return;

        if (Plus)
            Bomb.AddTimer();
        else
            Bomb.ReduceTimer();
    }
}
