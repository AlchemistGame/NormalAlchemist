using UnityEngine;
using UnityEngine.UI;

public class ControlPanelView : MonoBehaviour
{
    public GameObject toCombatPanel;
    public Button startGameBtn;
    public GameObject inCombatPanel;
    public Text curPlayerName;
    public Button attackBtn;
    public Button finishBtn;

    private void Awake()
    {
        inCombatPanel.SetActive(false);
        toCombatPanel.SetActive(true);
    }

    private void Start()
    {
        Event.Register(EventsEnum.StartPlayerMove, this, "ForBidPlayerOperation");
        Event.Register(EventsEnum.FinishPlayerMove, this, "RecoverPlayerOperation");
        Event.Register(EventsEnum.StartPlayerAttack, this, "ForBidPlayerOperation");
        Event.Register(EventsEnum.FinishPlayerAttack, this, "RecoverPlayerOperation");

        startGameBtn.onClick.AddListener(() =>
        {
            ActorManager.Instance.FinishCurrentTurn();
            curPlayerName.text = ActorManager.Instance.GetCurActorName();

            toCombatPanel.SetActive(false);
            inCombatPanel.SetActive(true);
        });

        attackBtn.onClick.AddListener(() =>
        {
            Event.Broadcast(EventsEnum.StartPlayerAttack);
        });

        finishBtn.onClick.AddListener(() =>
        {
            ActorManager.Instance.FinishCurrentTurn();

            curPlayerName.text = ActorManager.Instance.GetCurActorName();
        });
    }

    public void ForBidPlayerOperation()
    {
        attackBtn.interactable = false;
        finishBtn.interactable = false;
    }

    public void RecoverPlayerOperation()
    {
        attackBtn.interactable = true;
        finishBtn.interactable = true;
    }
}
