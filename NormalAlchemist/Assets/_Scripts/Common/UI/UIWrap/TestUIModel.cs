using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 纯数据层,后续自动生成
/// </summary>
 public class TestUIModel : BaseModel
{
    private dynamic _testString;
    public dynamic TestString
    {
        get
        {
            return _testString;
        }
        set
        {
            _testString = value;
            //这个方法直接生成
            TryCall("TestString", "CallBindDataStringChange", value);
        }
    }
}
