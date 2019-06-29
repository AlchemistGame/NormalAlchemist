using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// 技能管理器
/// </summary>
public class SkillManager : BaseSceneManager
{
    private static SkillManager instance;

    private List<CanUseSkill> usingSkillList = new List<CanUseSkill>();
    private List<BuffView> effectingBuffList = new List<BuffView>();

    private Dictionary<uint, List<CanUseSkill>> SkillDict = new Dictionary<uint, List<CanUseSkill>>();
    private Dictionary<uint, List<BuffView>> BuffDict = new Dictionary<uint, List<BuffView>>();

    public static SkillManager Instance
    {
        get
        {
            if (instance == null)
                instance = new SkillManager();
            return instance;
        }
    }

    void UpdateUsingSkill()
    {
        for (int i = usingSkillList.Count - 1; i >= 0; i--)
        {
            if (usingSkillList[i].isUsing)
                usingSkillList[i].Update();
            else
            {
                DisUseSkill(usingSkillList[i]);
                usingSkillList.RemoveAt(i);
            }
        }
    }

   /// <summary>
   /// 回收技能
   /// </summary>
   /// <param name="Skill">技能</param>
    private void DisUseSkill(CanUseSkill Skill)
    {
        if (SkillDict.ContainsKey(Skill.SkillEffectData.id))
            SkillDict[Skill.SkillEffectData.id].Add(Skill);
    }
    /// <summary>
    /// 停止技能
    /// </summary>
    /// <param name="Skill">技能</param>
    public void StopSkill(CanUseSkill Skill)
    {
        if (Skill == null)
            return;
        Skill.StopSkill();
    }
    /// <summary>
    /// 重新使用技能
    /// </summary>
    /// <param name="Skill">技能</param>
    public void ReuseSkillToGo(CanUseSkill Skill)
    {
        Skill.StopSkill();
        Skill.PlaySkill();
    }

    /// <summary>
    /// 获取可复用技能
    /// </summary>
    /// <param name="SkillEffectId">技能id</param>
    /// <returns>技能</returns>
    private CanUseSkill GetSkill(uint SkillEffectId)
    {
        if (SkillDict.ContainsKey(SkillEffectId))
        {
            if (SkillDict[SkillEffectId].Count > 1)
            {
                CanUseSkill ReturnSkill = SkillDict[SkillEffectId][SkillDict[SkillEffectId].Count - 1];
                SkillDict[SkillEffectId].RemoveAt(SkillDict[SkillEffectId].Count - 1);
                return ReturnSkill;
            }
            if (SkillDict[SkillEffectId].Count > 0)
            {
                return new CanUseSkill(SkillDict[SkillEffectId][0]);
            }
            else
            {
                Debug.LogError("Skill Temp Is Missing Or No This Skill");
                return null;
            }
        }
        else
        {
            Debug.LogError("No This Skill");
            return null;
        }
    }
    /// <summary>
    /// 对某个物体使用技能
    /// </summary>
    /// <param name="SkillEffectId">技能id</param>
    /// <param name="User">技能使用者</param>
    /// <param name="TargetGo">技能目标</param>
    /// <returns></returns>
    public CanUseSkill UseSkillToGo(uint SkillEffectId, GameObject User, GameObject TargetGo)
    {
        if (User == null || TargetGo == null)
            return null;
        CanUseSkill skill = GetSkill(SkillEffectId);
        skill.UseSkillToGo(User, TargetGo);
        usingSkillList.Add(skill);
        return skill;
    }
    /// <summary>
    /// 对多个目标使用技能
    /// </summary>
    /// <param name="SkillEffectId">技能id</param>
    /// <param name="ls_Targets">目标列表</param>
    /// <param name="user">使用者</param>
    /// <returns></returns>
    public CanUseSkill UseSkillToGos(uint SkillEffectId, List<GameObject> ls_Targets, GameObject user = null)
    {
        if (ls_Targets == null)
        {
            return null;
        }

        CanUseSkill skill = GetSkill(SkillEffectId);

        skill.UseSkillToMultiGo(user, ls_Targets);

        usingSkillList.Add(skill);
        return skill;
    }
    /// <summary>
    /// 对某地使用技能
    /// </summary>
    /// <param name="SkillEffectId">技能效果id</param>
    /// <param name="User">使用者</param>
    /// <param name="TargetPos">目标地址</param>
    /// <returns>技能</returns>
    public CanUseSkill UseSkillToPos(uint SkillEffectId, GameObject User, Vector3 TargetPos)
    {
        if (User == null)
            return null;
        CanUseSkill skill = GetSkill(SkillEffectId);
        skill.UseSkillToPos(User, TargetPos);
        usingSkillList.Add(skill);
        return skill;
    }

    /// <summary>
    /// (暂时未启用)对某些地方使用技能
    /// </summary>
    /// <param name="SkillEffectId">效果id</param>
    /// <param name="User">使用者</param>
    /// <param name="TargetPoses">目标地址</param>
    /// <returns></returns>
    public CanUseSkill UseSkillToPoses(uint SkillEffectId, GameObject User, List<Vector3> TargetPoses)
    {
        return null;
    }

    public override List<IGetProgress> Init()
    {
        Dictionary<object, object> SkillDataDict = ConfigManager.Instance.GetConfig<SkillEffect>();
        foreach (KeyValuePair<object, object> skillEffect in SkillDataDict)
        {
            SkillEffect effectData = (SkillEffect)skillEffect.Value;
            if (effectData.id != 0)
            {
                if (!SkillDict.ContainsKey(effectData.id))
                {
                    SkillDict.Add(effectData.id, new List<CanUseSkill>());
                    SkillDict[effectData.id].Add(new CanUseSkill(effectData));
                }
            }
        }

        //KBEngine.Event.registerOut("sc_OnCut",this, "OnCutEffectShow");

        return null;
    }

    public override void OnDestroy()
    {

        usingSkillList.Clear();
        effectingBuffList.Clear();

        SkillDict.Clear();
        BuffDict.Clear();
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        UpdateUsingSkill();
    }
    public override void FixedUpdate()
    {
    }

    public override void onChangeSceneDone(emSceneStatus emStatus, emSceneStatus emLastStatus)
    {
        //throw new NotImplementedException();
    }
    /// <summary>
    /// (预留UI技能效果)可参考爆爆爆相关写法
    /// </summary>
    public void Show2DFightEffect()
    {

    }

    public override void onSetSceneDataDone(emSceneStatus curStatus, emSceneStatus preStatus)
    {
        //throw new NotImplementedException();
    }
}
