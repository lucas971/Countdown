using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float impactRadius;
    [SerializeField] private float force;
    [Range(0,10)]
    [SerializeField] private int timer;
    [SerializeField] private TMPro.TextMeshPro countdown;
    [SerializeField] private float bombDragSpeed;


    private int runtimeTimer;
    private bool started = false;
    private bool placed = false;
    private Vector3 mousePosition;
    private float chrono;
    private void Start()
    {
        chrono = 0f;
        UpdateTimerText();
    }

    public void Place()
    {
        placed = true;
    }

    public void InitiateSequence()
    {
        started = true;
        chrono = 0f;
        runtimeTimer = timer;
    }

    public void SetTimer(int timer)
    {
        this.timer = timer;
        runtimeTimer = timer;
        UpdateTimerText();
    }

    private void Update()
    {
        if (!placed)
        {
            mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = Vector2.Lerp(transform.position, mousePosition + Vector3.down, bombDragSpeed);
        }
        if (!started)
            return;

        chrono += Time.deltaTime;
        if (chrono >= 1)
        {
            chrono = 0;
            runtimeTimer--;
            UpdateTimerText();
            if (runtimeTimer == 0)
                Explode();
        }
    }
    private void Explode()
    {
        foreach (BoomObject boomObject in BoomManager.Instance.GetObjectsInExplosion((Vector2)transform.position, impactRadius))
        {
            Vector2 dir = boomObject.transform.position - transform.position;
            boomObject.Boom(dir.normalized * force );
        }
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        started = false;
        gameObject.SetActive(true);
        runtimeTimer = timer;
        UpdateTimerText();

    }
    private void UpdateTimerText()
    {
        countdown.text = runtimeTimer.ToString();
    }
}
