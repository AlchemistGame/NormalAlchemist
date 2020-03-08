using UnityEngine;
using UnityEngine.EventSystems;

namespace MyBattle
{
    public class ActorAttackState : State
    {
        public override void Enter()
        {
            BattleManager.Instance.OnUpdate += OnAttackInput;
        }

        public override void Exit()
        {
            BattleManager.Instance.OnUpdate -= OnAttackInput;
        }

        public void OnAttackInput()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 鼠标正在操作 UI
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // 攻击的必须是敌人(友军无法攻击)
                    Actor actor = hit.collider.GetComponent<Actor>();
                    if (actor != null && actor.data != null && actor.data.CanBeAttackedByCurrentActor)
                    {
                        actor.data.OnAttackedByOtherActor(BattleManager.Instance.ChangeState<CommandSelectionState>);
                    }
                }
            }
        }
    }
}