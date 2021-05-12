using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    #region PRIVATE VARIABLES
    //BOMBS
    private List<Bomb> bombs;
    private List<Bomb> placedBombs;
    private List<BoomObject> boomObjects;
    private LayerMask BombLayerMask;
    private Bomb currentBomb = null;
    private Vector2 gravityBackup;

    //STATES
    private bool sequenceStarted;
    #endregion

    #region INITIALIZATION
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;

        bombs = new List<Bomb>();
        Transform BombContainer = transform.Find("BOMBS");
        for (int i = 0; i < BombContainer.childCount; i++)
        {
            bombs.Add(BombContainer.GetChild(i).GetComponent<Bomb>());
            BombContainer.GetChild(i).gameObject.SetActive(false);
        }

        placedBombs = new List<Bomb>();

        boomObjects = new List<BoomObject>();
    }

    private void Start()
    {
        //INITIALIZE VARIABLES
        gravityBackup = Physics2D.gravity;
        Physics2D.gravity = Vector2.zero;
        BombLayerMask = LayerMask.GetMask("Bomb");
        sequenceStarted = false;

        //Fetch all boomobjects in the level
        foreach (BoomObject boomObject in FindObjectsOfType<BoomObject>())
        {
            boomObjects.Add(boomObject);
        }

        //Fill the UI horizontal layout with bombs icons.
        for (int i = 0; i < bombs.Count; i++)
            UIManager.Instance.LayoutAddBomb(i, bombs[i].Icon);

    }
    #endregion

    #region UPDATE
    private void Update()
    {
        if (sequenceStarted || CameraManager.Instance.OverrideInputs())
            return;

        //Check if a zoom is requested
        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
            CameraManager.Instance.RequestZoom(Input.mouseScrollDelta.y);

        //There was a right click
        else if (Input.GetMouseButtonDown(0) && currentBomb)
            PlaceBomb();
        else if (Input.GetMouseButtonDown(0))
            CheckClick();

        //There was a left click
        else if (Input.GetMouseButtonDown(1) && !currentBomb)
            RemoveBomb();
        else if (Input.GetMouseButtonDown(1))
            UnSelectBomb();
    }
    #endregion

    #region BOMB MANIPULATION
    public void SelectBomb(int which)
    {
        //Prevent from selecting a bomb while the sequence is running.
        if (sequenceStarted || currentBomb || CameraManager.Instance.OverrideInputs())
            return;

        //Create a bomb at the cursor's position.
        currentBomb = bombs[which];
        currentBomb.gameObject.SetActive(true);
        currentBomb.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void UnSelectBomb()
    {
        //Hide it
        currentBomb.gameObject.SetActive(false);

        //Reset its timer
        currentBomb.ResetTimer();

        //Ungrey it
        UIManager.Instance.LayoutUnGreyBomb(bombs.IndexOf(currentBomb));

        //Unselect it
        currentBomb = null;
    }

    private void PlaceBomb()
    {
        //Check if the bomb can be placed
        if (!currentBomb.Place())
            return;

        //Add it to the list of placed bombs
        placedBombs.Add(currentBomb);

        //Grey it
        UIManager.Instance.LayoutGreyBomb(bombs.IndexOf(currentBomb));

        //Unselect it
        currentBomb = null;
    }

    private void RemoveBomb()
    {
        //Check if there is a bomb at the cursor position
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, BombLayerMask);
        if (hit)
        {
            //Get the bomb component
            Bomb b = hit.collider.gameObject.GetComponent<Bomb>();

            //Remove the bomb from the placed bombs list
            placedBombs.Remove(b);

            //Unplace the bomb
            b.UnPlace();

            //Reset its timer
            b.ResetTimer();

            //UnGrey it
            UIManager.Instance.LayoutUnGreyBomb(bombs.IndexOf(b));

            //Destroy the bomb in the scene
            b.gameObject.SetActive(false);

        }
    }

    private void CheckClick()
    {
        //Check if there is a bomb at the cursor position
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, BombLayerMask);
        if (hit)
        {
            //Get the bomb component
            Bomb b;
            hit.collider.gameObject.TryGetComponent(out b);
            if (b) //If this is the bomb body, move the bomb
                MoveBomb(b);

            //Else get the timer component
            TimerButton timer;
            hit.collider.gameObject.TryGetComponent(out timer);
            if (timer) //If this is a timer, activate it
                UseTimer(timer);
        }
       
    }

    private void MoveBomb(Bomb b)
    {
        //Remove the bomb from the placed bombs list
        placedBombs.Remove(b);

        //Unplace the bomb
        b.UnPlace();

        //Set the bomb as the current held bomb
        currentBomb = b;
    }

    private void UseTimer(TimerButton timer)
    {
        timer.Use();
    }
    #endregion

    #region BOMB RADIUS CHECK
    public bool CheckBomb(Bomb b)
    {
        foreach (Bomb other in placedBombs)
        {
            float dist = Vector2.Distance(b.transform.position, other.transform.position);
            if (dist < b.ExplosionRadius + other.ExplosionRadius)
                return false;
        }
        return true;
    }
    #endregion

    #region EXPLOSION SEQUENCE
    public void InitiateSequence()
    {
        CameraManager.Instance.InitiateSequence();
        Physics2D.gravity = gravityBackup;
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
        Physics2D.gravity = Vector2.zero;
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

        CameraManager.Instance.StopSequence();
    }

    public void Clear()
    {
        //Stop the Bombs
        while (placedBombs.Count > 0)
        {
            //Select the first bomb in the list
            Bomb b = placedBombs[0];

            //Remove the bomb from the list
            placedBombs.Remove(b);

            //Unplace the bomb
            b.UnPlace();
            b.ResetTimer();

            //UnGrey it
            UIManager.Instance.LayoutUnGreyBomb(bombs.IndexOf(b));

            //Destroy the bomb in the scene
            b.gameObject.SetActive(false);
        }
    }
    #endregion

    #region STOP
    public void Stop()
    {
        Physics2D.gravity = gravityBackup;
    }
    #endregion
}
