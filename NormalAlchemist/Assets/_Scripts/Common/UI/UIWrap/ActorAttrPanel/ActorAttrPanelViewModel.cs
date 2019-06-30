public class ActorAttrPanelViewModel : BaseModel
{

    private dynamic _actorname;
    public dynamic ActorName
    {
        get
        {
            return _actorname;
        }
        set
        {
            _actorname = value;
            TryCallHandle("ActorName", "CallBindStringToText", value);
        }
    }
    private dynamic _actorhp;
    public dynamic ActorHp
    {
        get
        {
            return _actorhp;
        }
        set
        {
            _actorhp = value;
            TryCallHandle("ActorHp", "CallBindStringToText", value);
        }
    }
    private dynamic _actorspd;
    public dynamic ActorSpd
    {
        get
        {
            return _actorspd;
        }
        set
        {
            _actorspd = value;
            TryCallHandle("ActorSpd", "CallBindStringToText", value);
        }
    }
    private dynamic _actormoverange;
    public dynamic ActorMoveRange
    {
        get
        {
            return _actormoverange;
        }
        set
        {
            _actormoverange = value;
            TryCallHandle("ActorMoveRange", "CallBindStringToText", value);
        }
    }
    private dynamic _actorattack;
    public dynamic ActorAttack
    {
        get
        {
            return _actorattack;
        }
        set
        {
            _actorattack = value;
            TryCallHandle("ActorAttack", "CallBindStringToText", value);
        }
    }
    private dynamic _actordef;
    public dynamic ActorDef
    {
        get
        {
            return _actordef;
        }
        set
        {
            _actordef = value;
            TryCallHandle("ActorDef", "CallBindStringToText", value);
        }
    }
}