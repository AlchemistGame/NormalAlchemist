using UnityEngine;

/// <summary>
/// 各种角色单位的基类 (怪物, NPC, 玩家)
/// </summary>
public class Actor : MonoBehaviour
{
    public int HP = 100;

    // 行动速度
    public int speed = 100;

    /// <summary>
    /// 此角色在场景中对应的物体
    /// </summary>
    public GameObject sceneObject;

    public bool isDead
    {
        get
        {
            return HP <= 0;
        }
    }

    protected virtual void Update()
    {

    }

    protected virtual void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter:" + name);
    }

    protected virtual void OnMouseExit()
    {
        Debug.Log("OnMouseExit:" + name);
    }
}
