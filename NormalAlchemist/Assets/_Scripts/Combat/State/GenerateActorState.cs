namespace MyBattle
{
    public class GenerateActorState : State
    {
        public override void Enter()
        {
            base.Enter();

            BattleManager.Instance.AddActorToBattleField("Model/UnityChan", "Friend", "召唤物" + UnityEngine.Random.Range(0, 1000),
                BattleManager.Instance.targetCoord, 100);

            BattleManager.Instance.ChangeState<CommandSelectionState>();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}