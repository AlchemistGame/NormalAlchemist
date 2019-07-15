using MyBattle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorOperationPanel : MonoBehaviour
{
    public ScrollRect CardContainer;
    public CardItem CardPrefab;
    public Text ActorName;
    public Button MoveBtn;
    public Button NormalAttackBtn;
    public Button FinishOperationBtn;

    private List<CardItem> cardItemList = new List<CardItem>();

    #region public API
    public void InitActorData(ActorData actor)
    {
        InitInfoPanel(actor);
        GenerateCards(actor.OwnedCards);
        BindingActorEvents(actor);
    }

    public void ClearActorData()
    {
        ActorName.text = string.Empty;

        for (int i = 0; i < cardItemList.Count; i++)
        {
            Destroy(cardItemList[i].gameObject);
        }
        cardItemList.Clear();

        MoveBtn.onClick.RemoveAllListeners();
        NormalAttackBtn.onClick.RemoveAllListeners();
        FinishOperationBtn.onClick.RemoveAllListeners();
    }
    #endregion

    #region private Methods
    private void InitInfoPanel(ActorData actorData)
    {
        ActorName.text = actorData.name;
    }

    private void GenerateCards(List<CardData> cardDataList)
    {
        for (int i = 0; i < cardDataList.Count; i++)
        {
            CardItem cardInstance = Instantiate<CardItem>(CardPrefab);
            cardInstance.InitData(cardDataList[i]);
            cardInstance.transform.SetParent(CardContainer.content);
            cardInstance.transform.localScale = Vector3.one;
            cardInstance.gameObject.SetActive(true);
            cardItemList.Add(cardInstance);
        }
    }

    private void BindingActorEvents(ActorData actor)
    {
        MoveBtn.onClick.AddListener(actor.OnMove);
        NormalAttackBtn.onClick.AddListener(actor.OnNormalAttack);
        FinishOperationBtn.onClick.AddListener(actor.OnFinishOperation);
    }
    #endregion
}
