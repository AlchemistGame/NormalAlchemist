using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static GameMain Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        InitScene();
    }

    private void InitScene()
    {
        ACTOR_INFO enemy_info = new ACTOR_INFO();
        enemy_info.name = "Enemy";
        enemy_info.position = new Vector3(0.5f, 0, 8.5f);
        enemy_info.rotation = new Vector3(0, 180, 0);
        ActorManager.Instance.CreateActor("Model/UnityChan", "Enemy", enemy_info);

        ACTOR_INFO player_info = new ACTOR_INFO();
        player_info.name = "Player";
        player_info.position = new Vector3(0.5f, 0, 0.5f);
        player_info.rotation = new Vector3(0, 0, 0);
        ActorManager.Instance.CreateActor("Model/UnityChan", "Friend", player_info);
    }

}
