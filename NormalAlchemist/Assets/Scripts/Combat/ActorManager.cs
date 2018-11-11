using System.Collections.Generic;
using UnityEngine;

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
    private Actor previousFrameHitActor;  // 上一帧鼠标所指向的 actor

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

        OnMouseUpdate();
    }

    private void Init()
    {
        actorList = new List<Actor>();
        curActorIndex = 0;
    }

    private void OnMouseUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
        {
            Actor curFrameHitActor = null;
            foreach (var actor in actorList)
            {
                if (actor.sceneObject == hit.collider.gameObject)
                {
                    curFrameHitActor = actor;
                }
            }

            if (curFrameHitActor != previousFrameHitActor)
            {
                if (previousFrameHitActor != null)
                {
                    previousFrameHitActor.OnMouseExit();
                }

                if (curFrameHitActor != null)
                {
                    curFrameHitActor.OnMouseEnter();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (curFrameHitActor != null)
                {
                    curFrameHitActor.OnMouseClick();
                }
            }

            previousFrameHitActor = curFrameHitActor;
        }
        else
        {
            if (previousFrameHitActor != null)
            {
                previousFrameHitActor.OnMouseExit();
            }
            previousFrameHitActor = null;
        }
    }


    public void CreateActor(string model_path, string tag, ACTOR_INFO basic_info)
    {
        Player player = new Player(basic_info);
        GameObject modelObject = Instantiate<GameObject>(Resources.Load<GameObject>(model_path));
        modelObject.tag = tag;
        player.AddToScene(modelObject);
        actorList.Add(player);
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
