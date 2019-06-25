using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestWrapTestView : BaseView, IBindView
{
    #region WrapStart
    public new Dictionary<string, dynamic> ModelDefaultData = new Dictionary<string, dynamic>()
    {
         { "TestString","这是默认内容" },
    };
    #endregion WrapEnd
}