using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : BaseManager
{
    private static MapManager instance = null;
    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MapManager();
            }
            return instance;
        }
    }

    public override void Init()
    {
    }

    public override void Release()
    {
    }

    public override void Update()
    {
    }
    public void EnterMap(int x,int y)
    {

    }
    
}
