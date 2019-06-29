using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public enum emTrackType
{
    emTrackType_Direction,
    emTrackType_Static,
}

public struct SkillEffect : IConfig
{
    public UInt32 id;

    public string SkillEffectName;

    public UInt16 StartEffectId;
    public double StartDelay;

    public UInt16 TrackEffectId;
    public double TrackDelay;
    public byte isTrace;
    public byte isStatic;
    public byte isDestroyOnHit;

    public UInt16 HitEffectId;
    public double HitDelay;

    public double SkillEffectLife;
    public byte isPointToGo;
    public Vector3 UseOffset;
    public override bool Equals(object obj)
    {
        if (obj is SkillEffect)
            return id == ((SkillEffect)obj).id;

        return false;
    }

    public static bool operator ==(SkillEffect src, SkillEffect tager)
    {
        return src.id == tager.id;
    }
    public static bool operator !=(SkillEffect src, SkillEffect tager)
    {
        return src.id != tager.id;
    }
}

public struct Effect : IConfig
{
    public UInt32 id;

    public string EffectName;
    /// <summary>
    /// -1(一直),0(不循环),n（循环n次）
    /// </summary>
    public int LoopValue;
    /// <summary>
    /// 循环间隔
    /// </summary>
    public double LoopOffset;
    public double Delay;
    public double Scale;
    public UnityEngine.Vector3 StartPosOffset;

    public bool isFollow;

    //0为不摇动
    public byte _ShakeType
    {
        get
        {
            switch (ShakeType)
            {
                case "Normal":
                    return (byte)emShakeType.emShakeType_Normal;
                case "Horizontal":
                    return (byte)emShakeType.emShakeType_Horizontal;
                case "Vertical":
                    return (byte)emShakeType.emShakeType_Vertical;
                default:
                    return (byte)emShakeType.emShakeType_None;
            }
        }

    }
    public string ShakeType;
    public double ShakeKeepTime;
    public double ShakeStartTime;
    public double ShakePower;

    public double LifeTime;

    /// <summary>
    /// 特效Prefeb名称，必填
    /// </summary>
    public string Model;
    /// <summary>
    ///特效音效名，没有则填不填
    /// </summary>
    public string Sound;

    // public 

    public override bool Equals(object obj)
    {
        if (obj is Effect)
            return id == ((Effect)obj).id;

        return false;
    }

    public static bool operator ==(Effect src, Effect tager)
    {
        return src.id == tager.id;
    }
    public static bool operator !=(Effect src, Effect tager)
    {
        return src.id != tager.id;
    }
}

public class CanUseSkill
{
    public List<GameObject> _SkillEffects = new List<GameObject>();

    public List<GameObject> _SkillTrace = new List<GameObject>();

    private SkillEffect _SkillEffectData;

    //TODO:改成对应的Pool
    public static dynamic EffectPool;

    private List<GameObject> _SkillStartEffects = new List<GameObject>();
    public GameObject _StartEffectPref;
    private double _StartEffectId;
    public double _StartEffectDelay;
    public double _StartEffectLife;
    public bool _EndStart = false;

    private List<GameObject> _SkillTrackEffects = new List<GameObject>();
    public GameObject _TrackEffectPref;
    private double _TrackEffectId;
    public double _TrackEffectDelay;
    public double _TrackEffectLife;
    public emTrackType _TrackType = emTrackType.emTrackType_Direction;
    public bool _EndTrack = false;
    public bool _EndFly = false;

    private List<GameObject> _SkillHitEffects = new List<GameObject>();
    public GameObject _HitEffectPref;
    private double _HitEffectId;
    public double _HitEffectDelay;
    public double _HitEffectLife;
    public bool _EndHit = false;

    public bool _isUsing = false;

    public float _usingTime = -1;

    public bool _isPointToGo = true;

    private GameObject _User;

    private GameObject _TargetGo;

    private Vector3 _TargetPos;
    private Vector3 _UsePos;

