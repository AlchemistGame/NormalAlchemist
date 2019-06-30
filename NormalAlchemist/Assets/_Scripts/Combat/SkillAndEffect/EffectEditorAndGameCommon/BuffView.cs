using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct SBuffInfo
{
    public ushort tabID;
    public List<ushort> arrTime;
    //层数
    public byte u8Tier;
    public byte isPerpetual;
}

public struct Buff : IConfig
{
    public UInt16 id;
    public string Name;
    public string Intro;
    public string BuffIcon;
    public string BuffEffect;
    //叠加类型
    public byte OverlayType;
    public BuffType StateType;
}

public class BuffView
{
    private ushort _BuffId;
    private float _BuffLife;
    private byte _BuffCount;
    private BuffType _BuffType;
    private List<BuffLayer> ls_BuffLayers = new List<BuffLayer>();
    public delegate void BuffViewEvent(BuffView view);
    public BuffViewEvent OnDestroy;
    private bool _isPerpetual;
    private GameObject _BuffGo;
    public struct BuffLayer
    {
        public byte _layerId;
        public float _buffLife;
        public BuffLayer(byte layer, float layerLife)
        {
            _layerId = layer;
            _buffLife = layerLife;
        }
    }

    public BuffType buffType
    {
        get
        {
            return _BuffType;
        }
    }

    public ushort BuffId
    {
        get
        {
            return _BuffId;
        }
        set
        {
            _BuffId = value;
        }
    }
    public byte BuffCount
    {
        get
        {
            return _BuffCount;
        }
        set
        {
            _BuffCount = value;
        }
    }
    public float BuffLife
    {
        set
        {
            _BuffLife = value;
        }
        get
        {
            return _BuffLife;
        }
    }

    public bool isPerpetual
    {
        get
        {
            return _isPerpetual;
        }
    }

    public BuffView(GameObject go)
    {
        if (go == null)
        {
            Debug.LogWarning("Buff Go is Null");
        }
        _BuffGo = go;
    }


    public void UpdateBuffLayer(SBuffInfo buffInfo)
    {
        if (_BuffId == 0)
        {
            _BuffId = buffInfo.tabID;
            _BuffType = ConfigManager.Instance.GetConfig<Buff>(buffInfo.tabID).StateType;
            _isPerpetual = buffInfo.isPerpetual == 1;
        }
        else
        {
            if (_BuffId != buffInfo.tabID)
                return;
        }
        //ConfigManager.Instance.GetConfig<>
        _BuffCount = buffInfo.u8Tier;
        ls_BuffLayers.Clear();
        byte i = 1;
        if (buffInfo.arrTime != null)
            foreach (var info in buffInfo.arrTime)
            {
                ls_BuffLayers.Add(new BuffLayer(i, info));
                i++;
            }
        if (i > 1)
        {
            Play();
        }
    }

    public void Play()
    {
        if (_BuffGo != null)
            if (!_BuffGo.activeSelf)
                _BuffGo.SetActive(true);
    }
    public void Stop()
    {
        if (_BuffGo != null)
            if (_BuffGo.activeSelf)
                _BuffGo.SetActive(false);
    }

    // Update is called once per frame
    public void Update()
    {
        _BuffLife = 0;
        if (!_isPerpetual)
        {
            for (int i = ls_BuffLayers.Count - 1; i >= 0; i--)
            {
                if (ls_BuffLayers[i]._layerId > 0)
                {
                    if (ls_BuffLayers[i]._buffLife < 0)
                    {
                        ls_BuffLayers.RemoveAt(i);
                    }
                    else
                    {
                        ls_BuffLayers[i] = new BuffLayer(ls_BuffLayers[i]._layerId, ls_BuffLayers[i]._buffLife - Time.deltaTime);

                        if (ls_BuffLayers[i]._buffLife > _BuffLife)
                        {
                            _BuffLife = ls_BuffLayers[i]._buffLife;
                        }

                        if (ls_BuffLayers[i]._buffLife < 0)
                        {
                            ls_BuffLayers.RemoveAt(i);
                        }
                    }
                }
            }
        }
        else
        {
            _BuffLife = float.MaxValue;
        }

        if (_BuffLife > 0)
        {
            Play();
        }
        else
        {
            Stop();
            DetroyThis();
        }

        _BuffCount = (byte)ls_BuffLayers.Count;
        if (BuffCount <= 0 && !_isPerpetual)
        {
            Stop();
            DetroyThis();
        }
    }

    bool isToDestroy = false;
    void DetroyThis()
    {
        if (!isToDestroy)
        {
            if (OnDestroy != null)
            {
                OnDestroy(this);
            }
            isToDestroy = true;
        }
    }
    public void Release()
    {
        if (_BuffGo != null)
            GameObject.Destroy(_BuffGo);
    }
}
