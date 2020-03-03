using System.Collections.Generic;
using UnityEngine;

// A* search algorithm
namespace MyBattle
{
    public class PathFinder
    {
        private static PathFinder pathFinder;
        private static PathFinder Instance
        {
            get
            {
                if (pathFinder == null)
                {
                    pathFinder = new PathFinder();
                }

                return pathFinder;
            }
        }

        public static List<GridUnitData> RequestPathfind(GridUnitData start, GridUnitData target)
        {
            return Instance.FindPath(start, target);
        }

        private List<GridUnitData> FindPath(GridUnitData start, GridUnitData target)
        {
            List<GridUnitData> foundPath = new List<GridUnitData>();

            HashSet<GridUnitData> closedSet = new HashSet<GridUnitData>();
            closedSet.Add(start);   // 出发点肯定是最终路径的一部分

            List<GridUnitData> openSet = new List<GridUnitData>();
            openSet.Add(start);
            while (openSet.Count > 0)
            {
                GridUnitData currentNode = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost ||
                        (openSet[i].fCost == currentNode.fCost &&
                        openSet[i].hCost < currentNode.hCost))
                    {
                        //and then we assign a new current node
                        if (!currentNode.Equals(openSet[i]))
                        {
                            currentNode = openSet[i];
                        }
                    }
                }

                //we remove the current node from the open set and add to the closed set
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                //if the current node is the target node
                if (currentNode.Equals(target))
                {
                    //that means we reached our destination, so we are ready to retrace our path
                    foundPath = RetracePath(start, currentNode);
                    break;
                }

                //if we haven't reached our target, then we need to start looking the neighbours
                foreach (GridUnitData neighbour in GetNeighbours(currentNode))
                {
                    if (!closedSet.Contains(neighbour))
                    {
                        //we create a new movement cost for our neighbours
                        float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                        //and if it's lower than the neighbour's cost
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            //we calculate the new costs
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, target);
                            //Assign the parent node
                            neighbour.parentNode = currentNode;
                            //And add the neighbour node to the open set
                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
            }

            //we return the path at the end
            return foundPath;
        }

        private List<GridUnitData> RetracePath(GridUnitData startNode, GridUnitData endNode)
        {
            //Retrace the path, is basically going from the endNode to the startNode
            List<GridUnitData> path = new List<GridUnitData>();
            GridUnitData currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                //by taking the parentNodes we assigned
                currentNode = currentNode.parentNode;
            }

            //then we simply reverse the list
            path.Reverse();

            return path;
        }

        private List<GridUnitData> GetNeighbours(GridUnitData node, bool getVerticalNeighbours = false)
        {
            //This is were we start taking our neighbours
            List<GridUnitData> retList = new List<GridUnitData>();

            // + new Int3(0, 0, 0) is the current node
            for (int step = -1; step <= 1; step += 1)
            {
                Int3 searchPos = new Int3();

                searchPos = node.gridCoord + new Int3(step, 0, 0);
                GridUnitData newNode = GetNeighbourNode(searchPos, true, node);
                if (newNode != null)
                {
                    retList.Add(newNode);
                }

                //If we don't want a 3d A*, then we don't search the y
                if (getVerticalNeighbours)
                {
                    searchPos = node.gridCoord + new Int3(0, step, 0);
                    newNode = GetNeighbourNode(searchPos, true, node);
                    if (newNode != null)
                    {
                        retList.Add(newNode);
                    }
                }

                searchPos = node.gridCoord + new Int3(0, 0, step);
                newNode = GetNeighbourNode(searchPos, true, node);
                if (newNode != null)
                {
                    retList.Add(newNode);
                }
            }

            return retList;
        }

        private GridUnitData GetNeighbourNode(Int3 adjPos, bool searchTopDown, GridUnitData currentNodePos)
        {
            //this is where the meat of it is
            //We can add all the checks we need here to tweak the algorythm to our heart's content
            //but first let's start from the the usual stuff you'll see in A*

            GridUnitData retVal = null;

            //let's take the node from the adjacent positions we passed
            GridUnitData node = GridMapManager.GetGridUnitDataFromGridCoord(adjPos);

            //if it's not null and we can walk on it
            if (node != null && node.IsWalkable)
            {
                //we can use that node
                retVal = node;
            }//if not
            else if (searchTopDown)//and we want to have 3d A* 
            {
                //then look what the adjacent node have under him
                adjPos.y -= 1;
                GridUnitData bottomBlock = GridMapManager.GetGridUnitDataFromGridCoord(adjPos);

                //if there is a bottom block and we can walk on it
                if (bottomBlock != null && bottomBlock.IsWalkable)
                {
                    retVal = bottomBlock;// we can return that
                }
                else
                {
                    //otherwise, we look what it has on top of it
                    adjPos.y += 2;
                    GridUnitData topBlock = GridMapManager.GetGridUnitDataFromGridCoord(adjPos);
                    if (topBlock != null && topBlock.IsWalkable)
                    {
                        retVal = topBlock;
                    }
                }
            }

            //if the node is diagonal to the current node then check the neighbouring nodes
            //so to move diagonally, we need to have 4 nodes walkable
            int originalX = adjPos.x - currentNodePos.gridCoord.x;
            int originalZ = adjPos.z - currentNodePos.gridCoord.z;

            if (Mathf.Abs(originalX) == 1 && Mathf.Abs(originalZ) == 1)
            {
                // the first block is originalX, 0 and the second to check is 0, originalZ
                //They need to be pathfinding walkable
                GridUnitData neighbour1 = GridMapManager.GetGridUnitDataFromGridCoord(
                    new Int3(currentNodePos.gridCoord.x + originalX, currentNodePos.gridCoord.y, currentNodePos.gridCoord.z));
                if (neighbour1 == null || !neighbour1.IsWalkable)
                {
                    retVal = null;
                }

                GridUnitData neighbour2 = GridMapManager.GetGridUnitDataFromGridCoord(
                    new Int3(currentNodePos.gridCoord.x, currentNodePos.gridCoord.y, currentNodePos.gridCoord.z + originalZ));
                if (neighbour2 == null || !neighbour2.IsWalkable)
                {
                    retVal = null;
                }
            }

            //and here's where we can add even more additional checks
            if (retVal != null)
            {
                //Example, do not approach a node from the left
                /*if(node.x > currentNodePos.x) {
                    node = null;
                }*/
            }

            return retVal;
        }

        private int GetDistance(GridUnitData posA, GridUnitData posB)
        {
            //We find the distance between each node
            //not much to explain here

            int distX = Mathf.Abs(posA.gridCoord.x - posB.gridCoord.x);
            int distZ = Mathf.Abs(posA.gridCoord.z - posB.gridCoord.z);
            int distY = Mathf.Abs(posA.gridCoord.y - posB.gridCoord.y);

            if (distX > distZ)
            {
                return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
            }

            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }

    }
}
