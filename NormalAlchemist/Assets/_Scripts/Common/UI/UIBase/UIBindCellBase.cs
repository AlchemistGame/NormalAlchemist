using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UIBindCellBase : MonoBehaviour
{
    public string bindCellName;
    public virtual object GetDefaultObj()
    {
        return null;
    }
    public virtual string GetHandleFunc()
    {
        return null;
    }
    public virtual string GetFuncName()
    {
        return null;
    }
}
