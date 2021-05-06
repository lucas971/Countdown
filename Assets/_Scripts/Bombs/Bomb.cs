using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    #region CONSTS
    private const float gridSize = .5f;
    #endregion
    #region EDITOR FIELDS
    [Header("Explosion")]
    [SerializeField] private float ExplosionForce;
    [SerializeField] private Collider2D ExplosionCollider;

    [Space]
    [Header("Coutdown")]
    [SerializeField] private TMPro.TextMeshPro CountdownText;

    [Space]
    [Header("Placement")]
    [SerializeField] private float CursorDragSpeed;
    [SerializeField] private Collider2D BodyCollider;
    #endregion

    #region COMPONENTS
    private SpriteRenderer spriteRenderer;
    #endregion

    #region PRIVATE VARIABLES
    //Collider detection
    private List<BoomObject> inRadius;
    private int inCollision;

    //Timer
    private int timerBackup;
    private int timer;
    private float chrono;

    //State
    private bool started = false;
    private bool placed = false;
    private bool colliderBlock = false;
    private int inBombMask = 0;
    #endregion

    #region INITIALIZATION
    private void Awake()
    {
        inRadius = new List<BoomObject>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        inCollision = 0;
        chrono = 0f;
        UpdateTimerText();
    }
    #endregion

    #region TRIGGER
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (started)
        {
            BoomObject boomObjectTry;
            if (collision.transform.gameObject.TryGetComponent<BoomObject>(out boomObjectTry))
            {
                if (!inRadius.Contains(boomObjectTry))
                    inRadius.Add(boomObjectTry);
            }
        }

        else
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("BombMask"))
            {
                inBombMask++;
                return;
            }
            colliderBlock = true;
            inCollision++;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (started)
        {
            BoomObject boomObjectTry;
            if (collision.transform.gameObject.TryGetComponent<BoomObject>(out boomObjectTry))
            {
                if (inRadius.Contains(boomObjectTry))
                    inRadius.Remove(boomObjectTry);
            }
        }
        else
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("BombMask"))
            {
                inBombMask--;
                return;
            }
            inCollision--;
            if (inCollision == 0)
            {
                colliderBlock = false;
            }
        }
    }
    #endregion

    #region PLACEMENT

    private void SnapToGrid(Vector2 posToSnap)
    {
        float snappedX;
        float snappedY;

        int xDiv = (int)(posToSnap.x / gridSize);
        int yDiv = (int)(posToSnap.y / gridSize);

        snappedX = xDiv * gridSize;
        snappedY = yDiv * gridSize;
        transform.position = new Vector3(snappedX, snappedY, 0);
    }
    public bool Place()
    {
        if (!colliderBlock && inBombMask>0)
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
        BodyCollider.enabled = false;
        ExplosionCollider.enabled = true;
    }

    public void Reset()
    {
        BodyCollider.enabled = true;
        ExplosionCollider.enabled = false;
        started = false;
        gameObject.SetActive(true);
        timer = timerBackup;
        UpdateTimerText();

    }

    #endregion

    #region UPDATE
    private void Update()
    {
        if (!placed && !started && !colliderBlock && inBombMask > 0)
        {
            spriteRenderer.color = Color.white;
        }
        else if (colliderBlock || inBombMask <= 0)
        {
            spriteRenderer.color = Color.red;
        }
        if (!placed)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //transform.position = Vector2.Lerp(transform.position, mousePosition + Vector3.down, CursorDragSpeed);
            SnapToGrid(mousePosition);
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
        foreach (BoomObject boomObject in inRadius)
        {
            Vector2 dir = boomObject.transform.position - transform.position;
            boomObject.Boom(dir.normalized * ExplosionForce );
        }
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
