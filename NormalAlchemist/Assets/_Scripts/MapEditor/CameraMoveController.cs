using System.Collections;
using UnityEngine;

public class CameraMoveController : MonoBehaviour
{
    private bool gF;
    private bool gB;
    private bool gR;
    private bool gL;
    private bool rL;
    private bool rR;
    private float mS;
    private float rS;
    private Camera cam;
    [HideInInspector]
    public bool is2D;
    [HideInInspector]
    public Vector3 sel;
    [HideInInspector]
    public bool isInTopView;
    private bool isMiddleButtonPressed;
    private Vector2 startDrag;
    private GameObject yArea;
    private bool isLeftButtonPressed;
    private bool isAltPressed;
    private bool canDragAround;
    private Vector3 hitPoint;
    private bool isRightButtonPressed;
    private bool isIOSdetected;
    [HideInInspector]
    public float cameraSensitivity = 1.0f;

    void Start()
    {
        yArea = GameObject.Find("MAIN/YArea");
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
        sel = new Vector3(500.0f, 0.0f, 500.0f);
        isInTopView = false;

        cam = GameObject.Find("MapEditorCamera").GetComponent<Camera>();
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
            yArea.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * 1.5f * cameraSensitivity);
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
            this.transform.RotateAround(cam.gameObject.transform.position, Vector3.up, rS * cameraSensitivity * Time.deltaTime);
        }
        else if (rR)
        {
            this.transform.RotateAround(cam.gameObject.transform.position, Vector3.up, -rS * cameraSensitivity * Time.deltaTime);
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

                if (!cam.orthographic)
                    this.transform.position += new Vector3(0.0f, 0.1f * cameraSensitivity, 0.0f);
                else
                    cam.orthographicSize += 0.1f * cameraSensitivity;
            }
            else
            {
                gF = isNotGrid;

                if (!cam.orthographic)
                    this.transform.position -= new Vector3(0.0f, 0.1f * cameraSensitivity, 0.0f);
                else
                    cam.orthographicSize -= 0.1f * cameraSensitivity;
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
}