    private List<GameObject> _MultiTargetGo;
    private List<Vector3> _MultiTargetPos;

    public List<GameObject> Targets
    {
        get
        {
            if (_isPointToGo && _TargetGo != null)
            {
                return new List<GameObject>() { _TargetGo };
            }
            else
            {
                if (_isPointToGo && _TargetGo == null)
                {
                    if (_MultiTargetGo != null)
                    {
                        if (_MultiTargetGo.Count > 1)
                            return _MultiTargetGo;
                    }
                }
            }

            return null;
        }
    }

    //命中边缘触发命中特效，体积偏移量
    float HitRange = 0.2f;

    public SkillEffect SkillEffectData
    {
        get
        {
            return _SkillEffectData;
        }
    }

    public bool isUsing
    {
        get
        {
            return _isUsing;
        }
    }

    public CanUseSkill(SkillEffect skillEffectData)
    {
        _SkillEffectData = skillEffectData;
        BuildSkill();
    }

    public CanUseSkill(CanUseSkill skillTemp)
    {
        _SkillEffectData = skillTemp.SkillEffectData;
        BuildSkill();
    }

    public void SetSkillData(SkillEffect skillEffectData)
    {
        Clear();
        _SkillEffectData = skillEffectData;
        BuildSkill();
    }

    // Use this for initialization
    public void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {

        if (_isUsing)
        {

            if (_SkillEffectData.SkillEffectLife > 0)
            {
                if (_usingTime + _SkillEffectData.SkillEffectLife >= Time.time)
                    StopSkill();
                return;
            }

            if (_StartEffectPref && _EndStart == false)
                if (_SkillEffectData.StartDelay + _usingTime <= Time.time)
                {
                    OnStart();
                }
            if (_TrackEffectPref && _EndTrack == false)
                if (_SkillEffectData.TrackDelay + _usingTime <= Time.time)
                {
                    OnTrack();
                }

            //没有轨迹效果或者为指向性非追踪技能 则通过判断延时播放命中效果，否则通过命中播放命中效果
            if ((!_TrackEffectPref) || (_SkillEffectData.isTrace == 0 && (_TargetGo != null || _TargetPos != null)))
            {
                if ( _EndHit == false)
                {
                    if (_SkillEffectData.HitDelay + _usingTime <= Time.time)
                    {
                        if (_TargetGo)
                        {
                            OnHit(_TargetGo);
                        }
                        else
                        {
                            if (_MultiTargetGo != null)
                            {
                                OnHit(_MultiTargetGo);
                            }
                            else
                            {
                                if (_MultiTargetPos != null)
                                {
                                    foreach (var pos in _MultiTargetPos)
                                    {
                                        OnHit(pos);
                                    }
                                }
                                else
                                {
                                    OnHit(_TargetPos);
                                }
                            }
                        }

                    }
                }
            }

            if (_SkillEffectData.isTrace == 1 && _SkillEffectData.isStatic == 0 && !_EndFly)
                UpdateTrack();
        }
    }

    List<Action> HitInfo = null;
    public virtual void UseSkillToMultiGo(GameObject User, List<GameObject> MultiTarget, List<Action> ls_OnSkillHitInfo = null)
    {
        _User = User;
        _TargetGo = null;
        UseSkillToMultiGo(MultiTarget, ls_OnSkillHitInfo);
    }

    public virtual void UseSkillToMultiGo(List<GameObject> MultiTarget,List<Action> ls_OnSkillHitInfo = null)
    {
        if (_SkillEffectData.id == 0)
            return;
        StopSkill();
        ResetSkillToFire();

        HitInfo = ls_OnSkillHitInfo;
        _usingTime = Time.time;
        _isUsing = true;
        _MultiTargetGo = MultiTarget;
        _isPointToGo = true;
        PlaySkill();
    }

    public virtual void UseSkillToPos(GameObject User, Vector3 TargetPos, List<Action> ls_OnSkillHitInfo = null)
    {
        _User = User;
        UseSkillToPos(TargetPos, ls_OnSkillHitInfo);
    }

