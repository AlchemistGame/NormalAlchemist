using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewTestUIView : BaseView, IBindView
{
#region WrapStart
    public new Dictionary<string, dynamic> ModelDefaultData = new Dictionary<string, dynamic>()
    {
         { "TestString_One","这是默认内容" },
    };
    #endregion WrapEnd
}