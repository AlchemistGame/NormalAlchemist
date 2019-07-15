using System.Collections.Generic;
using UnityEngine;

namespace MyBattle
{
    public class ActorMoveState : State
    {
        private List<Vector3> movingPath;           // 包含了正在移动的所有目标点
        private float toNextNodeTotalTime = 0;      // 从当前位置到下个目标点所需经历总时长
        private float toNextNodeElapsedTime = 0;    // 已经过的时长
        private Vector3 curNodePos;                 // 玩家所处当前节点的坐标
        private float moveSpeed = 3f;
        private GameObject sceneObject
        {
            get
            {
                return BattleManager.Instance.currentActor.sceneObject.gameObject;
            }
        }

        public override void Enter()
        {
            base.Enter();

            BattleManager.Instance.currentActor.sceneObject.GetComponent<Animator>().SetBool("Run", true);

            movingPath = MyBattle.GridMapManager.Instance.GetPathFromStartToEnd(sceneObject.transform.position, BattleManager.Instance.targetPosition);
            if (movingPath.Count > 0)
            {
                toNextNodeTotalTime = (movingPath[0] - sceneObject.transform.position).magnitude / moveSpeed;
                curNodePos = sceneObject.transform.position;
                sceneObject.transform.LookAt(movingPath[0]);
            }

            BattleManager.Instance.OnUpdate += OnMove;
        }

        public override void Exit()
        {
            base.Exit();

            BattleManager.Instance.OnUpdate -= OnMove;
        }

        private void OnMove()
        {
            if (movingPath == null || movingPath.Count <= 0)
            {
                BattleManager.Instance.ChangeState<CommandSelectionState>();
            }
            else
            {
                toNextNodeElapsedTime += Time.deltaTime;

                if (toNextNodeElapsedTime >= toNextNodeTotalTime)
                {
                    sceneObject.transform.position = movingPath[0];
                    curNodePos = sceneObject.transform.position;
                    movingPath.RemoveAt(0);
                    toNextNodeElapsedTime = 0;

                    if (movingPath.Count > 0)
                    {
                        toNextNodeTotalTime = (movingPath[0] - sceneObject.transform.position).magnitude / moveSpeed;
                        sceneObject.transform.LookAt(movingPath[0]);
                    }
                }
                else
                {
                    sceneObject.transform.position = curNodePos + (toNextNodeElapsedTime / toNextNodeTotalTime) * (movingPath[0] - curNodePos);
                }
            }
        }
    }
}