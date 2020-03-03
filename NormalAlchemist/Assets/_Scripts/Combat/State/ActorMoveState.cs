using System.Collections.Generic;
using UnityEngine;

namespace MyBattle
{
    public class ActorMoveState : State
    {
        private List<GridUnitData> movingNodeList;           // 包含了正在移动的所有目标点
        private float toNextNodeTotalTime = 0.5f;     // 从当前位置到下个目标点所需经历总时长
        private float toNextNodeElapsedTime = 0;    // 已经过的时长
        private Vector3 curNodePos;                 // 玩家所处当前节点的坐标

        public override void Enter()
        {
            base.Enter();

            movingNodeList = GridMapManager.Instance.GetPathFromStartToEnd(BattleManager.Instance.currentActor.gridCoord,
                BattleManager.Instance.targetCoord);
            if (movingNodeList.Count > 0)
            {
                curNodePos = BattleManager.Instance.currentActor.DisplayPos;
                BattleManager.Instance.currentActor.sceneObject.transform.LookAt(GridMapManager.GridCoordToWorldPos(movingNodeList[0].gridCoord + new Int3(0, 1, 0)));
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
            if (movingNodeList == null || movingNodeList.Count <= 0)
            {
                BattleManager.Instance.ChangeState<CommandSelectionState>();
            }
            else
            {
                toNextNodeElapsedTime += Time.deltaTime;

                if (toNextNodeElapsedTime >= toNextNodeTotalTime)
                {
                    // 一个节点走完, 规范一下位置
                    BattleManager.Instance.currentActor.gridCoord = movingNodeList[0].gridCoord + new Int3(0, 1, 0);
                    movingNodeList.RemoveAt(0);

                    curNodePos = BattleManager.Instance.currentActor.DisplayPos;
                    toNextNodeElapsedTime = 0;

                    if (movingNodeList.Count > 0)
                    {
                        BattleManager.Instance.currentActor.sceneObject.transform.LookAt(GridMapManager.GridCoordToWorldPos(movingNodeList[0].gridCoord + new Int3(0, 1, 0)));
                    }
                }
                else
                {
                    BattleManager.Instance.currentActor.DisplayPos = curNodePos + (toNextNodeElapsedTime / toNextNodeTotalTime) *
                        (GridMapManager.GridCoordToWorldPos(movingNodeList[0].gridCoord + new Int3(0, 1, 0)) - curNodePos);
                }
            }
        }
    }
}