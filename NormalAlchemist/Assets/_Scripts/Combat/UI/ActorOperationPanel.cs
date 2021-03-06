﻿using MyBattle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorOperationPanel : MonoBehaviour
{
    public ScrollRect CardContainer;
    public CardItem CardPrefab;
    public Text ActorName;
    public Text ActorHp;
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
        MoveBtn.gameObject.SetActive(false);
        NormalAttackBtn.gameObject.SetActive(false);
        FinishOperationBtn.gameObject.SetActive(false);
        ActorName.text = string.Empty;
        ActorHp.text = string.Empty;

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
        MoveBtn.gameObject.SetActive(true);
        NormalAttackBtn.gameObject.SetActive(true);
        FinishOperationBtn.gameObject.SetActive(true);
        ActorName.text = actorData.name;
        ActorHp.text = "HP:" + actorData.HP;
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
        MoveBtn.onClick.AddListener(() => EventManager.Broadcast(EventsEnum.PreMoveActor));
        NormalAttackBtn.onClick.AddListener(() => EventManager.Broadcast(EventsEnum.DoAttackActor));
        FinishOperationBtn.onClick.AddListener(() => EventManager.Broadcast(EventsEnum.DoFinishOperation));
    }
    #endregion
}
