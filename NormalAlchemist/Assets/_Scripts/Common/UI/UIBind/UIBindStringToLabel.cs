using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIBindStringToLabel : UIBindCellBase
{
    public Text text;
    public string LabelText
    {
        get
        {
            return text.text;
        }  
        set
        {
            text.text = value;
        }
    }


}
