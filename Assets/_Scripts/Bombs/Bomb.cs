using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    #region CONSTS
    public const float gridSize = .5f;
    #endregion

    #region EDITOR FIELDS
    [Header("Physics")]
    [SerializeField] private float ExplosionForce;
    [SerializeField] private float ExplosionRadius;
    [SerializeField] private LayerMask BoomObjectMask;

    [Space]
    [Header("Vfxs")]
    [SerializeField] private GameObject ExplosionVFX;

    [Space]
    [Header("Coutdown")]
    [SerializeField] private TMPro.TextMeshPro CountdownText;

    [Space]
    [Header("Placement")]
    [SerializeField] private float CursorDragSpeed;
    [SerializeField] private GameObject CursorVisuals;
    #endregion

    #region COMPONENTS
    private SpriteRenderer spriteRenderer;
    #endregion

    #region PRIVATE VARIABLES
    //Collider detection
    private List<BoomObject> inRadius;
    private Vector2 mousePosition;

    //Timer
    private int timerBackup;
    private int timer;
    private float chrono;

    //State
    private bool started = false;
    private bool placed = false;
    private bool placable = false;
    #endregion

    #region INITIALIZATION
    private void Awake()
    {
        inRadius = new List<BoomObject>();
        spriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        chrono = 0f;
        UpdateTimerText();
    }
    #endregion

    #region PLACEMENT

    private Vector2 SnapToGrid(Vector2 posToSnap)
    {
        float snappedX;
        float snappedY;

        int xDiv = (int)(posToSnap.x / gridSize) - 1;
        int yDiv = (int)(posToSnap.y / gridSize) - 1;

        snappedX = xDiv * gridSize;
        snappedY = yDiv * gridSize;

        return new Vector2(snappedX, snappedY);
    }
    public bool Place()
    {
        if (placable)
        {
            placed = true;
            return true;
        }
        return false;
    }

    public void InitiateSequence()
    {
        started = true;
        chrono = 0f;
        timer = timerBackup;
        inRadius.Clear();
    }

    public void Reset()
    {
        started = false;
        gameObject.SetActive(true);
        timer = timerBackup;
        UpdateTimerText();
    }

    #endregion

    #region CURSOR
    public void ShowCursorVisuals(bool show)
    {
        CursorVisuals.SetActive(show);
    }
    #endregion

    #region UPDATE
    private void Update()
    {
        if (!placed && !started)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            placable = BombTester.TestPos(SnapToGrid(mousePosition));
            
            if (placable)
                spriteRenderer.color = Color.white;
            else
                spriteRenderer.color = Color.red;

            transform.position = SnapToGrid(mousePosition);
        }
        if (!started)
            return;

        chrono += Time.deltaTime;
        if (chrono >= 1)
        {
            chrono = 0;
            timer--;
            UpdateTimerText();
            if (timer == 0)
                Explode();
        }
    }
    #endregion

    #region EXPLOSION
    private void Explode()
    {
        BoomObject b;
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(transform.position, ExplosionRadius, Vector2.zero, 0, BoomObjectMask))
        {
            hit.transform.TryGetComponent<BoomObject>(out b);
            if (b)
                inRadius.Add(b);
        }
        foreach (BoomObject boomObject in inRadius)
        {
            Vector2 dir = boomObject.transform.position - transform.position;
            boomObject.Boom(dir.normalized * ExplosionForce );
        }

        inRadius.Clear();
        GameObject vfx = Instantiate(ExplosionVFX);
        vfx.transform.position = transform.position;
        Destroy(vfx, .5f);

        gameObject.SetActive(false);
    }
    #endregion

    #region COUNTDOWN
    private void UpdateTimerText()
    {
        CountdownText.text = timer.ToString();
    }

    public void SetTimer(int newTimer)
    {
        timerBackup = newTimer;
        timer = newTimer;
        UpdateTimerText();
    }
    #endregion
}
