using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    private enum PlayerState
    {
        Idle,
        Move,
        Attack,
        Wait        // 当前回合无法行动
    }
    private PlayerState _curState;
    private PlayerState curState
    {
        get
        {
            return _curState;
        }
        set
        {
            _curState = value;

            switch (value)
            {
                case PlayerState.Idle:
                    curAnimator.SetBool("Run", false);
                    break;
                case PlayerState.Move:
                    curAnimator.SetBool("Run", true);
                    break;
                case PlayerState.Attack:
                    curAnimator.SetBool("Jab", true);
                    break;
                default:
                    break;
            }
        }
    }

    private GameObject selectedBlockGO;
    private Animator curAnimator;
    private List<Vector3> movingPath;           // 包含了正在移动的所有目标点
    private float moveSpeed = 1f;
    private Vector3 curNodePos;                 // 玩家所处当前节点的坐标
    private float toNextNodeTotalTime = 0;      // 从当前位置到下个目标点所需经历总时长
    private float toNextNodeElapsedTime = 0;    // 已经过的时长

    public Player(ACTOR_INFO basic_info)
    {
        name = basic_info.name;
        position = basic_info.position;
        rotation = basic_info.rotation;

        Init();
    }

    public void AddToScene(GameObject go)
    {
        sceneObject = go;

        // 添加到场景后, 进行初始化
        sceneObject.name = name;
        sceneObject.transform.position = position;
        sceneObject.transform.eulerAngles = rotation;
        curAnimator = sceneObject.GetComponent<Animator>();
    }

    public override void OnUpdate()
    {
        if (curState == PlayerState.Idle)
        {
            OnCommandInput();
        }
        else if (curState == PlayerState.Move)
        {
            OnMove();
        }
    }


    private void Init()
    {
        selectedBlockGO = GameObject.Find("selected_block_graphics");
        selectedBlockGO.GetComponent<Renderer>().enabled = false;
        
        curState = PlayerState.Wait;
    }
    
    private void OnCommandInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag.Equals("Ground"))
            {
                Vector3 hitPosition = hit.point;

                if (Input.GetMouseButtonDown(0))
                {
                    movingPath = Pathfinding.GridManager.Instance.GetPathFromStartToEnd(sceneObject.transform.position, hitPosition);
                    if (movingPath.Count > 0)
                    {
                        toNextNodeTotalTime = (movingPath[0] - sceneObject.transform.position).magnitude / moveSpeed;
                        curNodePos = sceneObject.transform.position;
                        curState = PlayerState.Move;
                    }
                }
                else
                {
                    OnGridSelect(hitPosition);
                }
            }
        }
    }
    
    private void OnMove()
    {
        if (movingPath == null || movingPath.Count <= 0)
        {
            curState = PlayerState.Idle;
        }
        else
        {
            toNextNodeElapsedTime += Time.deltaTime;

            if (toNextNodeElapsedTime >= toNextNodeTotalTime)
            {
                sceneObject.transform.position = movingPath[0];
                curNodePos = sceneObject.transform.position;
                movingPath.RemoveAt(0);
                toNextNodeElapsedTime = 0;

                if (movingPath.Count > 0)
                {
                    toNextNodeTotalTime = (movingPath[0] - sceneObject.transform.position).magnitude / moveSpeed;
                }
            }
            else
            {
                sceneObject.transform.position = curNodePos + (toNextNodeElapsedTime / toNextNodeTotalTime) * (movingPath[0] - curNodePos);
            }
        }
    }

    private void OnGridSelect(Vector3 groundPos)
    {
        selectedBlockGO.transform.position = Pathfinding.GridManager.Instance.GetGridCenterPos(groundPos);
        selectedBlockGO.GetComponent<Renderer>().enabled = true;
    }

    public override void OnTurnBegin()
    {
        curState = PlayerState.Idle;
    }

    public override void OnTurnEnd()
    {
        curState = PlayerState.Wait;
    }
}
