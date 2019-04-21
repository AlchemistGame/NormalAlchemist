using UnityEngine;

public abstract class Actor
{
    /// <summary>
    /// 角色名
    /// </summary>
    public string name;
    /// <summary>
    /// 角色位置
    /// </summary>
    public Vector3 position;
    /// <summary>
    /// 角色朝向
    /// </summary>
    public Vector3 rotation;
    /// <summary>
    /// 此角色在场景中对应的物体
    /// </summary>
    public GameObject sceneObject;

    /// <summary>
    /// 每帧更新状态
    /// </summary>
    public abstract void OnUpdate();

    /// <summary>
    /// 回合开始时调用
    /// </summary>
    public abstract void OnTurnBegin();

    /// <summary>
    /// 回合结束时调用
    /// </summary>
    public abstract void OnTurnEnd();

    public virtual void OnMouseEnter()
    {
        // 鼠标进入模型
    }

    public virtual void OnMouseExit()
    {
        // 鼠标离开模型
    }

    public virtual void OnMouseClick()
    {
        // 鼠标点击模型
    }
}
