using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIBindStringToLabel : UIBindCellBase
{
    public enum FieldType
    {
        String,
        Int,
        Float,
        Byte,
    }

    [SerializeField]
    public FieldType fieldType;

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

    public override object GetDefaultObj()
    {
        switch (fieldType)
        {
            case FieldType.String:
                return "";
            case FieldType.Int:
                return 0;
            case FieldType.Float:
                return 0.0f;
            case FieldType.Byte:
                return byte.MinValue;
        }
        return "";
    }

    public override string GetHandleFunc()
    {
        return "CallBindStringToText";
    }
}
