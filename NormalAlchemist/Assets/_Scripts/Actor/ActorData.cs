using System.Collections.Generic;
using UnityEngine;

namespace MyBattle
{
    public enum ActorCamp
    {
        friend,
        enemy
    }

    /// <summary>
    /// 各种角色单位的基类 (怪物, NPC, 玩家)
    /// </summary>
    public abstract class ActorData
    {
        public string name;
        public ActorCamp camp;
        public Vector2Int coord;
        public int speed = 100; // 行动速度
        public List<CardData> OwnedCards { get; private set; }
        public float modelHeight = 1f;

        protected Actor sceneObject;

        private int hp = 100;
        private string prefabPath;

        public ActorData(string name, ActorCamp camp, Vector2Int coord, int speed, string prefabPath)
        {
            this.name = name;
            this.camp = camp;
            this.coord = coord;
            this.speed = speed;
            this.prefabPath = prefabPath;

            OwnedCards = new List<CardData>();
        }

        #region 公共属性
        public bool CanBeAttackedByCurrentActor
        {
            get
            {
                if (BattleManager.Instance.currentActor.camp != this.camp)
                {
                    return true;
                }

                return false;
            }
        }

        public int HP
        {
            get
            {
                return hp;
            }
            set
            {
                hp = value;
                if (hp <= 0)
                {
                    OnDestroy();
                }
            }
        }

        public Vector3 DisplayPos
        {
            get
            {
                return sceneObject.transform.position - new Vector3(0, modelHeight / 2, 0);
            }
            set
            {
                // 地面高度 + 模型高度
                sceneObject.transform.position = value + new Vector3(0, modelHeight / 2, 0);
            }
        }
        #endregion

        #region 子类继承的接口
        protected virtual void OnDestroy()
        {
            Object.Destroy(sceneObject.gameObject);
            ActorManager.Instance.DestroyActor(this);
            BattleManager.Instance.RemoveActorFromBattleField(this);
        }

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
        public Actor CreateSceneObject()
        {
            if (sceneObject == null)
            {
                sceneObject = Object.Instantiate<Actor>(Resources.Load<Actor>(prefabPath));
                sceneObject.name = name;
                sceneObject.transform.SetParent(ActorManager.Instance.transform);
                sceneObject.Init(this);
            }

            Refresh();

            return sceneObject;
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
            DisplayPos = GridMapManager.ActorCoordToWorldPos(coord);
        }

        public void OnAttackedByOtherActor(System.Action action)
        {
            this.HP -= 50;

            action?.Invoke();
        }
        #endregion
    }
}