using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enum_NeedLead_UI
{
    Null,
    MainUI,
    JoinFamily,
    Military_SquadSelect,
    Military_SquadEdit,
    DrawCard_Main,
    StateWar_WarSelect,
    StateWar_Room_StateSelect,
    MatchingUI,
    BattleUI,
    StoryFightUI,
    //InStateWar_CityDetail,
    FightingUI,
    FightingPrepareUI,
    OptionalGenerals,
    OptionalGenerals_LoverSelect,
    OptionalGenerals_GiftSelect,
    Sociality,

}

public enum Enum_NeedLead_Icon
{
    MainUI_DrawCard,
    MainUI_Military,
    MainUI_Battle,
    MainUI_StateWar,
    MainUI_Matching,
    MainUI_OptionalGenerals,
    MainUI_Sociality,
    MainUI_Property,
    MainUI_TaskBtn,

    DrawCard_TenDraw,
    DrawCard_Exit,

    Military_SquadSelect_FirstSquad,
    Military_SquadSelect_EditButton,
    Military_SquadSelect_SelectToFight,
    Military_SquadEdit_Strongest,
    Military_SquadEdit_Savesquad,

    InStateWar_OneCity,
    InStateWar_CityDetail_MoveTo,
    InStateWar_CityDetail_Recruit,
    InStateWar_MoveTo_SpeedUp,
    InStateWar_RecruitConfirm_RMBRecruit,

    Matching_StartNormalMatch,

    Fighting_ChiefTog,
    Fighting_EnemyTroop,
    Fighting_Skill_1,
    Fighting_SelfCampsite,
    Fighting_EnemyCampsite,
    Fighting_AutoFight,

    FightingPrepareUI_EndPrepare,

    StoryFightUI_Attack,
    StoryFightUI_Sweep,

    SelectStateWar_FirstStateWar,
    SelectStateWar_SingleApplication,
    SelectStateWar_Room_FirstState,
    SelectStateWar_Room_EnterWar,
    SelectStateWar_Room_JoinState,

    Battle_BattleBtn,
    Battle_StoryFightBtn,
    OptionalGenerals_AddLover,
    OptionalGenerals_CreateGeneral,
    OptionalGenerals_GiveGift,
    OptionalGenerals_LoverSelect_FirstGeneral,
    OptionalGenerals_GiftSelect_LastGift,
    OptionalGenerals_GiftSelect_Confirm,
    Sociality_ConfraternityTog,
}

public enum LockGuideType
{
    Null,
    Button,
    ThreeDTarget,
}

public interface INeedLead
{
    GameObject GetLeadTarget(string leadIcon, ref LockGuideType targetType);
    //bool isShouldLead();
    void ShowLeadSpecialContent();
    //void CallLead(int stepId);


}
