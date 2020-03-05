using System.Collections;
using UnityEngine;

namespace MyBattle
{
    public delegate void UpdateEventHandler();

    public class BattleManager : StateMachine
    {
        // 当前行动者
        [HideInInspector]
        public ActorData currentActor;
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
            // 生成一个主角
            AddActorToBattleField("CharacterModel/UnityChan", "Friend", "主角", new Vector2Int(25, 5), 50);

            // 生成一个敌人
            //AddActorToBattleField("CharacterModel/UnityChan", "Enemy", "敌人", new Int3(5, 0, 8));

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

        public void AddActorToBattleField(string model_path, string tag, string name, Vector2Int coord, int speed)
        {
            ActorData actor = ActorManager.Instance.CreateActor(model_path, tag, name, coord, speed);
            turnOrderController.AddActor(actor);
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
    }
}