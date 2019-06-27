/// <summary>
/// 回合结束后的善后工作
/// </summary>
public class AfterTurnState : State
{
    public override void Enter()
    {
        base.Enter();

        ActorManager.Instance.NextTurn();
    }
}
