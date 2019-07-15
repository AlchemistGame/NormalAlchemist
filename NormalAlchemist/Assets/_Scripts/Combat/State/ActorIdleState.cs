using UnityEngine;
using UnityEngine.EventSystems;

namespace MyBattle
{
    public class ActorIdleState : State
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

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag.Equals(TagDefine.land))
                {
                    Vector3 hitPosition = hit.point;

                    if (Input.GetMouseButtonDown(0))
                    {
                        BattleManager.Instance.targetPosition = hitPosition;

                        BattleManager.Instance.ChangeState<ActorMoveState>();
                    }
                }
            }
        }
    }
}