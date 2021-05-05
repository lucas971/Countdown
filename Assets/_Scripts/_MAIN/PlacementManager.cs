using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    [SerializeField] private Bomb bombToPlace;
    [SerializeField] private int numberToPlace;
    [SerializeField] private LayerMask bombLayerMask;
    [SerializeField] private float mouseScrollCooldown = .1f;

    private List<Bomb> placedBombs;
    private Dictionary<BoomObject, Vector3> boomObjects;

    private Bomb currentBomb = null;
    private int currentTimer;
    private float mouseScrollTimer;
    private bool sequenceStarted;
    
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;
        placedBombs = new List<Bomb>();
        boomObjects = new Dictionary<BoomObject, Vector3>();
        currentTimer = 1;
        mouseScrollTimer = 0f;
    }

    private void Start()
    {
        sequenceStarted = false;
        foreach (BoomObject boomObject in FindObjectsOfType<BoomObject>())
        {
            boomObjects[boomObject] = boomObject.transform.position;
        }
        for (int i = 0; i < numberToPlace; i++)
            UIManager.Instance.LayoutAddBomb();
    }

    private void Update()
    {
        if (sequenceStarted)
            return;

        if (Input.mouseScrollDelta.y != 0 && currentBomb && Time.time > mouseScrollTimer)
            IncrementTimer((int)Input.mouseScrollDelta.y);

        else if (Input.GetMouseButtonDown(0) && currentBomb)
            PlaceBomb();
        else if (Input.GetMouseButtonDown(0))
            MoveBomb();
        
        else if (Input.GetMouseButtonDown(1) && !currentBomb)
            UnPlaceBomb();
        else if (Input.GetMouseButtonDown(1))
            UnSelectBomb();

    }
    public void SelectBomb()
    {
        if (sequenceStarted)
            return;
        currentBomb = Instantiate(bombToPlace);
        currentBomb.SetTimer(currentTimer);
        currentBomb.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void PlaceBomb()
    {
        currentBomb.Place();
        placedBombs.Add(currentBomb);
        currentBomb = null;
        UIManager.Instance.LayoutRemoveBomb();
    }

    private void UnPlaceBomb()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, bombLayerMask);
        if (hit)
        {
            Bomb b = hit.collider.gameObject.GetComponent<Bomb>();
            placedBombs.Remove(b);
            Destroy(b.gameObject);
            UIManager.Instance.LayoutAddBomb();
        }
    }

    private void MoveBomb()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, bombLayerMask);
        if (hit)
        {
            Bomb b = hit.collider.gameObject.GetComponent<Bomb>();
            placedBombs.Remove(b);
            Destroy(b.gameObject);
            UIManager.Instance.LayoutAddBomb();
            SelectBomb();
        }
    }

    private void UnSelectBomb()
    {
        placedBombs.Remove(currentBomb);
        Destroy(currentBomb.gameObject);
    }

    private void IncrementTimer(int value)
    {
        mouseScrollTimer = Time.time + mouseScrollCooldown;
        currentTimer += value;
        currentTimer = Mathf.Max(1, currentTimer);
        currentBomb.SetTimer(currentTimer);
    }

    public void InitiateSequence()
    {
        if (currentBomb)
            UnSelectBomb();

        sequenceStarted = true;
        foreach (Bomb b in placedBombs)
            b.InitiateSequence();

        UIManager.Instance.ShowRetryButton();
    }

    public void LoadPreviousPlacement()
    {
        foreach (var boomObject in boomObjects)
        {
            boomObject.Key.transform.position = boomObject.Value;
            boomObject.Key.transform.rotation = Quaternion.identity;
            boomObject.Key.Stop();
        }
        foreach (Bomb b in placedBombs)
        {
            b.Reset();
        }
        sequenceStarted = false;

        UIManager.Instance.ShowClearButton();
    }

}
