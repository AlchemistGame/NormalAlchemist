using UnityEngine;

namespace MyBattle
{
    public struct Int3
    {
        public int x, y, z;

        public Int3(int tempX, int tempY, int tempZ)
        {
            x = tempX;
            y = tempY;
            z = tempZ;
        }

        public static bool operator ==(Int3 lhs, Int3 rhs)
        {
            bool status = false;
            if (lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z)
            {
                status = true;
            }
            return status;
        }

        public static bool operator !=(Int3 lhs, Int3 rhs)
        {
            bool status = false;
            if (lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z)
            {
                status = true;
            }
            return status;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", x, y, z);
        }
    }

    public enum GridType
    {
        ground,
        cloud,
        Max
    }

    public enum GridState
    {
        normal,
        highlight,
    }

    public class GridUnitData
    {
        public Int3 gridCoord;
        public GridType gridType;
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

        public bool IsWalkable
        {
            get
            {
                bool isWalkable = true;

                //GameObject[] blockObjects = GameObject.FindGameObjectsWithTag("Enemy");
                //foreach (var blockObj in blockObjects)
                //{
                //    if (gridCoord == CalcGridCoord(blockObj.transform.position))
                //    {
                //        isWalkable = false;
                //        break;
                //    }
                //}

                switch (gridType)
                {
                    case GridType.ground:
                        break;
                    case GridType.cloud:
                        isWalkable = false;
                        break;
                    default:
                        break;
                }

                return isWalkable;
            }
        }

        public Vector3 WorldCoord
        {
            get
            {
                return GridMapManager.GridCoordToWorldPos(gridCoord);
            }
        }

        public GridUnitData parentNode;

        public GridUnit CreateGridUnit()
        {
            if (instanceGU == null)
            {
                GridUnit loadGU = Resources.Load<GridUnit>(prefabPath);
                if (loadGU != null)
                {
                    instanceGU = Object.Instantiate(loadGU);
                    instanceGU.name = gridCoord.ToString();
                    instanceGU.transform.parent = GridMapManager.Instance.transform;
                    RefreshInstance();
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
    }
}