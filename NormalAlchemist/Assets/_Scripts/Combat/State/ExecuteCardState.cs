namespace MyBattle
{
    public class ExecuteCardState : State
    {
        public override void Enter()
        {
            BattleManager.Instance.currentCard.ExecuteCurrentCard();
        }

        public override void Exit()
        {
        }
    }
}