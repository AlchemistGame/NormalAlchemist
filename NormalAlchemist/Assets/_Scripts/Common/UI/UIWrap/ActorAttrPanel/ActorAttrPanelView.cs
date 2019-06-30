using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActorAttrPanelView : BaseView, IBindView
{
#region WrapStart
    public new Dictionary<string, dynamic> ModelDefaultData = new Dictionary<string, dynamic>()
    {
         { "ActorName","" },
         { "ActorHp",0 },
         { "ActorSpd",0 },
         { "ActorMoveRange",0 },
         { "ActorAttack",0 },
         { "ActorDef",0 },
    };
    #endregion WrapEnd
}