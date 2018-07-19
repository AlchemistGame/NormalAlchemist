using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : BaseManager {
    private static ActorManager instance = null;
    public static ActorManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ActorManager();
            }
            return instance;
        }
    }

    private Dictionary<uint, ActorEntity> dict_Entity = new Dictionary<uint, ActorEntity>();
    private uint start_guid = 1;
    private ActorEntity CreateEntity()
    {
        ActorEntity ae = new ActorEntity(start_guid);
        dict_Entity.Add(start_guid, ae);
        start_guid++;
        return ae;
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
    
}
