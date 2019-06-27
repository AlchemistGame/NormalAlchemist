using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public struct ActorInfo
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
}

public delegate void UpdateEventHandler();

public class ActorManager : StateMachine
{
    // 当前行动者
    [HideInInspector]
    public Actor currentActor;
    public event UpdateEventHandler OnUpdate;
    [HideInInspector]
    public Vector3 targetPosition;
    public GameObject selectedBlockGO;
    public Button moveBtn;
    public Button attackBtn;
    public Button finishBtn;
    public static ActorManager Instance { get; private set; }

    private TurnOrderController turnOrderController;
    private IEnumerator turnOrderEnumerator;

    private void Awake()
    {
        Instance = this;

        turnOrderController = new TurnOrderController();
        turnOrderEnumerator = turnOrderController.Tick();
    }

    private void Update()
    {
        if (OnUpdate != null)
        {
            OnUpdate();
        }
    }

    public void CreateActor(string model_path, string tag, ActorInfo basic_info)
    {
        GameObject modelObject = Instantiate<GameObject>(Resources.Load<GameObject>(model_path));
        Player player = modelObject.AddComponent<Player>();
        player.Init(basic_info);
        modelObject.tag = tag;
        turnOrderController.AddActor(player);
    }

    public void NextTurn()
    {
        turnOrderEnumerator.MoveNext();
        currentActor = (Actor)turnOrderEnumerator.Current;
        ChangeState<PreTurnState>();
    }
}
