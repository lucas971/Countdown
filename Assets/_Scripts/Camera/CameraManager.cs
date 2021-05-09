using System.Collections;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    protected const int _MinCameraSize = 8;
    protected const float _ZoomSpeed = 10;
    protected const float _ZoomValue = 3;
    protected const float _PanningSpeed = 10;

    #region EDITOR FIELDS
    [Header("Level Parameters")]
    [SerializeField] protected bool CanPan;
    [SerializeField] protected bool CanScale;
    [SerializeField] protected Rect Boundaries;
    #endregion

    #region PRIVATE PARAMETERS
    //STATES
    private bool IsPanning = false;
    private bool IsZooming = false;
    private bool LockZoomUp = false;

    //MOUSE
    private Vector2 lastPos;
    private Vector2 mouseMovement;

    //CAMERA BOUNDS
    private float cameraMinX;
    private float cameraMinY;
    private float cameraMaxX;
    private float cameraMaxY;
    #endregion

    #region INITIALIZATION
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);

        Instance = this;
    }
    #endregion

    #region PUBLIC METHODS
    public bool AllowScale()
    {
        return CanScale;
    }

    public bool AllowPan()
    {
        return CanPan;
    }

    public void StartPanning()
    {
        IsPanning = true;
        lastPos = Input.mousePosition;
    }

    public void RequestZoom(float value)
    {
        if (IsZooming)
            return;
        if (value > 0 && Camera.main.orthographicSize == _MinCameraSize)
            return;
        if (value < 0 && LockZoomUp)
            return;

        LockZoomUp = false;
        IsZooming = true;
        StartCoroutine(Zoom(value < 0));
    }
    #endregion

    #region UPDATE
    private void Update()
    {
        if (IsPanning)
        {
            PanningUpdate();
        }
    }
    #endregion

    #region PANNING
    private void PanningUpdate()
    {
        //Get the mouse delta
        mouseMovement = lastPos - (Vector2)Input.mousePosition;
        lastPos = Input.mousePosition;

        //If there is a movement
        if (mouseMovement.magnitude > 0)
        {
            Camera.main.transform.position += (Vector3)mouseMovement * Time.deltaTime * _PanningSpeed;
            CorrectCameraBounds();
        }


        //Stop the panning sequence
        if (Input.GetMouseButtonUp(0))
            IsPanning = false;
    }
    #endregion

    #region ZOOM
    protected IEnumerator Zoom(bool unzoom)
    {
        //Adapt speed and target zoom to direction
        float value = _ZoomValue;
        float speed = _ZoomSpeed;
        if (!unzoom)
        {
            value *= -1;
            speed *= -1;
        }
        float TargetSize = Mathf.Max(_MinCameraSize, Camera.main.orthographicSize + value);


        bool done = false;
        while (!done)
        {
            //Change the zoom
            Camera.main.orthographicSize += speed * Time.deltaTime;
            
            //If we unzoomed, correct camera bounds. If it returns true, it means we cover either all the height or
            //width of the scene and must cancel the zoom.
            if (unzoom && CorrectCameraBounds())
            {
                //Cancel the zoom
                Camera.main.orthographicSize -= speed * Time.deltaTime;
                //Prevent from unzooming
                LockZoomUp = true;
                break;
            }

            //Check if we're done
            if ((Camera.main.orthographicSize > TargetSize && value > 0) || 
                (Camera.main.orthographicSize < TargetSize && value < 0))
            {
                Camera.main.orthographicSize = TargetSize;
                done = true;
            }

            yield return null;
        }

        IsZooming = false;
    }
    #endregion

    #region BOUNDS CORRECTION
    private bool CorrectCameraBounds()
    {
        ComputeCameraRect();
        bool CancelZooming = false; //This is used to tell the zoom routine if it must stop.


        //Horizontal tests
        if (cameraMaxX > Boundaries.xMax)
        {
            Camera.main.transform.position += Vector3.left * (cameraMaxX - Boundaries.xMax);
        }
        if (cameraMinX < Boundaries.xMin)
        {
            Camera.main.transform.position += Vector3.right * (Boundaries.xMin - cameraMinX);
            //Check if the zoom level is too big for the scene
            if (cameraMaxX > Boundaries.xMax)
            {
                CancelZooming = true;
            }
        }

        //Vertical test
        if (cameraMaxY > Boundaries.yMax)
        {
            Camera.main.transform.position += Vector3.down * (cameraMaxY - Boundaries.yMax);
        }
        if (cameraMinY < Boundaries.yMin)
        {
            Camera.main.transform.position += Vector3.up * (Boundaries.yMin - cameraMinY);
            //Check if the zoom level is too big for the scene
            if (cameraMaxX > Boundaries.xMax)
            {
                CancelZooming = true;
            }
        }

        return CancelZooming;
    }

    private void ComputeCameraRect()
    {
        float xSize = Camera.main.aspect * Camera.main.orthographicSize;
        float ySize = Camera.main.orthographicSize;

        cameraMinX = Camera.main.transform.position.x - xSize;
        cameraMinY = Camera.main.transform.position.y - ySize;
        cameraMaxX = Camera.main.transform.position.x + xSize;
        cameraMaxY = Camera.main.transform.position.y + ySize;
    }
    #endregion

    #region GIZMOS
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(
            new Vector2(Boundaries.xMin, Boundaries.yMin),
            new Vector2(Boundaries.xMin, Boundaries.yMax)
            );

        Gizmos.DrawLine(
            new Vector2(Boundaries.xMin, Boundaries.yMax),
            new Vector2(Boundaries.xMax, Boundaries.yMax)
            );

        Gizmos.DrawLine(
            new Vector2(Boundaries.xMax, Boundaries.yMax),
            new Vector2(Boundaries.xMax, Boundaries.yMin)
            );

        Gizmos.DrawLine(
            new Vector2(Boundaries.xMax, Boundaries.yMin),
            new Vector2(Boundaries.xMin, Boundaries.yMin)
            );
    }
    #endregion
}