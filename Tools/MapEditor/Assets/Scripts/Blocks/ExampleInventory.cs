using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// stores the currently held block, and switches it with 1-9 keys
namespace VoxelFramework
{
    public class ExampleInventory : MonoBehaviour
    {
        public Text blockNameText;
        public static ushort HeldBlock;

        public void Update()
        {
            // 切换玩家持有的方块
            for (ushort i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    if (Engine.GetVoxelType(i) != null)
                    {
                        ExampleInventory.HeldBlock = i;
                        blockNameText.text = "当前持有方块：" + i.ToString();
                    }
                }
            }

        }
    }

}
