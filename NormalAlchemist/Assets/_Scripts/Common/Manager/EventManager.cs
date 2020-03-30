using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 事件系统
/// </summary>
public class EventManager
{
    private struct sTypeMethod
    {
        public MethodInfo info;
        public object target;
    }

    private static Dictionary<string, List<sTypeMethod>> dict_FireMethod = new Dictionary<string, List<sTypeMethod>>();

    public static void Register(string registEvent, object target, string methodName)
    {
        try
        {
            MethodInfo methodInfo = target.GetType().GetMethod(methodName);
            List<sTypeMethod> ls_TypeMethod;

            if (methodInfo == null)
            {
                Debug.LogError("can't find specified method");
            }
            else
            {
                if (dict_FireMethod.TryGetValue(registEvent, out ls_TypeMethod))
                {
                    foreach (var i in ls_TypeMethod)
                    {
                        if (i.target == target && i.info.Name == methodName)
                        {
                            Debug.Log("Please Dont Register On Method Twice");
                            return;
                        }
                    }
                }
                else
                {
                    dict_FireMethod.Add(registEvent, new List<sTypeMethod>());
                    dict_FireMethod.TryGetValue(registEvent, out ls_TypeMethod);
                }

                sTypeMethod typeMethod = new sTypeMethod();
                typeMethod.info = methodInfo;
                typeMethod.target = target;
                ls_TypeMethod.Add(typeMethod);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public static void DeRegist(string registEvent, object target)
    {
        List<sTypeMethod> ls_TypeMethod;
        if (dict_FireMethod.TryGetValue(registEvent, out ls_TypeMethod))
        {
            for (int i = ls_TypeMethod.Count - 1; i >= 0; i--)
            {
                if (ls_TypeMethod[i].target == target)
                {
                    ls_TypeMethod.RemoveAt(i);
                }
            }
        }
    }

    public static void DeRegistAll(object target)
    {
        var iter = dict_FireMethod.GetEnumerator();
        while (iter.MoveNext())
        {
            List<sTypeMethod> list = iter.Current.Value;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].target == target)
                {
                    list.RemoveAt(i);
                }
            }
        }
    }

    public static void Broadcast(string registEvent, params object[] args)
    {
        List<sTypeMethod> ls_TypeMethod;
        if (dict_FireMethod.TryGetValue(registEvent, out ls_TypeMethod))
        {
            foreach (var i in ls_TypeMethod)
            {
                i.info.Invoke(i.target, args);
            }
        }
    }
}
