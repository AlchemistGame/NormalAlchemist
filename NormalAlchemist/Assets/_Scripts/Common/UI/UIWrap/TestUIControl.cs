using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 逻辑层，用于实现具体的逻辑和刷新对应的Data数据
/// </summary>
/// 
public class TestUIControl : BaseControll
{
    //public TestUIModel testModel;
    public void Start()
    {
        SetModel<TestUIModel>();
        data.TestString = "测试";
    }
}
