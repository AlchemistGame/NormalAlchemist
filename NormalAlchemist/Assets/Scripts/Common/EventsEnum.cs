public class EventsEnum
{
    /// <summary>
    /// 角色开始移动 ( 期间玩家无法操作 )
    /// </summary>
    public static string StartPlayerMove = "start_player_move";

    /// <summary>
    /// 角色移动结束 ( 玩家恢复正常操作 )
    /// </summary>
    public static string FinishPlayerMove = "finish_player_move";

    /// <summary>
    /// 角色开始攻击 ( 选中敌人攻击 )
    /// </summary>
    public static string StartPlayerAttack = "start_player_attack";

    /// <summary>
    /// 角色攻击动作结束
    /// </summary>
    public static string FinishPlayerAttack = "finish_player_attack";
}
