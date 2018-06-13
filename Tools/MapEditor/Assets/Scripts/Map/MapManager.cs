using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject cubePrefab;

    private GameObject indicationObject;
    private Camera currentCamera;
    private enum EditorMode
    {
        Add,
        Remove,
    }
    private EditorMode currentMode = EditorMode.Add;
    private const float addDistance = 10.0f;

    private void Awake()
    {
        currentCamera = this.GetComponent<Camera>();
    }

    void Start()
    {
        indicationObject = Instantiate<GameObject>(cubePrefab);


    }

    void Update()
    {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        switch (currentMode)
        {
            case EditorMode.Add:
                Vector3 targetPos = ray.GetPoint(addDistance);
                Vector3 alignedPosition = new Vector3(Mathf.Floor(targetPos.x / MapData.gridEdge) * MapData.gridEdge + MapData.gridEdge / 2.0f,
                    Mathf.Floor(targetPos.y / MapData.gridEdge) * MapData.gridEdge + MapData.gridEdge / 2.0f,
                    Mathf.Floor(targetPos.z / MapData.gridEdge) * MapData.gridEdge + MapData.gridEdge / 2.0f);

                indicationObject.transform.position = alignedPosition;
                indicationObject.SetActive(true);
                break;
            case EditorMode.Remove:
                indicationObject.SetActive(false);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Transform selectTrans = hit.transform;

                    Debug.Log(selectTrans.name);
                }
                break;
            default:
                break;
        }

        // E键进入添加模式
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentMode = EditorMode.Add;
        }
        // Esc键进入删除模式
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currentMode = EditorMode.Remove;
        }
    }

    private void AddCubeToWorld()
    {

    }

}
