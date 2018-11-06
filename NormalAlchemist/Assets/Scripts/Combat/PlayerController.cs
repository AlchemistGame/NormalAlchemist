using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject selectedBlockGO;

    private enum PlayerState
    {
        Idle,
        Move,
        Attack
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

    private Animator curAnimator;
    private List<Vector3> movingPath;           // 包含了正在移动的所有目标点
    private float moveSpeed = 1f;
    private Vector3 curNodePos;                 // 玩家所处当前节点的坐标
    private float toNextNodeTotalTime = 0;      // 从当前位置到下个目标点所需经历总时长
    private float toNextNodeElapsedTime = 0;    // 已经过的时长

    private void Awake()
    {
        curAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        curState = PlayerState.Idle;
        selectedBlockGO.GetComponent<Renderer>().enabled = false;
    }

    private void Update()
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
                    movingPath = Pathfinding.GridManager.Instance.GetPathFromStartToEnd(transform.position, hitPosition);
                    if (movingPath.Count > 0)
                    {
                        toNextNodeTotalTime = (movingPath[0] - transform.position).magnitude / moveSpeed;
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
                transform.position = movingPath[0];
                curNodePos = transform.position;
                movingPath.RemoveAt(0);
                toNextNodeElapsedTime = 0;

                if (movingPath.Count > 0)
                {
                    toNextNodeTotalTime = (movingPath[0] - transform.position).magnitude / moveSpeed;
                }
            }
            else
            {
                transform.position = curNodePos + (toNextNodeElapsedTime / toNextNodeTotalTime) * (movingPath[0] - curNodePos);
            }
        }
    }

    private void OnGridSelect(Vector3 groundPos)
    {
        selectedBlockGO.transform.position = Pathfinding.GridManager.Instance.GetGridCenterPos(groundPos);
        selectedBlockGO.GetComponent<Renderer>().enabled = true;
    }
}
