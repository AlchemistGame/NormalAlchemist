using System.Collections;
using System.IO;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraType
    {
        IsometricPerspective,
        IsometricOrtho,
        TwodPerspective,
        TwodOrtho
    }

    private bool gF;
    private bool gB;
    private bool gR;
    private bool gL;
    private bool rL;
    private bool rR;
    private float mS;
    private float rS;
    public GameObject mainRootGO;
    public GameObject yAreaGO;
    public Camera cameraGO;
    [HideInInspector]
    public bool is2D;
    private bool isOrtho;
    [HideInInspector]
    public bool isInTopView;    // 俯视图
    private bool isMiddleButtonPressed;
    private Vector2 startDrag;

    private bool isLeftButtonPressed;
    private bool isAltPressed;
    private bool canDragAround;
    private Vector3 hitPoint;
    private bool isRightButtonPressed;
    [HideInInspector]
    public float cameraSensitivity = 1.0f;

    void Start()
    {
        isMiddleButtonPressed = false;
        hitPoint = Vector3.zero;
        isLeftButtonPressed = false;
        isAltPressed = false;
        isRightButtonPressed = false;
        canDragAround = false;
        gF = false;
        gB = false;
        gR = false;
        gL = false;
        rL = false;
        rR = false;
        mS = 0.3f;
        rS = 70.0f;
        isInTopView = false;

        SetCamera(CameraType.IsometricPerspective);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isLeftButtonPressed = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                hitPoint = hit.collider.gameObject.transform.position;
                canDragAround = true;
            }
            else
            {
                canDragAround = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isLeftButtonPressed = false;
            canDragAround = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isAltPressed = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            isAltPressed = false;
        }

        if (isLeftButtonPressed && canDragAround && isAltPressed)
        {
            transform.RotateAround(hitPoint, Vector3.up, Input.GetAxis("Mouse X") * 3.0f * cameraSensitivity);
            yAreaGO.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * 1.5f * cameraSensitivity);
        }

        if (Input.GetMouseButtonDown(2))
        {
            isMiddleButtonPressed = true;
            startDrag = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(2))
        {
            isMiddleButtonPressed = false;
        }

        if (isMiddleButtonPressed)
        {
            float pX = Input.GetAxis("Mouse X");
            float pY = Input.GetAxis("Mouse Y");

            transform.Translate(new Vector3(-pX * 0.7f * cameraSensitivity, 0, -pY * 0.7f * cameraSensitivity) * 1f);
        }

        if (Input.GetMouseButtonDown(1))
        {
            isRightButtonPressed = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRightButtonPressed = false;
        }

        if (isRightButtonPressed && isAltPressed)
        {
            transform.Translate(new Vector3(-Input.GetAxis("Mouse X") * 0.7f * cameraSensitivity, 0, -Input.GetAxis("Mouse Y") * 0.7f * cameraSensitivity) * 1f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            gL = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            gR = true;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            gF = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            gB = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            rL = true;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            rR = true;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            gL = false;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            gR = false;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            gF = false;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            gB = false;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            rL = false;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            rR = false;
        }

        float scrollY = Input.GetAxis("Mouse ScrollWheel");

        if (Input.mousePosition.x < 150 || Input.mousePosition.x < 0 || Input.mousePosition.y < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height)
        {
            scrollY = 0.0f;
        }

        if (scrollY >= 0.1f)
        {
            if (is2D || isInTopView)
            {
                StartCoroutine(MoveUpDown(false, false));
                StartCoroutine(MoveUpDown(false, false));
            }
            else
            {
                StartCoroutine(MoveUpDown(false, true));
                StartCoroutine(MoveUpDown(false, true));
            }
        }
        else if (scrollY <= -0.1f)
        {
            if (is2D || isInTopView)
            {
                StartCoroutine(MoveUpDown(true, false));
                StartCoroutine(MoveUpDown(true, false));
            }
            else
            {
                StartCoroutine(MoveUpDown(true, true));
                StartCoroutine(MoveUpDown(true, true));
            }
        }

        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.Underscore))
        {
            if (is2D || isInTopView)
            {
                StartCoroutine(MoveUpDown(true, false));
            }
            else
            {
                StartCoroutine(MoveUpDown(true, true));
            }
        }

        if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.Plus))
        {
            if (is2D || isInTopView)
            {
                StartCoroutine(MoveUpDown(false, false));
            }
            else
            {
                StartCoroutine(MoveUpDown(false, true));
            }
        }
    }

    void FixedUpdate()
    {
        if (gL)
        {
            this.transform.Translate(Vector3.left * mS * cameraSensitivity);
        }
        else if (gR)
        {
            this.transform.Translate(Vector3.right * mS * cameraSensitivity);
        }

        if (gF)
        {
            this.transform.Translate(Vector3.forward * mS * cameraSensitivity);
        }
        else if (gB)
        {
            this.transform.Translate(Vector3.back * mS * cameraSensitivity);
        }

        if (rL)
        {
            this.transform.RotateAround(cameraGO.gameObject.transform.position, Vector3.up, rS * cameraSensitivity * Time.deltaTime);
        }
        else if (rR)
        {
            this.transform.RotateAround(cameraGO.gameObject.transform.position, Vector3.up, -rS * cameraSensitivity * Time.deltaTime);
        }
    }

    public IEnumerator MoveUpDown(bool isUp, bool isNotGrid)
    {
        int counter = 0;
        int stopC = 3;

        if (!isNotGrid)
            stopC = 10;

        while (counter++ != stopC)
        {
            if (isUp)
            {
                gB = isNotGrid;

                if (!cameraGO.orthographic)
                    this.transform.position += new Vector3(0.0f, 0.1f * cameraSensitivity, 0.0f);
                else
                    cameraGO.orthographicSize += 0.1f * cameraSensitivity;
            }
            else
            {
                gF = isNotGrid;

                if (!cameraGO.orthographic)
                    this.transform.position -= new Vector3(0.0f, 0.1f * cameraSensitivity, 0.0f);
                else
                    cameraGO.orthographicSize -= 0.1f * cameraSensitivity;
            }

            yield return 0;
        }

        gB = false;
        gF = false;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    #region 外部接口
    private void LoadCameraInfo()
    {
        if (System.IO.File.Exists(GlobalSettings.MapLevelsDir + "BattleFiled.map"))
        {
            StreamReader sr = new StreamReader(GlobalSettings.MapLevelsDir + "BattleFiled.map");
            string info = sr.ReadToEnd();
            sr.Close();
            MapData myData = JsonUtility.FromJson<MapData>(info);

            if (myData.settings != "")
            {
                string[] allinfo = myData.settings.Split(":"[0]);
                // main pos
                float pX = System.Convert.ToSingle(allinfo[0]);
                float pY = System.Convert.ToSingle(allinfo[1]);
                float pZ = System.Convert.ToSingle(allinfo[2]);
                // main rot
                float _main_rX = System.Convert.ToSingle(allinfo[3]);
                float _main_rY = System.Convert.ToSingle(allinfo[4]);
                float _main_rZ = System.Convert.ToSingle(allinfo[5]);
                // yarea rot
                float _yarea_rX = System.Convert.ToSingle(allinfo[6]);
                float _yarea_rY = System.Convert.ToSingle(allinfo[7]);
                float _yarea_rZ = System.Convert.ToSingle(allinfo[8]);
                // mapeditorcamera rot
                float _mapeditorcamera_rX = System.Convert.ToSingle(allinfo[9]);
                float _mapeditorcamera_rY = System.Convert.ToSingle(allinfo[10]);
                float _mapeditorcamera_rZ = System.Convert.ToSingle(allinfo[11]);

                mainRootGO.transform.position = new Vector3(pX, pY, pZ);
                mainRootGO.transform.localEulerAngles = new Vector3(_main_rX, _main_rY, _main_rZ);
                yAreaGO.transform.localEulerAngles = new Vector3(_yarea_rX, _yarea_rY, _yarea_rZ);
                cameraGO.transform.localEulerAngles = new Vector3(_mapeditorcamera_rX, _mapeditorcamera_rY, _mapeditorcamera_rZ);
            }
        }
    }

    private void SetCamera(CameraType camType)
    {
        Camera camTemp = cameraGO.GetComponent<Camera>();
        switch (camType)
        {
            case CameraType.IsometricPerspective:
                camTemp.orthographic = false;
                camTemp.fieldOfView = 60;
                camTemp.farClipPlane = 1000.0f;
                isOrtho = false;
                is2D = false;
                break;
            case CameraType.IsometricOrtho:
                camTemp.orthographic = true;
                camTemp.orthographicSize = 5;
                camTemp.nearClipPlane = -100.0f;
                camTemp.farClipPlane = 1000.0f;
                is2D = false;
                isOrtho = true;
                break;
            case CameraType.TwodPerspective:
                camTemp.orthographic = false;
                camTemp.nearClipPlane = 0.1f;
                camTemp.farClipPlane = 1000.0f;
                isOrtho = false;
                is2D = true;
                break;
            case CameraType.TwodOrtho:
                camTemp.orthographic = true;
                camTemp.orthographicSize = 5;
                camTemp.nearClipPlane = -10.0f;
                camTemp.farClipPlane = 300.0f;
                isOrtho = true;
                is2D = true;
                break;
            default:
                break;
        }
    }
    #endregion
}
