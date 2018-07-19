using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapEditorMode
{
    Edit,   // 编辑地图模式
    CameraMove, // 控制摄像机模式
}

public class InputController : MonoBehaviour
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

    public Text modeNameText;

    public static MapEditorMode currentMode = MapEditorMode.CameraMove;

    private Vector2 cameraRotation = new Vector2(0, -45);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentMode = MapEditorMode.Edit;
            modeNameText.text = "地图编辑模式";
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currentMode = MapEditorMode.CameraMove;
            modeNameText.text = "控制视角模式";
        }
    }

    // LateUpdate is called every frame, if the Behaviour is enabled
    private void LateUpdate()
    {
        if (currentMode == MapEditorMode.CameraMove)
        {
            HandleCameraMove();
        }
    }

    private void HandleCameraMove()
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