    public virtual void UseSkillToPos(Vector3 TargetPos, List<Action> ls_OnSkillHitInfo = null)
    {
        if (_SkillEffectData.id == 0)
            return;

        StopSkill();
        ResetSkillToFire();

        HitInfo = ls_OnSkillHitInfo;
        _usingTime = Time.time;
        _isUsing = true;
        _TargetPos = TargetPos;
        _isPointToGo = false;
        PlaySkill();
    }

    public virtual void UseSkillToGo(GameObject User, GameObject TargetGo, List<Action> ls_OnSkillHitInfo = null)
    {
        _User = User;
        _MultiTargetGo = null;
        UseSkillToGo(TargetGo, ls_OnSkillHitInfo);
    }

    public virtual void UseSkillToGo(GameObject TargetGo, List<Action> ls_OnSkillHitInfo = null)
    {
        if (_SkillEffectData.id == 0)
            return;

        StopSkill();
        ResetSkillToFire();

        HitInfo = ls_OnSkillHitInfo;
        _usingTime = Time.time;
        _isUsing = true;
        _TargetGo = TargetGo;
        _isPointToGo = true;
        PlaySkill();
    }

    //TODO 在使用技能中减少技能的ReBuild改为停止和使用
    private void BuildSkill()
    {
        if (_SkillEffectData.id == 0)
            return;

        if (SkillEffectData.isStatic == 1)
            _TrackType = emTrackType.emTrackType_Static;
        else
            _TrackType = emTrackType.emTrackType_Direction;

        if (_SkillEffectData.HitEffectId != 0)
            BuildHitEffect();
        if (_SkillEffectData.StartEffectId != 0)
            BuildStartEffect();
        if (_SkillEffectData.TrackEffectId != 0)
            BuildTrackEffect();
    }

    public virtual void StopSkill()
    {
        for (int i = _SkillStartEffects.Count - 1; i >= 0; i--)
        {
            if (_SkillStartEffects[i] == null)
            {
                _SkillStartEffects.RemoveAt(i);
                continue;
            }

            if (_SkillStartEffects[i])
            {
                _SkillStartEffects[i].GetComponent<EffectBase>().StopEffect();

                if (_SkillStartEffects[i].activeInHierarchy)
                {
                    _SkillStartEffects[i].SetActive(false);
                    _SkillStartEffects[i].transform.position = Vector3.zero;
                }
            }
        }

        for (int i = _SkillTrackEffects.Count - 1; i >= 0; i--)
        {
            if (_SkillTrackEffects[i] == null)
            {
                _SkillTrackEffects.RemoveAt(i);
                continue;
            }
            if (_SkillTrackEffects[i])
            {
                _SkillTrackEffects[i].GetComponent<EffectBase>().StopEffect();
                _SkillTrackEffects[i].transform.position = Vector3.zero;
                if (_SkillTrackEffects[i].activeInHierarchy)
                    _SkillTrackEffects[i].SetActive(false);
            }
        }

        for (int i = _SkillHitEffects.Count - 1; i >= 0; i--)
        {
            if (_SkillHitEffects[i] == null)
            {
                _SkillHitEffects.RemoveAt(i);
                continue;
            }

            if (_SkillHitEffects[i])
            {
                _SkillHitEffects[i].GetComponent<EffectBase>().StopEffect();
                _SkillHitEffects[i].transform.position = Vector3.zero;
                if (_SkillHitEffects[i].activeInHierarchy)
                    _SkillHitEffects[i].SetActive(false);
            }
        }

        _isUsing = false;
    }

    public void RemoveSkillEffect()
    {
        ClearEffect(_SkillStartEffects);
        ClearEffect(_SkillHitEffects);
        ClearEffect(_SkillTrackEffects);
    }


    public void Clear()
    {
        RemoveSkillEffect();
        _isPointToGo = true;
        _TargetGo = null;
        _TargetPos = Vector3.zero;

        ResetSkillToFire();
        _HitEffectPref = null;
        _TrackEffectPref = null;
        _StartEffectPref = null;

        _isUsing = false;
    }

