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
        EventManager.Register(EventsEnum.StartPlayerMove, this, "ForBidPlayerOperation");
        EventManager.Register(EventsEnum.FinishPlayerMove, this, "RecoverPlayerOperation");
        EventManager.Register(EventsEnum.StartPlayerAttack, this, "ForBidPlayerOperation");
        EventManager.Register(EventsEnum.FinishPlayerAttack, this, "RecoverPlayerOperation");

        startGameBtn.onClick.AddListener(() =>
        {
            ActorManager.Instance.FinishCurrentTurn();
            curPlayerName.text = ActorManager.Instance.GetCurActorName();

            toCombatPanel.SetActive(false);
            inCombatPanel.SetActive(true);
        });

        attackBtn.onClick.AddListener(() =>
        {
            EventManager.Broadcast(EventsEnum.StartPlayerAttack);
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
