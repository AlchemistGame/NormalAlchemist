namespace MyBattle
{
    public abstract class CardData
    {
        public string name;
        public string description;
        public ActorData cardOwner;

        public CardData(string name, string desc, ActorData owner)
        {
            this.name = name;
            this.description = desc;
            this.cardOwner = owner;
        }

        public void DoEffect()
        {
            OnEffect();

            cardOwner.RemoveCard(this);
        }

        public abstract void OnEffect();
    }
}