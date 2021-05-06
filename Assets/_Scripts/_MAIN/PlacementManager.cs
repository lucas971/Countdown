using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    #region EDITOR FIELDS
    [Header("Bombs")]
    [SerializeField] private Bomb BombPrefab;
    [SerializeField] private int BombsToPlace;
    [SerializeField] private LayerMask BombLayerMask;

    [Space]
    [Header("Input")]
    [SerializeField] private float MouseScrollCooldown = .1f;
    #endregion

    //BOMBS
    private List<Bomb> placedBombs;
    private List<BoomObject> boomObjects;
    private Bomb currentBomb = null;
    private int currentTimer;
    private float mouseScrollTimer;

    //STATES
    private bool sequenceStarted;

    #region INITIALIZATION
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;
        placedBombs = new List<Bomb>();
        boomObjects = new List<BoomObject>();
    }

    private void Start()
    {
        //INITIALIZE VARIABLES
        currentTimer = 1;
        mouseScrollTimer = 0f;
        sequenceStarted = false;

        //Fetch all boomobjects in the level
        foreach (BoomObject boomObject in FindObjectsOfType<BoomObject>())
        {
            boomObjects.Add(boomObject);
        }

        //Fill the UI horizontal layout with bombs icons.
        for (int i = 0; i < BombsToPlace; i++)
            UIManager.Instance.LayoutAddBomb();
    }
    #endregion

    #region UPDATE
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
    #endregion

    #region BOMB MANIPULATION
    public void SelectBomb()
    {
        //Prevent from selecting a bomb while the sequence is running.
        if (sequenceStarted)
            return;

        //Create a bomb at the cursor's position.
        currentBomb = Instantiate(BombPrefab);
        currentBomb.SetTimer(currentTimer);
        currentBomb.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void UnSelectBomb()
    {
        placedBombs.Remove(currentBomb);
        Destroy(currentBomb.gameObject);
    }

    private void PlaceBomb()
    {
        //Check if the bomb can be placed
        if (!currentBomb.Place())
            return;

        //Add it to the list of placed bombs
        placedBombs.Add(currentBomb);

        //Unselect it
        currentBomb = null;

        //Remove it from the UI Layout
        UIManager.Instance.LayoutRemoveBomb();
    }

    private void UnPlaceBomb()
    {
        //Check if there is a bomb at the cursor position
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, BombLayerMask);
        if (hit)
        {
            //Get the bomb component
            Bomb b = hit.collider.gameObject.GetComponent<Bomb>();

            //Remove the bomb from the placed bombs list
            placedBombs.Remove(b);

            //Destroy the bomb in the scene
            Destroy(b.gameObject);

            //Add a new bomb to th UI Layout
            UIManager.Instance.LayoutAddBomb();
        }
    }

    private void MoveBomb()
    {
        //Check if there is a bomb at the cursor position
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, BombLayerMask);
        if (hit)
        {
            //Get the bomb component
            Bomb b = hit.collider.gameObject.GetComponent<Bomb>();

            //Remove the bomb from the placed bombs list
            placedBombs.Remove(b);

            //Destroy the bomb in the scene
            Destroy(b.gameObject);

            //Add a new bomb to th UI Layout
            UIManager.Instance.LayoutAddBomb();

            //Spawn a new bomb at the previous bomb location
            SelectBomb();
        }
    }
    

    private void IncrementTimer(int value) //TODO : CHANGE WITH BUTTONS ON THE BOMB
    {
        mouseScrollTimer = Time.time + MouseScrollCooldown;
        currentTimer += value;
        currentTimer = Mathf.Max(1, currentTimer);
        currentBomb.SetTimer(currentTimer);
    }
    #endregion

    #region EXPLOSION SEQUENCE
    public void InitiateSequence()
    {
        //If a bomb is held while pressing the detonation button, unselect it.
        if (currentBomb)
            UnSelectBomb();

        //Switch to the started state
        sequenceStarted = true;

        //Initiate Bomb sequences
        foreach (Bomb b in placedBombs)
            b.InitiateSequence();
        
        //Initiate Boom Object sequences
        foreach (BoomObject b in boomObjects)
            b.InitiateSequence();

        //Replace the CLEAR button with the RETRY button
        UIManager.Instance.ShowRetryButton();
    }
    #endregion

    #region LOAD PREVIOUS SETUP
    public void LoadPreviousPlacement()
    {
        //Stop the started state
        sequenceStarted = false;

        //Stop the Boom Objects
        foreach (var boomObject in boomObjects)
        {
            boomObject.Stop();
        }

        //Stop the Bombs
        foreach (Bomb b in placedBombs)
        {
            b.Reset();
        }

        //Replace the RETRY button with the CLEAR button
        UIManager.Instance.ShowClearButton();
    }
    #endregion
}