    public void PlaySkill()
    {
        for (int i = _SkillStartEffects.Count - 1; i >= 0; i--)
        {
            if (_SkillStartEffects[i] == null)
            {
                _SkillStartEffects.RemoveAt(i);
                continue;
            }

            if (_SkillStartEffects[i])
                _SkillStartEffects[i].GetComponent<EffectBase>().PlayEffect();

            if (!_SkillStartEffects[i].activeInHierarchy)
                _SkillStartEffects[i].SetActive(true);
        }
        for (int i = _SkillTrackEffects.Count - 1; i >= 0; i--)
        {
            if (_SkillTrackEffects[i] == null)
            {
                _SkillTrackEffects.RemoveAt(i);
                continue;
            }

            if (_SkillTrackEffects[i])
                _SkillTrackEffects[i].GetComponent<EffectBase>().PlayEffect();

            if (!_SkillTrackEffects[i].activeInHierarchy)
                _SkillTrackEffects[i].SetActive(true);
        }
        for (int i = _SkillHitEffects.Count - 1; i >= 0; i--)
        {
            if (_SkillHitEffects[i] == null)
            {
                _SkillHitEffects.RemoveAt(i);
                continue;
            }

            if (_SkillHitEffects[i])
                _SkillHitEffects[i].GetComponent<EffectBase>().PlayEffect();

            if (!_SkillHitEffects[i].activeInHierarchy)
                _SkillHitEffects[i].SetActive(true);
        }

        _isUsing = true;
    }

    public void ResetSkillToFire()
    {
        _EndHit = false;
        _EndStart = false;
        _EndTrack = false;
    }

    #region 起手效果相关
    void BuildStartEffect()
    {
        if (_SkillEffectData.StartEffectId == 0)
        {
            _StartEffectPref = null;
            _StartEffectId = 0;
            return;
        }
        _EndStart = false;
        if (_StartEffectPref != null && _StartEffectId == _SkillEffectData.StartEffectId)
        {
            return;
        }
        _StartEffectPref = ResourceManager.Instance.GetResource(emAssetBundle.Bundle_SkillEffect, ConfigManager.Instance.GetConfig<Effect>((uint)_SkillEffectData.StartEffectId).Model) as GameObject;

        //_StartEffectPref = ResourceManager.Instance.GetEffectByName(global_ConstDataDefine.EffectPath.MagicalEffectPath, ConfigManager.Instance.GetConfig<Effect>((uint)_SkillEffectData.StartEffectId).Model);

        _StartEffectId = _SkillEffectData.StartEffectId;
    }

    public virtual void OnStart()
    {
        GameObject StartEffectGo;

        if (!_StartEffectPref)
            return;

        if (_User)
        {
            StartEffectGo = EffectPool.createPrefabByPool(_StartEffectPref.transform, _StartEffectPref.transform.position, _StartEffectPref.transform.rotation, _User.transform).gameObject;
            if (_SkillEffectData.UseOffset != Vector3.zero)
            {
                StartEffectGo.transform.localPosition = _SkillEffectData.UseOffset/_User.transform.localScale.x;
            }
            //StartEffectGo = GameObject.Instantiate(_StartEffectPref, _User.transform, false) as GameObject;
        }
        else
        {
            StartEffectGo = EffectPool.createPrefabByPool(_StartEffectPref.transform, _StartEffectPref.transform.position, _StartEffectPref.transform.rotation).gameObject;
            //StartEffectGo = GameObject.Instantiate(_StartEffectPref) as GameObject;
        }
        _EndStart = true;

        if (_SkillStartEffects.Count > 0)
            ClearEffect(_SkillStartEffects);

        _SkillStartEffects.Add(StartEffectGo);

        EffectBase effectComp = StartEffectGo.GetComponent<EffectBase>();
        effectComp.TargetGo = _TargetGo;
        effectComp.TargetPos = _TargetPos;
        effectComp.UsePos = _UsePos;
        effectComp.UserGo = _User;
        effectComp.MultiTargetGo = _MultiTargetGo;
        effectComp.MultiTargetPos = _MultiTargetPos;
        effectComp.PlayEffect();
        //if (_User)
        //{
        //    StartEffectGo = GameObject.Instantiate(_StartEffect, _User.transform, false) as GameObject;
        //}
        //else
        //{
        //    StartEffectGo = GameObject.Instantiate(_StartEffect) as GameObject;
        //}

    }

