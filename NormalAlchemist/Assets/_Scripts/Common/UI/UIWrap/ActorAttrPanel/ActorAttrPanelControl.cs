using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAttrPanelControl : BaseControll
{
    ActorAttrPanelViewModel _Model;
    public void Start()
    {
        _Model = SetModel<ActorAttrPanelViewModel>();
        _Model.ActorName = "¡∂Ω ı ø";
        _Model.ActorSpd = 10;
        _Model.ActorMoveRange = 5;
        _Model.ActorHp = 100;
        _Model.ActorAttack = 10;
        _Model.ActorDef = 2;
    }
}