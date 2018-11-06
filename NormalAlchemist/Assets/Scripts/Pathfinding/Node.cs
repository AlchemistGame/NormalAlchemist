using UnityEngine;
using System.Collections;

namespace Pathfinding
{
    public class Node
    {
        // grid 坐标
        public int x;
        public int y;
        public int z;

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

        public Node parentNode;
        public bool isWalkable = true;

        //Types of nodes we can have, we will use this later on a case by case examples
        public NodeType nodeType;
        public enum NodeType
        {
            ground,
            air
        }
    }
}
