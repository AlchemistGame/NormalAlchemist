namespace MyBattle
{
    // 玩家选择指令
    public class CommandSelectionState : State
    {
        public override void Enter()
        {
            BattleManager.Instance.InitActorUI();
        }

        public override void Exit()
        {
            BattleManager.Instance.ClearActorUI();
        }
    }
}