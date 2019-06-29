using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public interface IEffectTargetSet
{
    void SetTarget(GameObject Go);
    void SetTarget(List<GameObject> GoList);
    void SetTarget(Vector3 Pos);
    void SetTarget(List<Vector3> PosList);
}
public enum emStaticEffectType
{
    OnlySelf,
    SelfTargetGo,
    SelfMultiTargetGo,
    SelfTargetPos,
    SelfMultiTargetPos,
}
public class StaticEffect : EffectBase
{
    //public bool isFollowUser;
    public emStaticEffectType isNeedTargetAndType;
    public IEffectTargetSet[] EffectChilds;
    // Use this for initialization
    public override void Init()
    {
        base.Init();
        EffectChilds = GetComponentsInChildren<IEffectTargetSet>(gameObject);
        switch (isNeedTargetAndType)
        {
            case emStaticEffectType.OnlySelf:
                break;
            case emStaticEffectType.SelfTargetGo:
                foreach (IEffectTargetSet childTargetSet in EffectChilds)
                {
                    childTargetSet.SetTarget(TargetGo);
                }
                break;
            case emStaticEffectType.SelfMultiTargetGo:
                foreach (IEffectTargetSet childTargetSet in EffectChilds)
                {
                    childTargetSet.SetTarget(MultiTargetGo);
                }
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (!_isInit)
        {
            Init();
        }
        if (!_isPlaying)
        {
            return;
        }

        if (Time.time - startMoveMentTime > LifeTime && LifeTime != 0)
        {
            //Destroy(gameObject);
            Clear();
            CanUseSkill.EffectPool.DestoryPrefabByPool(gameObject.transform);
        }
    }
    
}
