using UnityEngine;
using UnityEngine.EventSystems;

namespace MyBattle
{
    public class PreMoveState : State
    {
        public override void Enter()
        {
            base.Enter();

            BattleManager.Instance.OnUpdate += OnIdleInput;
        }

        public override void Exit()
        {
            base.Exit();

            BattleManager.Instance.OnUpdate -= OnIdleInput;
        }

        private void OnIdleInput()
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
                    GridUnit gu = hit.collider.GetComponent<GridUnit>();
                    if (gu != null && gu.OnTargetSelect != null)
                    {
                        gu.OnTargetSelect(BattleManager.Instance.ChangeState<ActorMoveState>);
                    }
                }
            }
        }
    }
}