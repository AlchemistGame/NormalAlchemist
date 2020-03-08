namespace MyBattle
{
    /// <summary>
    /// 回合前的准备工作
    /// </summary>
    public class PreTurnState : State
    {
        public override void Enter()
        {
            BattleManager.Instance.ChangeState<CommandSelectionState>();
        }

        public override void Exit()
        {

        }
    }
}