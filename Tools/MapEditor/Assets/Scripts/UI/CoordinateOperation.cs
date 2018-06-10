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
    public float cameraRotateSpeed = 0.8f;

	/// <summary>
    /// Rotation 的 X, Y, Z 值表示的是: 物体围绕 local 坐标系的 X, Y, Z 轴进行旋转
    /// 正负方向用左手法则判断( 大拇指指向旋转轴, 四指弯曲方向为正 )
    /// </summary>
	void Start ()
    {
        // Camera 指向 X 轴正方向
        xBtn.onClick.AddListener(() =>
        {
            var xRotation = Quaternion.Euler(0, -90, 0);
            sceneCamera.transform.rotation = xRotation;
        });

        yBtn.onClick.AddListener(() =>
        {
            var yRotation = Quaternion.Euler(90, 0, 0);
            sceneCamera.transform.rotation = yRotation;
        });

        zBtn.onClick.AddListener(() =>
        {
            var zRotation = Quaternion.Euler(0, 0, 0);
            sceneCamera.transform.rotation = zRotation;
        });

        // 返回原点并使 Camera 朝向 Z 轴正方向
        originalBtn.onClick.AddListener(() =>
        {
            sceneCamera.transform.position = new Vector3(0, 0, 0);

            var zRotation = Quaternion.Euler(0, 0, 0);
            sceneCamera.transform.rotation = zRotation;
        });
    }
	
	// Update is called once per frame
	void Update ()
    {
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
            float xOffset = Input.GetAxis("Mouse Y") * cameraRotateSpeed;
            float yOffset = -Input.GetAxis("Mouse X") * cameraRotateSpeed;
            sceneCamera.transform.Rotate(xOffset, yOffset, 0, Space.Self);
        }

        // 鼠标滚轮放大, 缩小视角
        float scrollWheelOffset = Input.GetAxis("Mouse ScrollWheel");
        sceneCamera.transform.Translate(new Vector3(0, 0, scrollWheelOffset * cameraScaleSpeed));
	}
}
