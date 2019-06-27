using UnityEngine;
using UnityEngine.EventSystems;

public class ActorIdleState : State
{
    public override void Enter()
    {
        base.Enter();

        ActorManager.Instance.OnUpdate += OnIdleInput;

        ActorManager.Instance.selectedBlockGO.GetComponent<Renderer>().enabled = true;
    }

    public override void Exit()
    {
        base.Exit();

        ActorManager.Instance.OnUpdate -= OnIdleInput;
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
                    ActorManager.Instance.targetPosition = hitPosition;

                    ActorManager.Instance.ChangeState<ActorMoveState>();
                }
                else
                {
                    OnGridSelect(hitPosition);
                }
            }
        }
    }

    private void OnGridSelect(Vector3 groundPos)
    {
        ActorManager.Instance.selectedBlockGO.transform.position = Pathfinding.GridManager.Instance.GetGridCenterPos(groundPos);
    }
}
