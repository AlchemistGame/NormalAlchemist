using UnityEngine;
using UnityEngine.EventSystems;

namespace MyBattle
{
    public class ActorAttackState : State
    {
        public override void Enter()
        {
            base.Enter();

            BattleManager.Instance.OnUpdate += OnAttackInput;
        }

        public override void Exit()
        {
            base.Exit();

            BattleManager.Instance.OnUpdate -= OnAttackInput;
        }

        public void OnAttackInput()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 鼠标正在操作 UI
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
            {
                // 攻击的必须是敌人(友军无法攻击)
                if (!BattleManager.Instance.currentActor.sceneObject.gameObject.tag.Equals(hit.collider.tag))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        BattleManager.Instance.currentActor.sceneObject.gameObject.transform.LookAt(hit.collider.transform);
                        BattleManager.Instance.currentActor.sceneObject.GetComponent<Animator>().SetBool("Jab", true);

                        BattleManager.Instance.ChangeState<CommandSelectionState>();
                    }
                }
            }
        }
    }
}