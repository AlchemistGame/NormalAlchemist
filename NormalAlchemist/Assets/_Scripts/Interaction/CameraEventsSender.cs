using UnityEngine;
using System.Collections;

// sends VoxelEvents such as OnLook, OnMouseDown, etc.
namespace VoxelFramework
{
    public class CameraEventsSender : MonoBehaviour
    {
        private GameObject SelectedBlockGraphics;

        public void Awake()
        {
            SelectedBlockGraphics = GameObject.Find("selected_block_graphics");
            SelectedBlockGraphics.GetComponent<Renderer>().enabled = false;
        }

        public void Update()
        {
            if (InputController.currentMode == MapEditorMode.Edit)
            {
                MouseCursorEvents();
            }
        }

        /// <summary>
        /// ÿ֡���м��, �Դ��������¼�
        /// </summary>
        private void MouseCursorEvents()
        {
            //Vector3 pos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10.0f);
            VoxelInfo raycast = Engine.VoxelRaycast(Camera.main.ScreenPointToRay(Input.mousePosition), 9999.9f, false);

            if (raycast != null)
            {
                // ���� block Ĭ�϶���û��ʵ������( һ�� chunk �ڵ����� blocks ͳһ���� Mesh), ���Դ˴���Ҫ����ʵ������������, Ȼ���ٴ��������¼�
                GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(raycast.GetVoxel())) as GameObject;

                // only execute this if the voxel actually has any events (either VoxelEvents component, or any component that inherits from it)
                if (voxelObject.GetComponent<VoxelEvents>() != null)
                {
                    voxelObject.GetComponent<VoxelEvents>().OnLook(raycast);

                    // for all mouse buttons, send events
                    for (int i = 0; i < 3; i++)
                    {
                        if (Input.GetMouseButtonDown(i))
                        {
                            voxelObject.GetComponent<VoxelEvents>().OnMouseDown(i, raycast);
                        }
                        if (Input.GetMouseButtonUp(i))
                        {
                            voxelObject.GetComponent<VoxelEvents>().OnMouseUp(i, raycast);
                        }
                        if (Input.GetMouseButton(i))
                        {
                            voxelObject.GetComponent<VoxelEvents>().OnMouseHold(i, raycast);
                        }
                    }
                }

                Destroy(voxelObject);
            }
            else
            {
                // disable selected block ui when no block is hit
                if (SelectedBlockGraphics != null)
                {
                    SelectedBlockGraphics.GetComponent<Renderer>().enabled = false;
                }
            }

        }
    }
}