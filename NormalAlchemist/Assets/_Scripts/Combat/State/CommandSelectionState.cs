namespace MyBattle
{
    // 玩家选择指令
    public class CommandSelectionState : State
    {
        public override void Enter()
        {
            base.Enter();

            BattleManager.Instance.InitActorUI();
        }

        public override void Exit()
        {
            base.Exit();

            BattleManager.Instance.ClearActorUI();
        }
    }
}