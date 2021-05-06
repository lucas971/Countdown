using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryHelper : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshPro time;

    public void Setup(int time)
    {
        this.time.text = time.ToString() + " s.";
    }
}
