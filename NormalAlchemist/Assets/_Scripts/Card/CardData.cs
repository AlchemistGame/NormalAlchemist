namespace MyBattle
{
    public abstract class CardData
    {
        public string cardName;
        public string cardDescription;
        public ActorData cardOwner;

        public CardData(string name, string desc, ActorData owner)
        {
            this.cardName = name;
            this.cardDescription = desc;
            this.cardOwner = owner;
        }

        #region 提供给外部调用的接口
        public void SelectCurrentCard()
        {
            BattleManager.Instance.currentCard = this;

            OnPreExecute();
        }

        public void ExecuteCurrentCard()
        {
            OnExecute();
        }

        public void FinishExecuteCard()
        {
            cardOwner.RemoveCard(this);
            BattleManager.Instance.currentCard = null;

            OnAfterExecute();
        }
        #endregion

        #region 子类需要继承实现的接口
        public abstract void OnPreExecute();
        public abstract void OnExecute();
        public abstract void OnAfterExecute();
        #endregion
    }
}