using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ACTOR_TYPE
{
    PLAYER,
    NPC,
    MONSTER
}

public struct ACTOR_INFO
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
}

public class ActorManager : MonoBehaviour
{
    public static ActorManager Instance { get; private set; }

    private List<Actor> actorList;
    private int curActorIndex;  // 当前行动者的索引

    private void Awake()
    {
        Instance = this;

        Init();
    }

    private void Update()
    {
        foreach (var actor in actorList)
        {
            actor.OnUpdate();
        }
    }

    private void Init()
    {
        actorList = new List<Actor>();
        curActorIndex = 0;
    }

    public void CreateActor(ACTOR_TYPE type, ACTOR_INFO basic_info)
    {
        switch (type)
        {
            case ACTOR_TYPE.PLAYER:
                Player player = new Player(basic_info);
                player.AddToScene(Instantiate<GameObject>(Resources.Load<GameObject>("Model/UnityChan")));
                actorList.Add(player);
                break;
            case ACTOR_TYPE.NPC:
                break;
            case ACTOR_TYPE.MONSTER:
                break;
            default:
                break;
        }
    }

    public void FinishCurrentTurn()
    {
        actorList[curActorIndex].OnTurnEnd();

        curActorIndex++;
        if (curActorIndex >= actorList.Count)
        {
            curActorIndex = 0;
        }

        actorList[curActorIndex].OnTurnBegin();
    }

    public string GetCurActorName()
    {
        return actorList[curActorIndex].name;
    }
}
