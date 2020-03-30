using System.Collections;
using UnityEngine;

namespace MyBattle
{
    public delegate void UpdateEventHandler();

    public class BattleManager : StateMachine
    {
        // 当前行动者
        public ActorData currentActor;
        public CardData currentCard;
        public event UpdateEventHandler OnUpdate;
        [HideInInspector]
        public Vector2Int targetCoord;
        public static BattleManager Instance { get; private set; }
        [SerializeField]
        private ActorOperationPanel actorOperationPanel;

        private TurnOrderController turnOrderController;
        private IEnumerator turnOrderEnumerator;
        private GridUnitData selectedGridUnitData;

        private void Awake()
        {
            Instance = this;

            turnOrderController = new TurnOrderController();
            turnOrderEnumerator = turnOrderController.Tick();

            EventManager.Register(EventsEnum.PreMoveActor, this, "PreMoveActor");
            EventManager.Register(EventsEnum.DoAttackActor, this, "DoAttackActor");
            EventManager.Register(EventsEnum.DoFinishOperation, this, "DoFinishOperation");
            EventManager.Register(EventsEnum.FinishGenerateGridMap, this, "InitScene");
        }

        private void Update()
        {
            OnUpdate?.Invoke();

            MouseCursorEvents();
        }

        public void InitScene()
        {
            // 生成主角
            ActorData playerActor = ActorManager.Instance.CreateActor("CharacterModel/UnityChanPlayer", ActorCamp.friend, "主角", new Vector2Int(25, 5), 50);
            playerActor.AddCard(new GenerateActorCardData("召唤Unity娘", "召唤一只Unity娘", playerActor,
                "CharacterModel/UnityChanFriend", ActorCamp.friend, "被召唤者" + UnityEngine.Random.Range(0, 1000)));
            AddActorToBattleField(playerActor);

            // 生成敌人
            ActorData enemyActor = ActorManager.Instance.CreateActor("CharacterModel/UnityChanEnemy", ActorCamp.enemy, "敌人", new Vector2Int(35, 10), 30);
            enemyActor.AddCard(new GenerateActorCardData("召唤Unity娘", "召唤一只Unity娘", enemyActor,
                "CharacterModel/UnityChanEnemy", ActorCamp.enemy, "被召唤者" + UnityEngine.Random.Range(0, 1000)));
            AddActorToBattleField(enemyActor);

            NextTurn();
        }

        public void NextTurn()
        {
            turnOrderEnumerator.MoveNext();
            currentActor = (ActorData)turnOrderEnumerator.Current;
            ChangeState<PreTurnState>();
        }

        private void MouseCursorEvents()
        {
            GridUnitData raycastGridUnitData = GridMapManager.GridUnitRaycast(Camera.main.ScreenPointToRay(Input.mousePosition), 9999.9f);

            if (raycastGridUnitData != selectedGridUnitData)
            {
                if (selectedGridUnitData != null)
                {
                    selectedGridUnitData.OnMouseExit();
                }

                if (raycastGridUnitData != null)
                {
                    raycastGridUnitData.OnMouseEnter();
                }
            }

            if (raycastGridUnitData != null)
            {
                raycastGridUnitData.OnMouseOver();

                // for all mouse buttons, send events
                for (int i = 0; i < 3; i++)
                {
                    if (Input.GetMouseButtonDown(i))
                    {
                        raycastGridUnitData.OnMouseDown(i);
                    }
                    if (Input.GetMouseButtonUp(i))
                    {
                        raycastGridUnitData.OnMouseUp(i);
                    }
                    if (Input.GetMouseButton(i))
                    {
                        raycastGridUnitData.OnMouseHold(i);
                    }
                }
            }

            if (selectedGridUnitData != null)
            {
                selectedGridUnitData.RefreshInstance();
            }
            if (raycastGridUnitData != null)
            {
                raycastGridUnitData.RefreshInstance();
            }

            selectedGridUnitData = raycastGridUnitData;
        }

        public void InitActorUI()
        {
            actorOperationPanel.InitActorData(currentActor);
        }

        public void ClearActorUI()
        {
            actorOperationPanel.ClearActorData();
        }

        public void PreMoveActor()
        {
            ChangeState<PreMoveState>();
        }

        public void DoAttackActor()
        {
            ChangeState<ActorAttackState>();
        }

        public void DoFinishOperation()
        {
            ChangeState<AfterTurnState>();
        }

        #region 供外部调用的接口
        public void AddActorToBattleField(ActorData actorData)
        {
            turnOrderController.AddActor(actorData);
        }

        public void RemoveActorFromBattleField(ActorData actorData)
        {
            turnOrderController.RemoveActor(actorData);
        }
        #endregion
    }
}