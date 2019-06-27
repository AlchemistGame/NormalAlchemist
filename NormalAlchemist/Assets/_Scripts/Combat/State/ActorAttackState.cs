using UnityEngine;
using UnityEngine.EventSystems;

public class ActorAttackState : State
{
    public override void Enter()
    {
        base.Enter();

        ActorManager.Instance.OnUpdate += OnAttackInput;
    }

    public override void Exit()
    {
        base.Exit();

        ActorManager.Instance.OnUpdate -= OnAttackInput;
    }

    public void OnAttackInput()
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
            // 攻击的必须是敌人(友军无法攻击)
            if (!ActorManager.Instance.currentActor.gameObject.tag.Equals(hit.collider.tag))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ActorManager.Instance.currentActor.gameObject.transform.LookAt(hit.collider.transform);
                    ActorManager.Instance.currentActor.GetComponent<Animator>().SetBool("Jab", true);

                    ActorManager.Instance.ChangeState<CommandSelectionState>();
                }
            }
        }
    }
}
