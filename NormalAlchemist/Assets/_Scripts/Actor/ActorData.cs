using System.Collections.Generic;
using UnityEngine;

namespace MyBattle
{
    /// <summary>
    /// 各种角色单位的基类 (怪物, NPC, 玩家)
    /// </summary>
    public class ActorData
    {
        public string name;
        public Int3 coord;
        public int HP = 100;
        public int speed = 100; // 行动速度
        public List<CardData> OwnedCards { get; private set; }
        public Actor sceneObject;

        public bool IsDead
        {
            get
            {
                return HP <= 0;
            }
        }

        public ActorData(string name, Int3 coord, int speed, Actor actor)
        {
            this.name = name;
            this.coord = coord;
            this.speed = speed;
            this.sceneObject = actor;

            OwnedCards = new List<CardData>();
        }

        #region 子类继承的接口
        protected virtual void Update()
        {

        }

        protected virtual void OnMouseEnter()
        {
            Debug.Log("OnMouseEnter:" + name);
        }

        protected virtual void OnMouseExit()
        {
            Debug.Log("OnMouseExit:" + name);
        }
        #endregion

        #region 供外部使用的接口
        public virtual void OnMove()
        {
            BattleManager.Instance.ChangeState<ActorIdleState>();
        }

        public virtual void OnNormalAttack()
        {
            BattleManager.Instance.ChangeState<ActorAttackState>();
        }

        public virtual void OnFinishOperation()
        {
            BattleManager.Instance.ChangeState<AfterTurnState>();
        }

        public void AddCard(CardData cardData)
        {
            OwnedCards.Add(cardData);
        }

        public void RemoveCard(CardData cardData)
        {
            OwnedCards.Remove(cardData);
        }

        // 刷新场上的显示
        public void Refresh()
        {
            sceneObject.transform.position = GridMapManager.GridCoordToWorldPos(coord);
        }
        #endregion
    }
}