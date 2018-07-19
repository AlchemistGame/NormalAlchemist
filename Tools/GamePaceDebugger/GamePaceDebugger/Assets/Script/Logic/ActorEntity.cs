using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class ActorEntity
{
    public ActorEntity(uint id){
        i_entityid = id;
    }

    private uint i_entityid;
    public uint EntityID
    {
        get
        {
            return i_entityid;
        }
    }
    Vector3 m_Position;
    public Vector3 Position
    {
        set
        {
            if (m_Position != value)
            {
                m_Position = value;
                CallMethod("set_Position",new object[] { value});
            }
        }
    }

    Vector2Int m_MapPoint;
    public Vector2Int MapPoint
    {
        set
        {
            if (m_MapPoint != value)
            {
                m_MapPoint = value;
                CallMethod("set_MapPoint",new object[] { value});
            }
        }
    }
    float m_fAttack;
    public float Attack
    {
        set
        {
            //if(Attack)
        }
    }
    float m_fSpeed;

    public enum emFaceTo
    {
        West,
        East,
        North,
        South,
    }
    ActorView v_BindView;
    HashSet<string> canCallMethods = new HashSet<string>();
    public bool BindView(ActorView actorView)
    {
        if (v_BindView != null)
        {
            return false;
        }
        else
        {
            v_BindView = actorView;
            MethodInfo[] methodInfo = v_BindView.GetType().GetMethods();
            foreach (var i in methodInfo)
            {
                if (i.IsPublic)
                    if (i.Name.Split('_')[0] == "set")
                    {
                        if (!canCallMethods.Contains(i.Name))
                        {
                            canCallMethods.Add(i.Name);
                        }
                    }
            }
            return true;
        }
    }

    private void CallMethod(string methodName, object[] value = null)
    {
        if(value == null)
        {
            value = new object[] { };
        }
        if (canCallMethods.Contains(methodName))
        {
            v_BindView.GetType().InvokeMember(methodName, BindingFlags.Public|BindingFlags.SetField|BindingFlags.SetProperty,null,v_BindView, value);
        }
    }

}

