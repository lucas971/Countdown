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
    [SerializeField] protected float ExplosionForce;
    [SerializeField] public float ExplosionRadius;
    [SerializeField] protected LayerMask BoomObjectMask;

    [Space]
    [Header("Vfxs")]
    [SerializeField] protected GameObject ExplosionVFX;

    [Space]
    [Header("Coutdown")]
    [SerializeField] protected TMPro.TextMeshPro CountdownText;

    [Space]
    [Header("Placement")]
    [SerializeField] protected float CursorDragSpeed;
    [SerializeField] protected GameObject CursorVisuals;
    [SerializeField] public Sprite Icon;
    #endregion

    #region COMPONENTS
    protected SpriteRenderer spriteRenderer;
    #endregion

    #region PROTECTED VARIABLES
    //Collider detection
    protected List<BoomObject> inRadius;
    protected Vector2 mousePosition;
    protected Color placeableColor;

    //Timer
    protected int timerBackup;
    protected int timer;
    protected float chrono;

    //State
    protected bool started = false;
    protected bool placed = false;
    protected bool placable = false;
    #endregion

    #region INITIALIZATION
    protected void Awake()
    {
        inRadius = new List<BoomObject>();
        spriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        placeableColor = spriteRenderer.color;
    }
    #endregion

    #region PLACEMENT

    protected Vector2 SnapToGrid(Vector2 posToSnap)
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

        if (timer == 0)
            Explode();
    }

    public void Reset()
    {

        started = false;
        gameObject.SetActive(true);
        timer = timerBackup;
        UpdateTimerText();
    }

    public void UnPlace()
    {
        placed = false;
    }

    #endregion

    #region CURSOR
    public void ShowCursorVisuals(bool show)
    {
        CursorVisuals.SetActive(show);
    }
    #endregion

    #region UPDATE
    protected void Update()
    {
        if (!placed && !started)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            placable = PlacementManager.Instance.CheckBomb(this) && BombTester.TestPos(SnapToGrid(mousePosition), gameObject);

            if (placable)
                spriteRenderer.color = placeableColor;
            else
                spriteRenderer.color = placeableColor + new Color(0, 0, 0, -.8f);

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
    protected virtual void Explode()
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
    protected void UpdateTimerText()
    {
        CountdownText.text = timer.ToString();
    }

    public void ResetTimer()
    {
        timerBackup = timer = 0;
        UpdateTimerText();
    }

    public bool AllowTimerChange()
    {
        return placed;
    }

    public void AddTimer()
    {
        timerBackup++;
        timer++;
        UpdateTimerText();
    }

    public void ReduceTimer()
    {
        if (timer == 0)
            return;
        timerBackup--;
        timer--;
        UpdateTimerText();
    }
    #endregion
}
