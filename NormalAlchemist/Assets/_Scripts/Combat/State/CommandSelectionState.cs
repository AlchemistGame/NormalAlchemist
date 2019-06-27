using UnityEngine;

// 玩家选择指令
public class CommandSelectionState : State
{
    public override void Enter()
    {
        base.Enter();

        ActorManager.Instance.moveBtn.onClick.AddListener(OnMoveCommandSelection);
        ActorManager.Instance.attackBtn.onClick.AddListener(OnAttackCommandSelection);
        ActorManager.Instance.finishBtn.onClick.AddListener(OnFinishTurnCommandSelection);

        ActorManager.Instance.currentActor.GetComponent<Animator>().SetBool("Run", false);
    }

    public override void Exit()
    {
        base.Exit();

        ActorManager.Instance.moveBtn.onClick.RemoveListener(OnMoveCommandSelection);
        ActorManager.Instance.attackBtn.onClick.RemoveListener(OnAttackCommandSelection);
        ActorManager.Instance.finishBtn.onClick.RemoveListener(OnFinishTurnCommandSelection);
    }

    public void OnMoveCommandSelection()
    {
        ActorManager.Instance.ChangeState<ActorIdleState>();
    }

    public void OnAttackCommandSelection()
    {
        ActorManager.Instance.ChangeState<ActorAttackState>();
    }

    public void OnFinishTurnCommandSelection()
    {
        ActorManager.Instance.ChangeState<AfterTurnState>();
    }
}
