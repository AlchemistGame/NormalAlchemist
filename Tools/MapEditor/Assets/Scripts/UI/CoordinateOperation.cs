using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoordinateOperation : MonoBehaviour {

    public Camera sceneCamera;
    public Button xBtn;
    public Button yBtn;
    public Button zBtn;
    public Button originalBtn;

    public float cameraMoveSpeed = 0.02f;
    public float cameraScaleSpeed = 0.2f;
    public float cameraRotateSpeed = 8f;

    private const float m_defaultDistance = 1.0f;
    private const float m_minDistance = 0.1f;
    private const float m_maxDistance = 200f;
    private float m_realDistance = m_defaultDistance;
    private float xRotateOffset = 0.0f;
    private float yRotateOffset = 0.0f;
    private Transform focusTrans;   // 当前聚焦物体

    void Start ()
    {
        // Camera 指向 X 轴正方向
        xBtn.onClick.AddListener(() =>
        {
            RotateAroundCenterPoint(Quaternion.Euler(0, -90, 0));
        });

        yBtn.onClick.AddListener(() =>
        {
            RotateAroundCenterPoint(Quaternion.Euler(90, 0, 0));
        });

        zBtn.onClick.AddListener(() =>
        {
            RotateAroundCenterPoint(Quaternion.Euler(0, 0, 0));
        });

        // 返回原点并使 Camera 朝向 Z 轴正方向
        originalBtn.onClick.AddListener(() =>
        {
            sceneCamera.transform.position = new Vector3(0, 0, 0);
            sceneCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
        });

        // 初始化
        xRotateOffset = sceneCamera.transform.eulerAngles.y;
        yRotateOffset = sceneCamera.transform.eulerAngles.x;
    }
	
	void Update ()
    {
        // 左键单击选中物体
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(sceneCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                focusTrans = hit.transform;
                Debug.Log("选中了物体: " + hit.transform.name);
            }
        }

        // 鼠标滚轮放大, 缩小视角
        float scrollWheelOffset = Input.GetAxis("Mouse ScrollWheel") * cameraScaleSpeed;
        sceneCamera.transform.Translate(new Vector3(0, 0, scrollWheelOffset));

        m_realDistance = Mathf.Clamp(m_realDistance - scrollWheelOffset, m_minDistance, m_maxDistance);

        // 按住鼠标左键移动视角
        if (Input.GetMouseButton(0))
        {
            float xOffset = Input.GetAxis("Mouse X") * cameraMoveSpeed;
            float yOffset = Input.GetAxis("Mouse Y") * cameraMoveSpeed;
            sceneCamera.transform.Translate(Vector3.left * xOffset);
            sceneCamera.transform.Translate(Vector3.down * yOffset);
        }

        // 按住鼠标右键旋转视角
        if (Input.GetMouseButton(1))
        {
            xRotateOffset += Input.GetAxis("Mouse X") * cameraRotateSpeed;
            yRotateOffset -= Input.GetAxis("Mouse Y") * cameraRotateSpeed;
            ClampAngle(ref xRotateOffset);
            ClampAngle(ref yRotateOffset);

            // 欧拉角旋转: 物体分别围绕 local 坐标系的 X, Y, Z 轴进行旋转
            // 旋转的正方向用左手法则判断( 大拇指指向旋转轴, 四指弯曲方向为正 )
            Quaternion cameraRotation = Quaternion.Euler(yRotateOffset, xRotateOffset, 0);
            RotateAroundCenterPoint(cameraRotation);
        }

        // Esc键退出选中模式
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            focusTrans = null;
        }
	}

    private void ClampAngle(ref float angle)
    {
        if (angle > 360f)
        {
            angle -= 360f;
        }
        else if (angle < -360f)
        {
            angle += 360f;
        }
    }

    /// <summary>
    /// 围绕当前视角中心旋转多少度
    /// </summary>
    /// <param name="rotation"></param>
    private void RotateAroundCenterPoint(Quaternion rotation)
    {
        Vector3 centerPoint;

        // 两种模式: 选中物体时, 围绕物体进行旋转
        if (focusTrans != null)
        {
            centerPoint = focusTrans.position;
        }
        // 未选中物体时, 围绕视角中心进行旋转
        else
        {
            centerPoint = sceneCamera.transform.position + sceneCamera.transform.forward * m_realDistance;
        }

        float centerToCameraDistance = (sceneCamera.transform.position - centerPoint).magnitude;
        Vector3 centerToCameraVector = new Vector3(0, 0, -centerToCameraDistance);

        sceneCamera.transform.rotation = rotation;
        sceneCamera.transform.position = centerPoint + rotation * centerToCameraVector;
    }
}
