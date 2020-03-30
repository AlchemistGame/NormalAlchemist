namespace MyBattle
{
    /// <summary>
    /// 回合结束后的善后工作
    /// </summary>
    public class AfterTurnState : State
    {
        public override void Enter()
        {
            BattleManager.Instance.NextTurn();
        }

        public override void Exit()
        {

        }
    }
}
