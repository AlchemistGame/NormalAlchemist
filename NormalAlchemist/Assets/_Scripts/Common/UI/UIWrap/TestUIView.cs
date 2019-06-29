using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 纯表现层，用于注册对应的bind事件和实现具体的bind事件逻辑处理
/// </summary>
public class TestUIView : BaseView, IBindView
{
    #region WrapStart
    public new Dictionary<string, dynamic> ModelDefaultData = new Dictionary<string, dynamic>()
    {
        { "TestString","这是初始值"},
    };
    #endregion WrapEnd

    public void CallBindDataStringChange(BaseData data)
    {
        (data.cell as UIBindStringToLabel).LabelText = data.value;
    }
}