    //TODO 修改为通过pool删除
    private void ClearEffect(List<GameObject> EffectList)
    {
        for (int i = EffectList.Count - 1; i >= 0; i--)
        {
            if (EffectList[i])
            {
                if (EffectList[i].GetComponent<EffectBase>() != null)
                {
                    EffectList[i].GetComponent<EffectBase>().Clear();
                    Debug.Log("Clear");
                }
                CanUseSkill.EffectPool.DestoryPrefabByPool(EffectList[i].transform);
            }
            EffectList.RemoveAt(i);
        }
    }
    #endregion

    #region 轨迹效果相关

    public virtual void OnTrack()
    {
        GameObject TrackEffectGo;

        _EndFly = false;
        if (!_TrackEffectPref)
            return;

        if (!_isPointToGo)
        {
            if (_TargetPos == null)
                return;
            switch (_TrackType)
            {
                //静止类型，即生成后轨迹特效实质是静止在空间中的
                case emTrackType.emTrackType_Static:
                    TrackEffectGo = EffectPool.createPrefabByPool(_TrackEffectPref.transform, _TargetPos, _TrackEffectPref.transform.rotation).gameObject;
                    //TrackEffectGo = GameObject.Instantiate(_TrackEffectPref, _TargetPos, _TrackEffectPref.transform.rotation) as GameObject;
                    //静止Effect脚本中自行锁定目标
                    //TrackEffectGo.transform.forward = (_TargetPos - TrackEffectGo.transform.position).normalized;
                    break;
                //运动类型,即生成后对象实质是在空间中运动的
                case emTrackType.emTrackType_Direction:
                    if (!_User)
                        return;
                    TrackEffectGo = EffectPool.createPrefabByPool(_TrackEffectPref.transform, _User.transform.position + _SkillEffectData.UseOffset, _TrackEffectPref.transform.rotation).gameObject;
                    
                    //TrackEffectGo = GameObject.Instantiate(_TrackEffectPref, _User.transform.position + _User.transform.forward, _TrackEffectPref.transform.rotation) as GameObject;
                    _SkillTrace.Add(TrackEffectGo);
                    break;
                default:
                    return;
            }

            //向特效自身管理类传递目标数据
        }
        else
        {
            if (_TargetGo == null)
                return;
            switch (_TrackType)
            {
                case emTrackType.emTrackType_Direction:
                    if (!_User)
                        return;
                    TrackEffectGo = EffectPool.createPrefabByPool(_TrackEffectPref.transform, _User.transform.position +_SkillEffectData.UseOffset, _TrackEffectPref.transform.rotation).gameObject;

                    //TrackEffectGo = GameObject.Instantiate(_TrackEffectPref, _User.transform.position + (Vector3.up * 0.5f) + _User.transform.forward, _TrackEffectPref.transform.rotation) as GameObject;
                    _SkillTrace.Add(TrackEffectGo);
                    TrackEffectGo.transform.forward = (_TargetGo.transform.position - TrackEffectGo.transform.position).normalized;
                    break;
                case emTrackType.emTrackType_Static:
                    TrackEffectGo = EffectPool.createPrefabByPool(_TrackEffectPref.transform, _TargetGo.transform.position, _TrackEffectPref.transform.rotation, _TargetGo.transform).gameObject;
                    //TrackEffectGo = GameObject.Instantiate(_TrackEffectPref, _TargetGo.transform.position, _TrackEffectPref.transform.rotation, _TargetGo.transform) as GameObject;
                    break;
                default:
                    return;
            }
        }
        //向特效自身管理类传递目标数据
        EffectBase effectComp = TrackEffectGo.GetComponent<EffectBase>();
        effectComp.TargetGo = _TargetGo;
        effectComp.TargetPos = _TargetPos;
        effectComp.UsePos = _UsePos;
        effectComp.UserGo = _User;
        effectComp.MultiTargetGo = _MultiTargetGo;
        effectComp.MultiTargetPos = _MultiTargetPos;
        effectComp.PlayEffect();

        if (_SkillTrackEffects.Count > 0)
            ClearEffect(_SkillTrackEffects);
        _SkillTrackEffects.Add(TrackEffectGo);

        _EndTrack = true;
    }

