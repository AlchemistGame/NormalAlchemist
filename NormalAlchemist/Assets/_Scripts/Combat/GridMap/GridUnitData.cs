using UnityEngine;

namespace MyBattle
{
    public enum GridState
    {
        normal,
        highlight,
    }

    public class GridUnitData
    {
        public Vector3Int gridCoord;
        public BlockType gridType;
        public GridState gridState;

        private const string prefabPath = "GridMap/GridUnit";
        private GridUnit instanceGU;

        //Node's costs for pathfinding purposes
        public float hCost;
        public float gCost;
        public float fCost
        {
            get //the fCost is the gCost+hCost so we can get it directly this way
            {
                return gCost + hCost;
            }
        }

        // 能不能走到这个方块上面 => + new Vector3Int(0, 1, 0)
        public bool IsWalkable
        {
            get
            {
                //GameObject[] blockObjects = GameObject.FindGameObjectsWithTag("Enemy");
                //foreach (var blockObj in blockObjects)
                //{
                //    if (gridCoord == CalcGridCoord(blockObj.transform.position))
                //    {
                //        isWalkable = false;
                //        break;
                //    }
                //}

                // 当前方块的上方已经存在一个方块
                Vector3Int aboveGridCoord = gridCoord + new Vector3Int(0, 1, 0);
                GridUnitData aboveGridUnitData = GridMapManager.GetGridUnitDataFromGridCoord(aboveGridCoord);
                if (aboveGridUnitData == null || aboveGridUnitData.gridType != BlockType.None)
                {
                    return false;
                }
                // 当前方块的上方已经站有一个角色
                Vector2Int aboveActorCoord = new Vector2Int(gridCoord.x, gridCoord.z);
                ActorData aboveActorData = ActorManager.Instance.GetActorDataFrom2DCoord(aboveActorCoord);
                if (aboveActorData != null)
                {
                    return false;
                }

                switch (gridType)
                {
                    case BlockType.None:
                        return false;
                    case BlockType.Mud:
                        break;
                    case BlockType.Rock:
                        break;
                    case BlockType.Grass:
                        break;
                    default:
                        break;
                }

                return true;
            }
        }

        public Vector3 WorldPos
        {
            get
            {
                return GridMapManager.GridCoordToWorldPos(gridCoord);
            }
        }

        public GridUnitData parentNode;

        public GridUnit CreateGridUnit()
        {
            if (gridType == BlockType.None)
            {
                if (instanceGU != null)
                {
                    Object.Destroy(instanceGU.gameObject);
                }
                instanceGU = null;
            }
            else
            {
                if (instanceGU == null)
                {
                    GridUnit loadGU = Resources.Load<GridUnit>(prefabPath);
                    if (loadGU != null)
                    {
                        instanceGU = Object.Instantiate(loadGU);
                        instanceGU.name = gridCoord.ToString();
                        instanceGU.transform.parent = GridMapManager.Instance.transform;
                        instanceGU.Init(this);
                        RefreshInstance();
                    }
                }
            }

            return instanceGU;
        }

        public void RefreshInstance()
        {
            instanceGU.Refresh(this);
        }

        public void OnMouseDown(int mouseButton)
        {

        }

        public void OnMouseUp(int mouseButton)
        {

        }

        public void OnMouseHold(int mouseButton)
        {

        }

        public void OnMouseEnter()
        {
            gridState = GridState.highlight;
        }

        public void OnMouseOver()
        {

        }

        public void OnMouseExit()
        {
            gridState = GridState.normal;
        }

        #region Public 接口
        public void OnTargetSelect(System.Action action)
        {
            Debug.Log(instanceGU.name);
            BattleManager.Instance.targetCoord = new Vector2Int(gridCoord.x, gridCoord.z);

            action?.Invoke();
        }
        #endregion
    }
}