using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : BaseManager
{
    private static ViewManager instance = null;
    public static ViewManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ViewManager();
            }
            return instance;
        }
    }
    public override void Init()
    {
    }
    public override void Release()
    {
        Event.DeRegistAll(this);
    }
    public override void Update()
    {
    }
}
