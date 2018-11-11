using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Actor
{
    private enum PlayerState
    {
        Idle,
        Move,
        Attack,
        Wait        // 当前无法行动
    }
    private PlayerState _curState;
    private PlayerState CurState
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
                    selectedBlockGO.GetComponent<Renderer>().enabled = false;
                    break;
                default:
                    break;
            }
        }
    }

    private GameObject selectedBlockGO;
    private Animator curAnimator;
    private List<Vector3> movingPath;           // 包含了正在移动的所有目标点
    private float moveSpeed = 3f;
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
        switch (CurState)
        {
            case PlayerState.Idle:
                OnIdleInput();
                break;
            case PlayerState.Move:
                OnMove();
                break;
            case PlayerState.Attack:
                OnAttackInput();
                break;
            case PlayerState.Wait:
                break;
            default:
                break;
        }
    }

    public void OnAttackBegin()
    {
        if (CurState == PlayerState.Wait)
        {
            return;
        }

        CurState = PlayerState.Attack;
    }

    public void OnAttackOver()
    {
        if (CurState == PlayerState.Wait)
        {
            return;
        }

        CurState = PlayerState.Idle;
    }

    public override void OnMouseEnter()
    {
        if (CurState == PlayerState.Wait)
        {
            ChangeModelOutlineColor(Color.red);
        }
        else
        {
            ChangeModelOutlineColor(Color.green);
        }
    }

    public override void OnMouseExit()
    {
        ChangeModelOutlineColor(Color.black);
    }

    public override void OnMouseClick()
    {
        ChangeModelOutlineColor(Color.black);
    }


    private void Init()
    {
        Event.Register(EventsEnum.StartPlayerAttack, this, "OnAttackBegin");
        Event.Register(EventsEnum.FinishPlayerAttack, this, "OnAttackOver");

        selectedBlockGO = GameObject.Find("selected_block_graphics");
        selectedBlockGO.GetComponent<Renderer>().enabled = false;

        CurState = PlayerState.Wait;
    }

    private void OnIdleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // 鼠标正在操作 UI
            return;
        }

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
                        sceneObject.transform.LookAt(movingPath[0]);
                        CurState = PlayerState.Move;
                        Event.Broadcast(EventsEnum.StartPlayerMove);
                    }
                }
                else
                {
                    OnGridSelect(hitPosition);
                }
            }
        }
    }

    private void OnAttackInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // 鼠标正在操作 UI
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
        {
            if (!sceneObject.tag.Equals(hit.collider.tag))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    sceneObject.transform.LookAt(hit.collider.transform);
                    curAnimator.SetBool("Jab", true);
                    Event.Broadcast(EventsEnum.FinishPlayerAttack);
                }
            }
        }
    }

    private void ChangeModelOutlineColor(Color color)
    {
        Renderer[] renderers = sceneObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (material.shader.name.Equals("Custom/Outline"))
                {
                    material.SetColor("_OutlineColor", color);
                }
            }
        }
    }

    private void OnMove()
    {
        if (movingPath == null || movingPath.Count <= 0)
        {
            CurState = PlayerState.Idle;
            Event.Broadcast(EventsEnum.FinishPlayerMove);
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
                    sceneObject.transform.LookAt(movingPath[0]);
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
        CurState = PlayerState.Idle;
    }

    public override void OnTurnEnd()
    {
        CurState = PlayerState.Wait;
    }
}
