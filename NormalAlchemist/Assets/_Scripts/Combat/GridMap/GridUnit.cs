using System;
using UnityEngine;

namespace MyBattle
{
    public class GridUnit : MonoBehaviour
    {
        public Material[] gridTypeMaterials;
        public Material[] gridStateMaterials;
        public Action<Action> OnTargetSelect;

        public void Init(GridUnitData data)
        {
            this.OnTargetSelect += data.OnTargetSelect;
        }

        public void Refresh(GridUnitData data)
        {
            transform.position = data.WorldPos;

            switch (data.gridState)
            {
                case GridState.normal:
                    this.GetComponent<Renderer>().material = gridTypeMaterials[(int)data.gridType];
                    break;
                case GridState.highlight:
                    this.GetComponent<Renderer>().material = gridStateMaterials[(int)data.gridState];
                    break;
                default:
                    break;
            }
        }
    }
}
