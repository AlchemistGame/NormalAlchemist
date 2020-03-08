namespace MyBattle
{
    public class GenerateActorCardData : CardData
    {
        public string actorModelPath;
        public ActorCamp actorCamp;
        public string actorName;

        public GenerateActorCardData(string name, string desc, ActorData owner,
            string actorModelPath, ActorCamp actorCamp, string actorName) : base(name, desc, owner)
        {
            this.actorModelPath = actorModelPath;
            this.actorCamp = actorCamp;
            this.actorName = actorName;
        }

        public override void OnPreExecute()
        {
            BattleManager.Instance.ChangeState<PreExecuteCardState>();
        }

        public override void OnExecute()
        {
            ActorData actor = ActorManager.Instance.CreateActor(actorModelPath, actorCamp, actorName, BattleManager.Instance.targetCoord, 70);
            actor.AddCard(new GenerateActorCardData(cardName, cardDescription, actor,
                actorModelPath, actorCamp, "被召唤者" + UnityEngine.Random.Range(0, 1000)));
            BattleManager.Instance.AddActorToBattleField(actor);

            FinishExecuteCard();
        }

        public override void OnAfterExecute()
        {
            BattleManager.Instance.ChangeState<CommandSelectionState>();
        }
    }
}
