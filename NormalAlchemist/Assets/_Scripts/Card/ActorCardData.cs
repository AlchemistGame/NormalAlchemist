namespace MyBattle
{
    public class ActorCardData : CardData
    {
        public ActorCardData(string name, string desc, ActorData owner) : base(name, desc, owner)
        {

        }

        public override void OnEffect()
        {
            BattleManager.Instance.ChangeState<PreGenerateActorState>();
        }
    }
}
