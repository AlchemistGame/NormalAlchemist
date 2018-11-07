using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public static GameMgr Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        InitScene();
    }

    private void InitScene()
    {
        ACTOR_INFO player_info = new ACTOR_INFO();
        player_info.name = "Player";
        player_info.position = new Vector3(0.5f, 0, 0.5f);
        player_info.rotation = new Vector3(0, 0, 0);
        ActorManager.Instance.CreateActor(ACTOR_TYPE.PLAYER, player_info);

        ACTOR_INFO enemy_info = new ACTOR_INFO();
        enemy_info.name = "Enemy";
        enemy_info.position = new Vector3(0.5f, 0, 8.5f);
        enemy_info.rotation = new Vector3(0, 180, 0);
        ActorManager.Instance.CreateActor(ACTOR_TYPE.PLAYER, enemy_info);
    }

}
