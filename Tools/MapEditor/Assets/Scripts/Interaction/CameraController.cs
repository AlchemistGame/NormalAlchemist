using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("视角旋转速度")]
    [Range(0f, 300)]
    public float rotateSpeed = 150f;
    [Tooltip("视角缩放速度")]
    [Range(0f, 120f)]
    public float zoomSpeed = 60f;
    [Tooltip("视角平移速度")]
    [Range(0f, 40f)]
    public float normalMoveSpeed = 20f;

    private Vector2 cameraRotation;

    // LateUpdate is called every frame, if the Behaviour is enabled
    private void LateUpdate()
    {
        // 按住鼠标右键旋转视角
        if (Input.GetMouseButton(1))
        {
            cameraRotation.x -= Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            cameraRotation.y += Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
        }

        transform.localRotation = Quaternion.AngleAxis(cameraRotation.x, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(cameraRotation.y, Vector3.left);

        // 鼠标滚轮放大, 缩小视角
        transform.position += transform.forward * zoomSpeed * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;

        // 按住鼠标左键移动视角
        if (Input.GetMouseButton(0))
        {
            transform.position -= transform.right * normalMoveSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
            transform.position -= transform.up * normalMoveSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
        }
    }
}
