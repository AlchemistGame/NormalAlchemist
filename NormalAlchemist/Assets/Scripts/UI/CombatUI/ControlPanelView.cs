using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelView : MonoBehaviour
{
    public Text curPlayerName;
    public Button attackBtn;
    public Button finishBtn;
    
	void Start ()
    {
        finishBtn.onClick.AddListener(() =>
        {
            ActorManager.Instance.FinishCurrentTurn();

            curPlayerName.text = ActorManager.Instance.GetCurActorName();
        });
	}
}
