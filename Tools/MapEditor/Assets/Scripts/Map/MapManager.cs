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
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            switch (currentMode)
            {
                case EditorMode.Add:
                    Vector3 targetPos = hit.point;
                    int gridX = Mathf.FloorToInt(targetPos.x / MapData.gridEdge) + 1;
                    int gridY = Mathf.FloorToInt(targetPos.y / MapData.gridEdge) + 1;
                    int gridZ = Mathf.FloorToInt(targetPos.z / MapData.gridEdge) + 1;

                    Vector3 alignedPosition = new Vector3(gridX * MapData.gridEdge,
                         gridY * MapData.gridEdge,
                         gridZ * MapData.gridEdge);

                    indicationObject.transform.position = alignedPosition;
                    indicationObject.SetActive(true);
                    break;
                case EditorMode.Remove:
                    indicationObject.SetActive(false);


                    break;
                default:
                    break;
            }
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
