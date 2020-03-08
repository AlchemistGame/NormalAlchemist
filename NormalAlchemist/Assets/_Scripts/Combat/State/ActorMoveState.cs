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
            movingNodeList = GridMapManager.Instance.GetPathFromStartToEnd(BattleManager.Instance.currentActor.coord,
                BattleManager.Instance.targetCoord);
            if (movingNodeList.Count > 0)
            {
                curNodePos = BattleManager.Instance.currentActor.DisplayPos;
            }

            BattleManager.Instance.OnUpdate += OnMove;
        }

        public override void Exit()
        {
            BattleManager.Instance.OnUpdate -= OnMove;

            BattleManager.Instance.currentActor.coord = BattleManager.Instance.targetCoord;
            BattleManager.Instance.currentActor.Refresh();
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
                    movingNodeList.RemoveAt(0);

                    curNodePos = BattleManager.Instance.currentActor.DisplayPos;
                    toNextNodeElapsedTime = 0;
                }
                else
                {
                    BattleManager.Instance.currentActor.DisplayPos = curNodePos + (toNextNodeElapsedTime / toNextNodeTotalTime) *
                        (GridMapManager.GridCoordToWorldPos(movingNodeList[0].gridCoord) - curNodePos);
                }
            }
        }
    }
}