    public virtual void UpdateTrack()
    {
        if (_SkillEffectData.isTrace == 0)
            return;
        if (!_TargetGo)
            return;
        for (int i = _SkillTrace.Count - 1; i >= 0; i--)
        {
            GameObject go = _SkillTrace[i];
            if (go == null)
            {
                _SkillTrace.RemoveAt(i);
                continue;
            }
            Vector3 dir = _TargetGo.transform.position - go.transform.position;
            if (dir.sqrMagnitude <= HitRange * HitRange)
            {
                if (_SkillEffectData.isDestroyOnHit == 1)
                {
                    if (go.GetComponent<EffectBase>() != null)
                    {
                        go.GetComponent<EffectBase>().Clear();
                        Debug.Log("Clear");
                    }
                    CanUseSkill.EffectPool.DestoryPrefabByPool(go.transform);
                    _SkillTrace.RemoveAt(i);
                }
                if (_HitEffectPref)
                    OnHit(_TargetGo);
            }
            if (go)
                go.transform.forward = dir.normalized;
        }
    }

    void BuildTrackEffect()
    {
        if (_SkillEffectData.TrackEffectId == 0)
        {
            _TrackEffectPref = null;
            _TrackEffectId = 0;
            return;
        }
        _EndTrack = false;
        if (_TrackEffectPref != null && _TrackEffectId == _SkillEffectData.TrackEffectId)
        {
            return;
        }
        _TrackEffectPref = ResourceManager.Instance.GetResource(emAssetBundle.Bundle_SkillEffect, ConfigManager.Instance.GetConfig<Effect>((uint)_SkillEffectData.TrackEffectId).Model) as GameObject;
        //_TrackEffectPref = ResourceManager.Instance.GetEffectByName(global_ConstDataDefine.EffectPath.MagicalEffectPath, ConfigManager.Instance.GetConfig<Effect>((uint)_SkillEffectData.TrackEffectId).Model);
        _TrackEffectId = _SkillEffectData.TrackEffectId;
    }

    #endregion

    #region 命中效果相关

    public delegate void OnHitCallBackHandle();
    public OnHitCallBackHandle OnHitCallBackSender;
    public void OnHitCallBack()
    {
        int Atker = 0;
        if (HitInfo != null)
            foreach (Action info in HitInfo)
            {
                info();
            }
        HitInfo = null;
    }

    void BuildHitEffect()
    {
        if (_SkillEffectData.HitEffectId == 0)
        {
            _HitEffectPref = null;
            _HitEffectId = 0;
            return;
        }
        _EndHit = false;
        if (_HitEffectPref != null && _HitEffectId == _SkillEffectData.HitEffectId)
        {
            return;
        }
        //GameObject effectPref = ResourceManager.Instance.GetEffectByName(global_ConstDataDefine.EffectPath.MagicalEffectPath, ConfigManager.Instance.GetConfig<Effect>((uint)_SkillEffectData.HitEffectId).Model);

        GameObject effectPref = ResourceManager.Instance.GetResource(emAssetBundle.Bundle_SkillEffect, ConfigManager.Instance.GetConfig<Effect>((uint)_SkillEffectData.HitEffectId).Model) as GameObject;

        if (effectPref != null)
            _HitEffectPref = effectPref;
        _HitEffectId = _SkillEffectData.HitEffectId;
    }

