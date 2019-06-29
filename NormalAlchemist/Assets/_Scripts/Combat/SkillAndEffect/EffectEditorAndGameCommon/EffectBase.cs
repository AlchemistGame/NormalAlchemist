using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI.CoroutineTween;

public interface EffectBaseInterface
{
    void PlayEffect();
    void StopEffect();
}
public class EffectBase : MonoBehaviour, EffectBaseInterface
{
    //[HideInInspector]
    //[SerializeField]
    //private bool m_isTrack;

    //[HideInInspector]
    //[SerializeField]
    //private bool m_isFollow;

    //[SerializeField]
    //public bool isTrack
    //{
    //    get
    //    {
    //        return m_isTrack;
    //    }
    //    set
    //    {
    //        m_isTrack = value;
    //        // RefreshShownValue();
    //        if (value == true)
    //        {
    //            if (!m_TrackScript)
    //            {
    //                m_TrackScript = gameObject.AddComponent<CastEffect>();
    //            }
    //        }
    //        else
    //        {
    //            if (m_TrackScript)
    //            {
    //                DestroyImmediate(m_TrackScript);
    //            }
    //        }
    //    }
    //}

    //[SerializeField]
    //public bool isFollow
    //{
    //    get
    //    {
    //        return m_isFollow;
    //    }
    //    set
    //    {
    //        m_isFollow = value;
    //        // RefreshShownValue();
    //        if (value == true)
    //        {
    //            if (!m_TrackScript)
    //            {
    //                m_TrackScript = gameObject.AddComponent<CastEffect>();
    //            }
    //        }
    //        else
    //        {
    //            if (m_TrackScript)
    //            {
    //                //DestroyObject(m_LoopScript);
    //                DestroyImmediate(m_TrackScript);
    //            }
    //        }
    //    }
    //}
    //CastEffect m_TrackScript;
    //StaticEffect m_StaticScript;
    public Vector3 UsePos;
    public Vector3 TargetPos;
    public List<Vector3> MultiTargetPos;

    public GameObject UserGo;
    public GameObject TargetGo;
    public List<GameObject> MultiTargetGo;
    //public int LoopValue;
    public float LifeTime = 0;
    //public GameObject TrackTargetGo;
    //public float LoopSpacing;
    ParticleSystem[] ls_Particles;
    Animation[] ls_Animations;

    protected float startMoveMentTime;

    protected bool _isInit = false;
    protected bool _isPlaying = false;

    [SerializeField]
    protected bool _isLooping = false;

    protected Vector3 originalPos;

    public List<ParticleSystem> ls_underLoopControllParticle = new List<ParticleSystem>();
    public List<Animation> ls_underLoopControllAnim = new List<Animation>();
    public List<Animator> ls_underLoopControllAnimator = new List<Animator>();
    void Awake()
    {
        ls_Particles = GetComponentsInChildren<ParticleSystem>();
        ls_Animations = GetComponentsInChildren<Animation>();

        originalPos = transform.position;

        if (ls_underLoopControllAnim != null)
        {

        }

        if (ls_underLoopControllParticle != null)
        {
            foreach (var particle in ls_underLoopControllParticle)
            {
                ParticleSystem.MainModule mainParticle = particle.main;
                if (_isLooping)
                    mainParticle.loop = true;
                else
                    mainParticle.loop = false;
            }
        }

        if (ls_underLoopControllAnimator != null)
        {
            //foreach (var animator in ls_underLoopControllAnimator)
            //{
            //    foreach (var clip in animator.GetCurrentAnimatorClipInfo(0))
            //    {
            //        if (_isLooping)
            //            clip.clip.wrapMode = WrapMode.Loop;
            //    }
            //}
        }
    }

    public void PlayEffect()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        if (ls_Particles != null)
        {
            foreach (ParticleSystem particle in ls_Particles)
            {
                if (particle == null)
                    continue;
                if (particle.isPlaying)
                    particle.Stop();
                particle.Play();
            }
        }

        if (ls_Animations != null)
        {
            foreach (Animation anim in ls_Animations)
            {
                if (anim == null)
                    continue;
                if (anim.isPlaying)
                    anim.Stop();
                anim.Play();
            }
        }
        _isInit = false;
        _isPlaying = true;
    }

    public void StopEffect()
    {
        foreach (ParticleSystem particle in ls_Particles)
        {
            if (particle.isPlaying)
                particle.Stop();
        }
        foreach (Animation anim in ls_Animations)
        {
            if (anim.isPlaying)
                anim.Stop();
        }
        _isInit = false;
        _isPlaying = false;
        gameObject.SetActive(false);
    }

    public void PauseEffect()
    {
        foreach (ParticleSystem particle in ls_Particles)
        {
            if (particle.isPlaying)
                particle.Pause();
        }
        foreach (Animation anim in ls_Animations)
        {
            if (anim.isPlaying)
                anim.Stop();
        }
    }

    public void ContinueEffect()
    {
        foreach (ParticleSystem particle in ls_Particles)
        {
            if (!particle.isPlaying)
                particle.Play();
        }
        foreach (Animation anim in ls_Animations)
        {
            if (!anim.isPlaying)
                anim.Play();
        }

        _isPlaying = true;
    }

    public virtual void Init()
    {
        startMoveMentTime = Time.time;
        _isInit = true;
    }

    public virtual void Clear()
    {
        transform.position = originalPos;

        UsePos = Vector3.zero;
        TargetPos = Vector3.zero;
        MultiTargetPos = null;

        UserGo = null;
        TargetGo = null;
        MultiTargetGo = null;
        StopEffect();
    }

}
