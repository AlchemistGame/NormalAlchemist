using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseView : MonoBehaviour
{
    public Dictionary<string, List<UIBindCellBase>> bindCell = new Dictionary<string, List<UIBindCellBase>>();
    public Dictionary<string, dynamic> ModelDefaultData;

    public void Awake()
    {
        var bindCellArray = gameObject.GetComponentsInChildren<UIBindCellBase>();
        for (int i = 0; i < bindCellArray.Length; i++)
        {
            if (!bindCell.ContainsKey(bindCellArray[i].bindCellName))
            {
                List<UIBindCellBase> uIBindCellBases = new List<UIBindCellBase>();
                uIBindCellBases.Add(bindCellArray[i]);
                bindCell.Add(bindCellArray[i].bindCellName, uIBindCellBases);
            }
            else
            {
                bindCell[bindCellArray[i].bindCellName].Add(bindCellArray[i]);
            }
        }
    }


}
