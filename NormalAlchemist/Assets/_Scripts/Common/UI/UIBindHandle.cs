using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBindHandle
{
    public static void CallFunc(string funcName)
    {

    }

    public static void CallBindStringToText(UIBindCellBase cell,object value)
    {
        ((UIBindStringToLabel)cell).LabelText = value.ToString();
    }

}
