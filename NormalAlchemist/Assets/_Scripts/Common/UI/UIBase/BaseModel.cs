using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModel
{
    dynamic _View = null;
    public dynamic View
    {
        get
        {
            return _View;
        }
        set
        {
            _View = value;
            InitModel(value);
        }
    }

    protected void TryCall(string key,string method,dynamic value )
    {
        if (View.bindCell.ContainsKey(key))
        {
            foreach (var i in View.bindCell[key])
            {
                BaseData sendData = new BaseData(i, value);
                View.GetType().GetMethod(method).Invoke(View,new object[]{ sendData});
            }
        }
    }

    protected virtual void InitModel(dynamic view) {
        foreach(var kv in view.ModelDefaultData)
        {
            this.GetType().GetProperty(kv.Key).SetValue(this, kv.Value);
        }
    }
}

public class BaseData
{
    public UIBindCellBase cell;
    public dynamic value;
    public BaseData(UIBindCellBase cell, dynamic data)
    {
        this.cell = cell;
        this.value = data;
    }
}