    public virtual void OnHit(List<GameObject> ls_HitGo)
    {
        GameObject HitEffectGo;
        //if (OnHitCallBack != null)
        OnHitCallBack();
        
        _EndHit = true;
        _EndFly = true;

        if (!_HitEffectPref)
            return;
        if (ls_HitGo == null)
            return;

        if (_SkillHitEffects.Count > 0)
            ClearEffect(_SkillHitEffects);

        foreach (var go in ls_HitGo)
        {
            HitEffectGo = EffectPool.createPrefabByPool(_HitEffectPref.transform, _HitEffectPref.transform.position, _HitEffectPref.transform.rotation, go.transform).gameObject;
            //HitEffectGo = GameObject.Instantiate(_HitEffectPref, go.transform, false) as GameObject;
            _SkillHitEffects.Add(HitEffectGo);

            EffectBase effectComp = HitEffectGo.GetComponent<EffectBase>();
            effectComp.TargetGo = _TargetGo;
            effectComp.TargetPos = _TargetPos;
            effectComp.UsePos = _UsePos;
            effectComp.UserGo = _User;
            effectComp.MultiTargetGo = _MultiTargetGo;
            effectComp.MultiTargetPos = _MultiTargetPos;
            effectComp.PlayEffect();
        }
    }

    public virtual void OnHit(GameObject HitGo)
    {
        GameObject HitEffectGo;
        //if (OnHitCallBack != null)
        OnHitCallBack();

        _EndHit = true;
        _EndFly = true;

        if (!_HitEffectPref)
            return;
        if (HitGo == null)
            return;
        HitEffectGo = EffectPool.createPrefabByPool(_HitEffectPref.transform, _HitEffectPref.transform.position, _HitEffectPref.transform.rotation, HitGo.transform).gameObject;
        //HitEffectGo = GameObject.Instantiate(_HitEffectPref, HitGo.transform, false) as GameObject;

        if (_SkillHitEffects.Count > 0)
            ClearEffect(_SkillHitEffects);
        _SkillHitEffects.Add(HitEffectGo);

        EffectBase effectComp = HitEffectGo.GetComponent<EffectBase>();
        effectComp.TargetGo = _TargetGo;
        effectComp.TargetPos = _TargetPos;
        effectComp.UsePos = _UsePos;
        effectComp.UserGo = _User;
        effectComp.MultiTargetGo = _MultiTargetGo;
        effectComp.MultiTargetPos = _MultiTargetPos;
        effectComp.PlayEffect();

        //_EndHit = true;
        //_EndFly = true;
    }

    public virtual void OnHit(Vector3 HitPos)
    {
        GameObject HitEffectGo;
        //if (OnHitCallBack != null)
        OnHitCallBack();
        
        _EndHit = true;
        _EndFly = true;

        if (!_HitEffectPref)
            return;
        HitEffectGo = EffectPool.createPrefabByPool(_HitEffectPref.transform, HitPos, _HitEffectPref.transform.rotation).gameObject;
        //HitEffectGo = GameObject.Instantiate(_HitEffectPref, HitPos, _HitEffectPref.transform.rotation) as GameObject;

        if (_SkillHitEffects.Count > 0)
            ClearEffect(_SkillHitEffects);
        _SkillHitEffects.Add(HitEffectGo);

        EffectBase effectComp = HitEffectGo.GetComponent<EffectBase>();
        effectComp.TargetGo = _TargetGo;
        effectComp.TargetPos = _TargetPos;
        effectComp.UsePos = _UsePos;
        effectComp.UserGo = _User;
        effectComp.MultiTargetGo = _MultiTargetGo;
        effectComp.MultiTargetPos = _MultiTargetPos;
        effectComp.PlayEffect();
        
    }
    #endregion 
}
