using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryHelper : MonoBehaviour
{
    
    [SerializeField] TMPro.TextMeshPro time;
    private int firstTime;

    public void Setup(int time)
    {
        this.time.text = time.ToString() + " s.";
        firstTime = time;
    }

    public void AddFork(int time)
    {
        this.time.text = firstTime.ToString() + "-" + time.ToString() + " s.";
    }
}
