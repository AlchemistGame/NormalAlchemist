using System;
using UnityEngine;

using System.Collections.Generic;

namespace AssetBundleDefine
{
    #region AssetBundleDefine
    //TODO 在此处添加Bundle包路径

    /// <summary>
    /// assetbundle包的内外网拉取地址，以及包类型和读取地址的映射
    /// </summary>
    public static class AssetBundlePath
    {
        //外网
        //public const string s_OnlineDownLoadPath = "http://jizhongchen.cn:3000/download/";
        //public const string s_OnlineDownLoadPathStandby = "http://123.207.216.69:3000/download/";
        public static List<string> s_OnlineDownLoadOriPath = new List<string>()
        {
            "http://jizhongchen.cn:3000/download/qmjb",
            "http://123.207.216.69:3000/download/",
            "http://1024mm.cn:3000/download/",
            "http://117.48.209.72:3000/download/"
        };

        public static List<string> s_OnlineDownLoadStandByPath = new List<string>()
        {
            "http://jizhongchen.cn:3000/download/qmjb",
            "http://123.207.216.69:3000/download/",
            "http://1024mm.cn:3000/download/",
            "http://117.48.209.72:3000/download/"
        };
        //内网
        public const string s_OfflineDownLoadPath = "http://10.40.15.55:80/download/qmjb/";
        public const string s_OfflineDownLoadPathStandby = "http://10.40.15.59:80/download/qmjb/";

        public const string s_RootResourcePath = "Assets/StreamingResource/";
        public const string s_RootBundlePath = "/StreamingAssetsBundle/";
        public const string s_RootCryptoBunldPath = "/Crypto/";

        public const string s_fileSuffix = ".assetbundle";
        public const string s_VersionFile = "Version.json";
        public const string s_DownLoadPathFile = "DownLoadPath.json";
        public const string s_AppName = "qmjb_demo.apk";
        public const string s_CodeBranchPathDefineFile = "PathDefine.json";

        public static Dictionary<emAssetBundle, List<emAssetBundle>> temp_DependDict = new Dictionary<emAssetBundle, List<emAssetBundle>>
        {
            {emAssetBundle.Bundle_UIPrefab,new List<emAssetBundle>(){emAssetBundle.Bundle_UISp_common,emAssetBundle.Bundle_UISp_DrawCard,emAssetBundle.Bundle_UISp_Fighting,emAssetBundle.Bundle_UISp_StateWar,emAssetBundle.Bundle_UISp_LoginAndMian, emAssetBundle.Bundle_Fonts } }
        };

        public static Dictionary<emAssetBundle, List<string>> s_dictAssetBundle = new Dictionary<emAssetBundle, List<string>>()
        {
            //{emAssetBundle.Bundle_Base, new List<string>() {"Base" } },

            {emAssetBundle.Bundle_GeneralCard, new List<string>() { "Sprites/largespritebundle/GeneralCard" } },
            {emAssetBundle.Bundle_SoldierCard,new List<string>() { "Sprites/largespritebundle/SoldierCard" } },
            //{emAssetBundle.Bundle_Fight, new List<string>() {"" } },
            {emAssetBundle.Bundle_Config, new List<string>() { "cfg" } },
            {emAssetBundle.Bundle_SkillEffect,new List<string>() { "Prefabs/EffectPrefabs/SkillEffects", "Prefabs/EffectPrefabs/BuffEffects" } },
            {emAssetBundle.Bundle_SoldierPrefabs,new List<string>() { "Prefabs/SoldierPrefabs" } },
            {emAssetBundle.Bundle_Sprite_SquadFlag,new List<string>() { "Sprites/largespritebundle/FlagIcon" } },
            {emAssetBundle.Bundle_Sprite_Achievement,new List<string>() { "Sprites/largespritebundle/AchievementIcon" } },
            {emAssetBundle.Bundle_Sprite_PlayerHeadIcon,new List<string>() { "Sprites/largespritebundle/PlayerHeadPort" } },
            {emAssetBundle.Bundle_Sprite_GeneralHeadIcon, new List<string>() { "Sprites/largespritebundle/HeadIcon" } },
            {emAssetBundle.Bundle_Sprite_2DBuild,new List<string>() { "Sprites/2dmap/2dbuild" } },
            {emAssetBundle.Bundle_TalkChannelIcon,new List<string>() { "Sprites/smallspritebundle/talk/talkchannelicon" } },
            {emAssetBundle.Bundle_DrawCard,new List<string>() { "Sprites/largespritebundle/cardbox" } },
            {emAssetBundle.Bundle_Pre_StateWarPrefab,new List<string>() { "Prefabs/StateMapPrefabs" } },
            {emAssetBundle.Bundle_StateFlag,new List<string>(){ "Sprites/largespritebundle/statewarprepare" } },
            {emAssetBundle.Bundle_BuffIcon, new List<string>() { "Sprites/largespritebundle/bufficon" } },
            {emAssetBundle.Bundle_ItemsIcon, new List<string>() { "Sprites/largespritebundle/itemsicon" } },
            {emAssetBundle.Bundle_SkillIcon, new List<string>() { "Sprites/largespritebundle/skillicon" } },
            {emAssetBundle.Bundle_Config_MonsterMap,new List<string>{ "cfg/MonsterMap" } },
            {emAssetBundle.Bundle_TempResource,new List<string>(){ "Sprites/largespritebundle/TempMap" } },

            {emAssetBundle.Bundle_StoryFightMap,new List<string>{"Sprites/largespritebundle/storyFightMap" } },
            {emAssetBundle.Bundle_UIPrefab,new List<string>(){ "Prefabs/UIPrefabs" } },
            {emAssetBundle.Bundle_FightMapPrefabs,new List<string>{ "Prefabs/FightMapPrefabs" } },
            {emAssetBundle.Bundle_Models,new List<string>(){ "Models" } },
            {emAssetBundle.Bundle_Fonts,new List<string>(){"Fonts" } },
            //{emAssetBundle.Bundle_SmallSprite,new List<string>{ "Sprites/smallspritebundle" } },

            #region ui分包
            {emAssetBundle.Bundle_UISp_common,new List<string>{ "Sprites/smallspritebundle/common" ,"Sprites/smallspritebundle/talk" , "Sprites/smallspritebundle/noviceguide" } },
            {emAssetBundle.Bundle_UISp_DrawCard,new List<string>{ "Sprites/smallspritebundle/drawcard" } },
            {emAssetBundle.Bundle_UISp_Fighting,new List<string>{ "Sprites/smallspritebundle/fighting" } },
            {emAssetBundle.Bundle_UISp_StateWar,new List<string>{ "Sprites/smallspritebundle/InStatewar" } },
            {emAssetBundle.Bundle_UISp_LoginAndMian,new List<string>{
                "Sprites/smallspritebundle/login","Sprites/smallspritebundle/main", "Sprites/smallspritebundle/personality" ,
                "Sprites/smallspritebundle/statewarPrepare", "Sprites/smallspritebundle/military", "Sprites/smallspritebundle/battle","Sprites/smallspritebundle/CreateGeneral","Sprites/smallspritebundle/RankList","Sprites/smallspritebundle/Sociality","Sprites/smallspritebundle/notice"} },
            #endregion
            
            {emAssetBundle.Bundle_Anim,new List<string>{ "Anim" } },
            {emAssetBundle.Bundle_Textures,new List<string>{ "Textures"} },

            {emAssetBundle.Bundle_Audio,new List<string>(){"Audio" } },

            // {emAssetBundle.Bundle_Audio,new List<string>(){"Audio" } },
            //{emAssetBundle.Bundle_StateWar_Map1, new List<string>() { "Sprites/2dmap/2dmapLL/1_2" } },
            //{emAssetBundle.Bundle_StateWar_Map2, new List<string>() { "Sprites/2dmap/2dmapLL/1_3" } },
            //{emAssetBundle.Bundle_StateWar_Map3, new List<string>() { "Sprites/2dmap/2dmapLL/1_4" } },
            //{emAssetBundle.Bundle_StateWar_Map4, new List<string>() { "Sprites/2dmap/2dmapLL/1_5" } },
            #region 地图分包地址
            {emAssetBundle.Bundle_StateWar_Map_1_1,new List<string>(){"Sprites/2dmap/2dmapLL/1_1"}},{emAssetBundle.Bundle_StateWar_Map_1_2,new List<string>(){"Sprites/2dmap/2dmapLL/1_2"}},
            { emAssetBundle.Bundle_StateWar_Map_1_3,new List<string>(){"Sprites/2dmap/2dmapLL/1_3"}},{emAssetBundle.Bundle_StateWar_Map_1_4,new List<string>(){"Sprites/2dmap/2dmapLL/1_4"}},
            { emAssetBundle.Bundle_StateWar_Map_1_5,new List<string>(){"Sprites/2dmap/2dmapLL/1_5"}},{emAssetBundle.Bundle_StateWar_Map_1_6,new List<string>(){"Sprites/2dmap/2dmapLL/1_6"}},
            { emAssetBundle.Bundle_StateWar_Map_1_7,new List<string>(){"Sprites/2dmap/2dmapLL/1_7"}},{emAssetBundle.Bundle_StateWar_Map_1_8,new List<string>(){"Sprites/2dmap/2dmapLL/1_8"}},
            { emAssetBundle.Bundle_StateWar_Map_1_9,new List<string>(){"Sprites/2dmap/2dmapLL/1_9"}},{emAssetBundle.Bundle_StateWar_Map_1_10,new List<string>(){"Sprites/2dmap/2dmapLL/1_10"}},
            { emAssetBundle.Bundle_StateWar_Map_1_11,new List<string>(){"Sprites/2dmap/2dmapLL/1_11"}},{emAssetBundle.Bundle_StateWar_Map_2_1,new List<string>(){"Sprites/2dmap/2dmapLL/2_1"}},
            { emAssetBundle.Bundle_StateWar_Map_2_2,new List<string>(){"Sprites/2dmap/2dmapLL/2_2"}},{emAssetBundle.Bundle_StateWar_Map_2_3,new List<string>(){"Sprites/2dmap/2dmapLL/2_3"}},
            { emAssetBundle.Bundle_StateWar_Map_2_4,new List<string>(){"Sprites/2dmap/2dmapLL/2_4"}},{emAssetBundle.Bundle_StateWar_Map_2_5,new List<string>(){"Sprites/2dmap/2dmapLL/2_5"}},
            { emAssetBundle.Bundle_StateWar_Map_2_6,new List<string>(){"Sprites/2dmap/2dmapLL/2_6"}},{emAssetBundle.Bundle_StateWar_Map_2_7,new List<string>(){"Sprites/2dmap/2dmapLL/2_7"}},
            { emAssetBundle.Bundle_StateWar_Map_2_8,new List<string>(){"Sprites/2dmap/2dmapLL/2_8"}},{emAssetBundle.Bundle_StateWar_Map_2_9,new List<string>(){"Sprites/2dmap/2dmapLL/2_9"}},
            { emAssetBundle.Bundle_StateWar_Map_2_10,new List<string>(){"Sprites/2dmap/2dmapLL/2_10"}},{emAssetBundle.Bundle_StateWar_Map_2_11,new List<string>(){"Sprites/2dmap/2dmapLL/2_11"}},
            { emAssetBundle.Bundle_StateWar_Map_3_1,new List<string>(){"Sprites/2dmap/2dmapLL/3_1"}},{emAssetBundle.Bundle_StateWar_Map_3_2,new List<string>(){"Sprites/2dmap/2dmapLL/3_2"}},
            { emAssetBundle.Bundle_StateWar_Map_3_3,new List<string>(){"Sprites/2dmap/2dmapLL/3_3"}},{emAssetBundle.Bundle_StateWar_Map_3_4,new List<string>(){"Sprites/2dmap/2dmapLL/3_4"}},
            { emAssetBundle.Bundle_StateWar_Map_3_5,new List<string>(){"Sprites/2dmap/2dmapLL/3_5"}},{emAssetBundle.Bundle_StateWar_Map_3_6,new List<string>(){"Sprites/2dmap/2dmapLL/3_6"}},
            { emAssetBundle.Bundle_StateWar_Map_3_7,new List<string>(){"Sprites/2dmap/2dmapLL/3_7"}},{emAssetBundle.Bundle_StateWar_Map_3_8,new List<string>(){"Sprites/2dmap/2dmapLL/3_8"}},
            { emAssetBundle.Bundle_StateWar_Map_3_9,new List<string>(){"Sprites/2dmap/2dmapLL/3_9"}},{emAssetBundle.Bundle_StateWar_Map_3_10,new List<string>(){"Sprites/2dmap/2dmapLL/3_10"}},
            { emAssetBundle.Bundle_StateWar_Map_3_11,new List<string>(){"Sprites/2dmap/2dmapLL/3_11"}},{emAssetBundle.Bundle_StateWar_Map_4_1,new List<string>(){"Sprites/2dmap/2dmapLL/4_1"}},
            { emAssetBundle.Bundle_StateWar_Map_4_2,new List<string>(){"Sprites/2dmap/2dmapLL/4_2"}},{emAssetBundle.Bundle_StateWar_Map_4_3,new List<string>(){"Sprites/2dmap/2dmapLL/4_3"}},
            { emAssetBundle.Bundle_StateWar_Map_4_4,new List<string>(){"Sprites/2dmap/2dmapLL/4_4"}},{emAssetBundle.Bundle_StateWar_Map_4_5,new List<string>(){"Sprites/2dmap/2dmapLL/4_5"}},
            { emAssetBundle.Bundle_StateWar_Map_4_6,new List<string>(){"Sprites/2dmap/2dmapLL/4_6"}},{emAssetBundle.Bundle_StateWar_Map_4_7,new List<string>(){"Sprites/2dmap/2dmapLL/4_7"}},
            { emAssetBundle.Bundle_StateWar_Map_4_8,new List<string>(){"Sprites/2dmap/2dmapLL/4_8"}},{emAssetBundle.Bundle_StateWar_Map_4_9,new List<string>(){"Sprites/2dmap/2dmapLL/4_9"}},
            { emAssetBundle.Bundle_StateWar_Map_4_10,new List<string>(){"Sprites/2dmap/2dmapLL/4_10"}},{emAssetBundle.Bundle_StateWar_Map_4_11,new List<string>(){"Sprites/2dmap/2dmapLL/4_11"}},
            { emAssetBundle.Bundle_StateWar_Map_5_1,new List<string>(){"Sprites/2dmap/2dmapLL/5_1"}},{emAssetBundle.Bundle_StateWar_Map_5_2,new List<string>(){"Sprites/2dmap/2dmapLL/5_2"}},
            { emAssetBundle.Bundle_StateWar_Map_5_3,new List<string>(){"Sprites/2dmap/2dmapLL/5_3"}},{emAssetBundle.Bundle_StateWar_Map_5_4,new List<string>(){"Sprites/2dmap/2dmapLL/5_4"}},
            { emAssetBundle.Bundle_StateWar_Map_5_5,new List<string>(){"Sprites/2dmap/2dmapLL/5_5"}},{emAssetBundle.Bundle_StateWar_Map_5_6,new List<string>(){"Sprites/2dmap/2dmapLL/5_6"}},
            { emAssetBundle.Bundle_StateWar_Map_5_7,new List<string>(){"Sprites/2dmap/2dmapLL/5_7"}},{emAssetBundle.Bundle_StateWar_Map_5_8,new List<string>(){"Sprites/2dmap/2dmapLL/5_8"}},
            { emAssetBundle.Bundle_StateWar_Map_5_9,new List<string>(){"Sprites/2dmap/2dmapLL/5_9"}},{emAssetBundle.Bundle_StateWar_Map_5_10,new List<string>(){"Sprites/2dmap/2dmapLL/5_10"}},
            { emAssetBundle.Bundle_StateWar_Map_5_11,new List<string>(){"Sprites/2dmap/2dmapLL/5_11"}},
            #endregion
            #region 武将卡牌分包地址
            {emAssetBundle.Bundle_GeneralCard_1,new List<string>(){ "Sprites/largespritebundle/GeneralCard/GeneralCard_1" } },{emAssetBundle.Bundle_GeneralCard_2,new List<string>(){ "Sprites/largespritebundle/GeneralCard/GeneralCard_2" } },
            { emAssetBundle.Bundle_GeneralCard_3,new List<string>(){ "Sprites/largespritebundle/GeneralCard/GeneralCard_3" } },{ emAssetBundle.Bundle_GeneralCard_4,new List<string>(){ "Sprites/largespritebundle/GeneralCard/GeneralCard_4" } },
            { emAssetBundle.Bundle_GeneralCard_5,new List<string>(){ "Sprites/largespritebundle/GeneralCard/GeneralCard_5" } },{ emAssetBundle.Bundle_GeneralCard_6,new List<string>(){ "Sprites/largespritebundle/GeneralCard/GeneralCard_6" } },
            { emAssetBundle.Bundle_GeneralCard_7,new List<string>(){ "Sprites/largespritebundle/GeneralCard/GeneralCard_7" } },{ emAssetBundle.Bundle_GeneralCard_8,new List<string>(){ "Sprites/largespritebundle/GeneralCard/GeneralCard_8" } }
        #endregion
        };

        public static Dictionary<emAssetBundle, List<emAssetBundle>> s_dictAssetBundleMapping = new Dictionary<emAssetBundle, List<emAssetBundle>>()
        {
            {emAssetBundle.Bundle_StateWar_Map_1_1, new List<emAssetBundle> {
#region 地图分包映射列表
                    //emAssetBundle.Bundle_StateWar_Map1,    emAssetBundle.Bundle_StateWar_Map2,    emAssetBundle.Bundle_StateWar_Map3,    emAssetBundle.Bundle_StateWar_Map4,
                emAssetBundle.Bundle_StateWar_Map_1_1,emAssetBundle.Bundle_StateWar_Map_1_2,emAssetBundle.Bundle_StateWar_Map_1_3,
                emAssetBundle.Bundle_StateWar_Map_1_4,emAssetBundle.Bundle_StateWar_Map_1_5,emAssetBundle.Bundle_StateWar_Map_1_6,
                emAssetBundle.Bundle_StateWar_Map_1_7,emAssetBundle.Bundle_StateWar_Map_1_8,emAssetBundle.Bundle_StateWar_Map_1_9,
                emAssetBundle.Bundle_StateWar_Map_1_10,emAssetBundle.Bundle_StateWar_Map_1_11,
                emAssetBundle.Bundle_StateWar_Map_2_1,emAssetBundle.Bundle_StateWar_Map_2_2,emAssetBundle.Bundle_StateWar_Map_2_3,
                emAssetBundle.Bundle_StateWar_Map_2_4,emAssetBundle.Bundle_StateWar_Map_2_5,emAssetBundle.Bundle_StateWar_Map_2_6,
                emAssetBundle.Bundle_StateWar_Map_2_7,emAssetBundle.Bundle_StateWar_Map_2_8,emAssetBundle.Bundle_StateWar_Map_2_9,
                emAssetBundle.Bundle_StateWar_Map_2_10,emAssetBundle.Bundle_StateWar_Map_2_11,
                emAssetBundle.Bundle_StateWar_Map_3_1,emAssetBundle.Bundle_StateWar_Map_3_2,emAssetBundle.Bundle_StateWar_Map_3_3,
                emAssetBundle.Bundle_StateWar_Map_3_4,emAssetBundle.Bundle_StateWar_Map_3_5,emAssetBundle.Bundle_StateWar_Map_3_6,
                emAssetBundle.Bundle_StateWar_Map_3_7,emAssetBundle.Bundle_StateWar_Map_3_8,emAssetBundle.Bundle_StateWar_Map_3_9,
                emAssetBundle.Bundle_StateWar_Map_3_10,emAssetBundle.Bundle_StateWar_Map_3_11,
                emAssetBundle.Bundle_StateWar_Map_4_1,emAssetBundle.Bundle_StateWar_Map_4_2,emAssetBundle.Bundle_StateWar_Map_4_3,
                emAssetBundle.Bundle_StateWar_Map_4_4,emAssetBundle.Bundle_StateWar_Map_4_5,emAssetBundle.Bundle_StateWar_Map_4_6,
                emAssetBundle.Bundle_StateWar_Map_4_7,emAssetBundle.Bundle_StateWar_Map_4_8,emAssetBundle.Bundle_StateWar_Map_4_9,
                emAssetBundle.Bundle_StateWar_Map_4_10,emAssetBundle.Bundle_StateWar_Map_4_11,
                emAssetBundle.Bundle_StateWar_Map_5_1,emAssetBundle.Bundle_StateWar_Map_5_2,emAssetBundle.Bundle_StateWar_Map_5_3,
                emAssetBundle.Bundle_StateWar_Map_5_4,emAssetBundle.Bundle_StateWar_Map_5_5,emAssetBundle.Bundle_StateWar_Map_5_6,
                emAssetBundle.Bundle_StateWar_Map_5_7,emAssetBundle.Bundle_StateWar_Map_5_8,emAssetBundle.Bundle_StateWar_Map_5_9,
                emAssetBundle.Bundle_StateWar_Map_5_10,emAssetBundle.Bundle_StateWar_Map_5_11,
#endregion
            }
            }
            ,
            {emAssetBundle.Bundle_GeneralCard,new List<emAssetBundle>{
               emAssetBundle.Bundle_GeneralCard,emAssetBundle.Bundle_GeneralCard_1,emAssetBundle.Bundle_GeneralCard_2,emAssetBundle.Bundle_GeneralCard_3,emAssetBundle.Bundle_GeneralCard_4,emAssetBundle.Bundle_GeneralCard_5,emAssetBundle.Bundle_GeneralCard_6,emAssetBundle.Bundle_GeneralCard_7,emAssetBundle.Bundle_GeneralCard_8,
                }
            }
        };

        public static List<emAssetBundle> GetMappingName(emAssetBundle emBundle)
        {
            if (s_dictAssetBundleMapping.ContainsKey(emBundle))
            {
                return s_dictAssetBundleMapping[emBundle];
            }
            else
            {
                return null;
            }
        }
    }
    #endregion
}

public static class global_ConstDataDefine
{
    public const int TroopMemberNum = 7;
    public const float g_LenthUnit = 100;   // 长度单位为厘米时所有长度运算除以此值
    public const float loseLinkOffset = 5;

    public const float doubleClickOffset = 0.4f;//双击判定最短间隔

    public const int DecodeEncodeBlockSize = 1048576;

    public const int GuideTenDrawCardPack = 100;
    /// <summary>
    /// 频道相关色彩
    /// </summary>
    public static class ChannelColor
    {

        //频道背景颜色
        public static Color PresentChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#87A0D2FF", out outColor);
                return outColor;
            }
        }
        public static Color PresentChannelBottom
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#433235FF", out outColor);
                return outColor;
            }
        }
        public static Color HornChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#36C8C3FF", out outColor);
                return outColor;
            }
        }
        public static Color WorldChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#2D3C31FF", out outColor);
                return outColor;
            }
        }
        public static Color BattleFieldChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#D76850FF", out outColor);
                return outColor;
            }
        }
        public static Color FamilyChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#328CF0FF", out outColor);
                return outColor;
            }
        }
        public static Color WhisperChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#7D1EFFFF", out outColor);
                return outColor;
            }
        }
        public static Color SystemChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#990707FF", out outColor);
                return outColor;
            }
        }
        public static Color AllChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#19A000FF", out outColor);
                return outColor;
            }
        }

        //频道标签颜色
        public static Color PresentChannelTag
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#5f737dFF", out outColor);
                return outColor;
            }
        }
        public static Color HornChannelTag
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#238c8cFF", out outColor);
                return outColor;
            }
        }
        public static Color WorldChannelTag
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#bea005FF", out outColor);
                return outColor;
            }
        }
        public static Color BattleFieldChannelTag
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#a55523FF", out outColor);
                return outColor;
            }
        }
        public static Color FamilyChannelTag
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#376eb4FF", out outColor);
                return outColor;
            }
        }
        public static Color WhisperChannelTag
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#7d4bb9FF", out outColor);
                return outColor;
            }
        }
        public static Color SystemChannelTag
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#d20a0aFF", out outColor);
                return outColor;
            }
        }

        //混合后的频道色
        public static Color Mixed_FamilyChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#262838FF", out outColor);
                return outColor;
            }
        }

        public static Color Mixed_BattleFieldChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#4E2013FF", out outColor);
                return outColor;
            }
        }

        public static Color Mixed_WhisperChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#380E3BFF", out outColor);
                return outColor;
            }
        }

        public static Color Mixed_AllChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#202C00FF", out outColor);
                return outColor;
            }
        }

        public static Color Mixed_HornChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#293A31FF", out outColor);
                return outColor;
            }
        }

        public static Color Mixed_SystemChannel
        {
            get
            {
                Color outColor = new Color(0, 0, 0, 0);
                ColorUtility.TryParseHtmlString("#3E0902FF", out outColor);
                return outColor;
            }
        }
        //public static Color AllChannelTag
        //{
        //    get
        //    {
        //        Color outColor = new Color(0, 0, 0, 0); 
        //        ColorUtility.TryParseHtmlString("#19A000FF", out outColor);
        //        return outColor;
        //    }
        //}
    }
    /// <summary>
    /// Sprite图标资源地址 相对于sprite目录
    /// </summary>
    public static class IconPath
    {
        public const string MsgBoxIconFolderPath = "common";

        public const string FlagIconFolderPath = "military/flagicon";
        public const string FlagIconPrefix = "array_";

        public const string HeadIconFolderPath = "military/headicon";
        public const string HeadIcon_GeneralPrefix = "head_general_";
        public const string HeadIcon_GeneralQualityPrefix = "bg_general_";
        public const string HeadIcon_AdviserQualityPrefix = "bg_general_";
        public const string HeadIcon_GeneralChooseQualityPrefix = "bg_general_";
        public const string HeadIcon_GeneralChooseQualityBgPrefix = "bg_military_choosegeneral_list_";
        public const string HeadIcon_GeneralLoadingQualityPrefix = "bg_loading_general_";
        //public const string GeneralRepoCardPath = "military/generalcard";
        public const string GeneralRepoCard_CardFacePrefix = "card_general_";
        public const string GeneralRepoCard_CardColorPrefix = "card_quality_";
        public const string GeneralRepoCard_CardQualityPrefix = "bg_quality_filter_";
        //public const string SoldierRepoCardPath = "military/soldiercard";
        public const string SoliderRepoCard_CardFacePrefix = "card_soldier_";

        public const string PlayerHeadIconPath = "personality/playerheadport";
        public const string PlayerHeadIconPrefix = "head_player_";

        public const string StateWarMapPath = "2dmap/2dmaplow";
        public const string StateWarMapPrefix = "BattleMap_";

        public const string StateWarCityPath = "2dmap/2dbuild";
        //public const string StateWarCityPrefix = "jianzu_";
        public const string StateWarCityEmpty = "jianzu_1";

        public const string DrawCardBagPrefix = "card_summon_";

        public const string StateFlagDisSuffix = "_disabled";
    }
    /// <summary>
    /// 特效资源地址
    /// </summary>
    public static class EffectPath
    {
        public const string MagicalEffectPath = "testeffect";
    }
    /// <summary>
    /// 模型资源地址
    /// </summary>
    public static class ModelPath
    {
        public const string SoldierModelPath = "Prefabs/EntityPrefabs/Model/";
    }
    public static class OtherPrefabsPath
    {
        public const string StateMapPrefabPath = "statemapprefabs";
        public const string MsgBoxexPrefabPath = "Prefabs/UIPrefabs/MsgBoxes/";
    }
    public static class RankColorInDrawCardProb
    {
        public readonly static Color SR = new Color(1, 1, 1, 1);
        public readonly static Color S = new Color(0.5f, 0.5f, 0.5f, 1f);
        public readonly static Color A = new Color(1, 1, 1, 1);
        public readonly static Color B = new Color(1, 1, 1, 1);
        public readonly static Color C = new Color(1, 1, 1, 1);
        public readonly static Color D = new Color(1, 1, 1, 1);

        public static Color GetRankColor(string rank)
        {

            switch (rank.Replace(" ", ""))
            {
                case "SR":
                    return SR;
                case "S":
                    return S;
                case "A":
                    return A;
                case "B":
                    return B;
                case "C":
                    return C;
                case "D":
                    return D;
                default:
                    return new Color(0, 0, 0, 0);
            }
        }
    }
    public static class PerformanceSettings
    {
        public struct PerformanceSetting
        {
            //public QualitySettings performanceQuality = QualitySettings.
        }
    }
    public static class OtherColors
    {
        public readonly static Color32 Col_ChopGeneralSelfLight = new Color32(105, 150, 255, 255);
        public readonly static Color32 Col_ChopGeneralEnemyLight = new Color32(255, 60, 60, 255);
        public readonly static Color32 DisableButtonText = new Color32(227, 227, 227, 255);
    }

    public static class RarityColor
    {
        public static string GetStringWithName(string Name, int rarity)
        {
            switch (rarity)
            {
                case 1:
                    return "<color=\"#FFFFFF\">" + Name + "</color>";
                case 2:
                    return "<color=\"#19EB0A\">" + Name + "</color>";
                case 3:
                    return "<color=\"#2882FF\">" + Name + "</color>";
                case 4:
                    return "<color=\"#D74BFF\">" + Name + "</color>";
                case 5:
                    return "<color=\"#FF8200\">" + Name + "</color>";
                default:
                    return null;
            }
        }
    }

}

#region 服务器枚举定义
public enum emAccountType
{
    accountName = 0,
    qq = 1,
    wx = 2

}
public enum emChangeSceneResult
{
    Success = 0,
    UnknownError = 1
}
#endregion

#region Enum定义
//包应分两种，一种需要常驻内存，另一种需要时加载可即时卸载存入字典，需要时调取
//当加载某些包时，对应的依赖项需常驻于内存（如UI,模型和模型的贴图）
public enum emAssetBundle
{

    Bundle_Base = 0,
    #region 常驻资源 resident_res
    //字体包
    Bundle_Fonts,
    //暂用UI基础包
    #region UI包
    //Bundle_SmallSprite,
    Bundle_UISp_common,
    Bundle_UISp_LoginAndMian,
    Bundle_UISp_DrawCard,
    Bundle_UISp_Fighting,
    Bundle_UISp_StateWar,
    #endregion


    #endregion
    Bundle_Config,
    Bundle_Config_MonsterMap,
    Bundle_SoldierPrefabs,
    Bundle_Models,
    Bundle_Textures,
    Bundle_Anim,
    Bundle_StoryFightMap,
    Bundle_FightMapPrefabs,
    //Bundle_Fight,
    Bundle_Pre_StateWarPrefab,

    Bundle_SkillEffect,

    Bundle_TempResource,
    Bundle_Audio,

    Bundle_UIPrefab,
    //Bundle_StateWar_Map,
    //Bundle_StateWar_Map1,
    //Bundle_StateWar_Map2,
    //Bundle_StateWar_Map3,
    //Bundle_StateWar_Map4,
    #region 动态资源包 
    //武将卡牌包
    Bundle_GeneralCard,
    #region 武将卡牌分包
    /// <summary>
    /// 由于目前6个打一张图集，分包暂时为48张一包
    /// </summary>
    Bundle_GeneralCard_1,
    Bundle_GeneralCard_2,
    Bundle_GeneralCard_3,
    Bundle_GeneralCard_4,
    Bundle_GeneralCard_5,
    Bundle_GeneralCard_6,
    Bundle_GeneralCard_7,
    Bundle_GeneralCard_8,
    #endregion

    //士兵卡牌包
    Bundle_SoldierCard,
    //武将头像
    Bundle_Sprite_GeneralHeadIcon,
    //部队标志
    Bundle_Sprite_SquadFlag,
    //玩家头像
    Bundle_Sprite_PlayerHeadIcon,
    //国战2D战场城市
    Bundle_Sprite_2DBuild,
    //卡牌包
    Bundle_DrawCard,
    //buff图标
    Bundle_BuffIcon,
    //国家标志
    Bundle_StateFlag,
    //聊天频道标志
    Bundle_TalkChannelIcon,
    //技能图标
    Bundle_SkillIcon,
    Bundle_ItemsIcon,
    //成就标志
    Bundle_Sprite_Achievement,
    #region 地图分包枚举
    Bundle_StateWar_Map_1_1,
    Bundle_StateWar_Map_1_2,
    Bundle_StateWar_Map_1_3,
    Bundle_StateWar_Map_1_4,
    Bundle_StateWar_Map_1_5,
    Bundle_StateWar_Map_1_6,
    Bundle_StateWar_Map_1_7,
    Bundle_StateWar_Map_1_8,
    Bundle_StateWar_Map_1_9,
    Bundle_StateWar_Map_1_10,
    Bundle_StateWar_Map_1_11,
    Bundle_StateWar_Map_2_1,
    Bundle_StateWar_Map_2_2,
    Bundle_StateWar_Map_2_3,
    Bundle_StateWar_Map_2_4,
    Bundle_StateWar_Map_2_5,
    Bundle_StateWar_Map_2_6,
    Bundle_StateWar_Map_2_7,
    Bundle_StateWar_Map_2_8,
    Bundle_StateWar_Map_2_9,
    Bundle_StateWar_Map_2_10,
    Bundle_StateWar_Map_2_11,
    Bundle_StateWar_Map_3_1,
    Bundle_StateWar_Map_3_2,
    Bundle_StateWar_Map_3_3,
    Bundle_StateWar_Map_3_4,
    Bundle_StateWar_Map_3_5,
    Bundle_StateWar_Map_3_6,
    Bundle_StateWar_Map_3_7,
    Bundle_StateWar_Map_3_8,
    Bundle_StateWar_Map_3_9,
    Bundle_StateWar_Map_3_10,
    Bundle_StateWar_Map_3_11,
    Bundle_StateWar_Map_4_1,
    Bundle_StateWar_Map_4_2,
    Bundle_StateWar_Map_4_3,
    Bundle_StateWar_Map_4_4,
    Bundle_StateWar_Map_4_5,
    Bundle_StateWar_Map_4_6,
    Bundle_StateWar_Map_4_7,
    Bundle_StateWar_Map_4_8,
    Bundle_StateWar_Map_4_9,
    Bundle_StateWar_Map_4_10,
    Bundle_StateWar_Map_4_11,
    Bundle_StateWar_Map_5_1,
    Bundle_StateWar_Map_5_2,
    Bundle_StateWar_Map_5_3,
    Bundle_StateWar_Map_5_4,
    Bundle_StateWar_Map_5_5,
    Bundle_StateWar_Map_5_6,
    Bundle_StateWar_Map_5_7,
    Bundle_StateWar_Map_5_8,
    Bundle_StateWar_Map_5_9,
    Bundle_StateWar_Map_5_10,
    Bundle_StateWar_Map_5_11,
    #endregion

    #endregion
    Bundle_End
}

public enum emGameStatus
{
    emGameStatus_None = 0,
    emGameStatus_Login,
    emGameStatus_Game,
    emGameStatus_Fight,
}
public enum emSceneStatus
{
    emSceneStatus_None = 0,
    emSceneStatus_Login,
    emSceneStatus_Game,
    emSceneStatus_StateWar,
    emSceneStatus_Fight,
}
public enum emUIStatus
{
    emUIStatus_None = 0,
    emUIStatus_Login,
    emUIstatus_CreatePlayer,
    emUIstatus_MainUI,
    emUIstatus_FightUi,
    emUIstatus_StateWarUI,
}

public enum emFactionType
{
    CampType_None = 0,
    CampType_Self,
    CampType_Enemy,
}

public enum emTroopRelation
{
    emTroopRelation_Self = 1,
    emTroopRelation_Enemy,
    emTroopRelation_Friend
}

public enum emSpaceType
{
    Null = 0,
    Match = 1,//  # 匹配
    StateWar_City = 2,//  # 攻城战
    StateWar_Line = 3,//  # 野战
}

public enum emServerState
{
    Null = 0,
    Hot = 1,//火爆
    Recommend = 2,//推荐
    Full = 3,//满
    Close = 4,//关闭
}

public enum Enum_MonsterType
{
    Normal = 0,
    Building = 1,
}

#region 消息系统相关枚举
public enum emSysMsgType
{
    emSysMsgType_World = 1,
    emSysMsgType_War = 2,
    emSysMsgType_PopBox = 3,
    emSysMsgType_TalkShow = 4
}

public enum emSysMsgShowPos
{
    emSysMsgShowPos_World,
    emSysMsgShowPos_War,
    emSysMsgShowPos_PopBox,
    emSysMsgShowPos_TalkShow
}

public enum emTalkShowMsgType
{
    emTalkShowMsgType_System,
    emTalkShowMsgType_World,
    emTalkShowMsgType_Friend,
    emTalkShowMsgType_Group
}

public enum emChannelType
{
    System = 1,
    Private = 2,
    Camp = 3,
    World = 4,
    Faction = 5,
    Family = 6
}

/// <summary>
/// 按钮样式
/// </summary>
public enum emMsgBoxButtonGroupDefine
{
    MsgBoxDefine_OkAndCancelBox,
    MsgBoxDefine_OkOnly,
    MsgBoxDefine_MaskOnly,
    MsgBoxDefine_End
}
/// <summary>
/// 预设样式
/// </summary>
public enum emAutoMsgBoxStyle
{
    emAutoMsgBoxStyle_None,
    emAutoMsgBoxStyle_FullCenter,
    emAutoMsgBoxStyle_UpCenter,
    emAutoMsgBoxStyle_BottomCenter
}
public enum emAwardUIType
{
    Null = 0,
    Sweep = 1,//#扫荡
    DrawCard = 2, //# 抽卡 
    Fight = 3,  //# 战斗结算
}
#endregion

#region 部队相关枚举
public enum emTroopUseType
{
    Null = 0,
    CurFight = 1,//  # 当前战争部队
    StateWar = 2,//  # 国战部队
    OnHook = 3,//  # 挂机中
}


#endregion

#region 武将相关枚举
public enum emGeneralType
{
    Null,
    Attack = 1,
    Defense,
    Speed,
    Strategy,   //谋略型
    Siege,  //攻城型
}

public enum emGeneralWork
{
    Null,
    Leader = 1, //主帅
    Adviser = 2,    //军师
    Warrior = 3,    //猛将
    Custom = 4,//自定义武将
}

public enum emGeneralColor
{
    Normal = 1,

}

#endregion

#region 士兵相关枚举
public enum emSoldierType
{
    Null,
    FootMan = 1, //步
    Rider,  //骑
    Archer, //弓
    Special //特殊兵种
}

public enum emSoldierShowType
{
    Null,
    Troop,  //部队
    CampSite,   //行营
    MainCity,   //主据点
}

public enum emSkillDistanceType
{
    Null,
    Near,
    Far,
    Special,
}

public enum emSkillType
{
    Null,
    LeaderSkill = 1, //主帅技能
    WarriorSkill = 2,    //猛将技能
    AdviserSkill = 3,    //军师技能
    SoldierSkill = 4,//士兵技能

}

public enum emSkillTargetType
{
    Null,
    Enemy,
    Self,
    Friend,
    SelfChief,
    EnemyChief,
    All,
    SelfCamp,
    EnemyCamp,
}

#endregion

#region 特效相关枚举
public enum emShakeType
{
    emShakeType_None,
    emShakeType_Normal,
    emShakeType_Vertical,
    emShakeType_Horizontal
}

public enum emEffectType
{
    emEffectType_None,
    emEffectType_Start,
    emEffectType_Track,
    emEffectType_Hit
}
#endregion

#region 玩家资源类型定义枚举
public enum emPlayerResource
{
    Population,
    Money,
    RMBMoney,
    Food
}
#endregion

#region 国战相关枚举

public enum Enum_SpaceType
{
    Null = 0,
    Match = 1,//  # 匹配
    StateWar_City = 2,//  # 攻城战
    StateWar_Line = 3,//  # 野战
    Battle = 4,//　#战役
    StoryFight = 5,// #剧情
}
public enum Enum_StateWar_Status
{
    Null = 0,
    Idle = 1,  //闲置等待
    Fighting = 2,  // 交战
    End = 3,  // 结束
}

public enum Enum_StateWar_RegResult
{
    Succeed = 0, //成功
    TotalPlayer = 1, // 总人数上限
    StatePlayer = 2, // 国家人数上限
    NotOpen = 3, // 未开启
    TimeOver = 4, // 时间已到人数不足
}

public enum Enum_StateWar_RoomStatus
{
    Close = 0,
    Register = 1, // 报名
    Fighting = 2, // 开战
}

public enum emStateWarPlayerPosType
{
    Null = 0,
    City = 1,
    Line = 2,
    WaterLine = 3
}


public enum Enum_StateWar_EnterResult
{
    Succeed = 0, // 成功
    NotReg = 1, // 未报名
    NotOpen = 2,// 未开启
    LackRes = 3,//资源不足
}

public enum Enum_CityType
{
    MainCity = 1,//主城
    Normal = 2,//城市
    Pass = 3,//关隘
    Port = 4,//港口
}

public enum Enum_AwardType
{
    Null = 0,
    Money = 1,  //# 金钱
    RMBMoney = 2, // # 充值金钱
    Food = 3,  //# 粮食
    General = 4,  //# 武将
    Soldier = 5,  //# 兵种
    Population = 6,//  # 人口
    Item = 7,  //# 物品
    Integral = 8,//  # 积分
    Feat = 9, // # 功勋
    Exp = 10,//  # 经验
    StateWarIntegral = 11,//  # 国战积分
}

public enum Enum_StateType
{
    /// <summary>
    /// 玩家
    /// </summary>
    Player = 0,
    /// <summary>
    /// 异族
    /// </summary>
    Alien = 1,
    /// <summary>
    /// 中立
    /// </summary>
    Neutral = 2,
}


#endregion

#region 玩家数据定义枚举
public enum emPlayerData
{
    Exp,
    Level,
    State,  //国家
    Integral,   //积分
    Feat,   //功勋
    OutToFightSquad,
}

public enum emExhibitionType
{
    None,
    GeneralCard,
    Achieve
}

#endregion

#region 战场枚举

enum Enum_CampType
{
    System = 0,//  # 系统
    Attack = 1,//  # 攻方
    Defense = 2,//  # 守方
}

enum Enum_CameraInitPos
{
    MinPos,
    MaxPos,
    CenterPos,
}

public enum Enum_HitParam
{
    Null = 0,
    SkillImmune = 1,
    HPImmune = 2,
    StrongholdImmune = 3,
}

public enum emFightStatus
{
    Waiting = 0,
    Ready = 1,
    Fighting = 2,
    End = 3
}


public enum emSkillRangeType
{

    SingleTarget,
    AOE,
    All,
    SingleTargetAndRange,
}

public enum emTroopUIType
{
    GodViewTroopUI,
    CloseViewTroopUI,
}

#endregion

#region buff枚举
public enum OverlayType
{
    DontOverlay = 0, //不叠加
    TimeOverlay = 1, //时长叠加
    CountOverlay = 2,//层数叠加
}
public enum BuffType
{
    Buff,
    DeBuff,
}
#endregion

#region 界面枚举
public enum emMainModelUI
{
    none,
    createGeneralUI,
    battleUI,
    matchUI,
    socialUI,
    DrawCardUI
}

public enum Enum_LockedFunc
{
    none,
    createGeneralFunc = 1,
    battleFunc=2,
    matchFunc=4,
    socialFunc=8,
    militaryFunc=16,
    drawcardFunc=32,
    statewarFunc=64,
    interiorFunc=128,
}

public enum Enum_HpBarUIType
{
    Null,
    Normal,
    Simple,
    Building,
}

public enum Enum_FightingUIView
{
    NormalGodView,
    NormalLockView,
    SimpleHpBarNoTop,
}

public enum Enum_BaseUIType {
    Normal,
    Fixed,
    PopUp,
    Except
}

public enum Enum_BaseUIShowMode {
    Normal,
    HideOther,
    Cover
}

public enum Enum_UIShowStatus
{
    Hiding,
    Showing,
    Show,
    Hide,
}
#endregion

#region 帮派枚举
public enum emFactionPost
{
    President = 1,  // 会长
    VicePresident = 2,  // 副会长
    Elder = 3,// 长老
    Elite = 4, //精英
    Member = 5,  // 成员
}

public enum emFactionPermission
{
    Appoint_VicePresident = 1,  // 任命副会长
    Appoint_Elder = 2, // 任命长老
    Appoint_Elite = 4, // 任命精英
    Appoint_Member = 8, // 任命成员
    Expel_VicePresident = 16, // 开除副会长
    Expel_Elder = 32, // 开除长老
    Expel_Elite = 64, // 开除精英
    Expel_Member = 128,// 开除成员
    Recruit = 256, // 招聘
    UpLevel = 512, // 升级
    Dissolve = 1024, // 解散
    Appoint_President = 2048,  // 任命会长
}

public enum emLeaveFactionReason
{
    AccordLeave = 0, // 主动离开
    Expel = 1,  // 开除
    Dissolve = 2,  // 解散
}

#endregion

#region 家族枚举

public enum emFamilyPost
{
    Shaikh = 1, // 族长
    ViceShaikh = 2,// 副族长
    Elder = 3,//  长老
    Elite = 4,// 精英
    Member = 5,// 成员
}

public enum emFamilyPermission
{
    ChangeNotice = 1  // 修改公告
}

#endregion

#region 排行榜枚举
public enum emRankingListType
{
    Level = 0,  // 等级榜
    Faction = 1,  // 帮会榜
    FightCapacity = 2  // 战力榜
}
   
#endregion
#endregion


#region 结构体定义
//显示用的奖励信息
public struct AwardInfo
{
    public Enum_AwardType Type;
    public uint ID;
    //数量大于1则显示
    public uint Num;
}


public struct TalkShowMsgData
{
    public emTalkShowMsgType msgType;

    public string SenderName;
    public string MsgContent;

}

public struct FontData
{
    public UInt16 font;
    public UInt16 fontsize;
}

public struct TopMsgData
{
    public struct RepeatSetting
    {
        public UInt16 isrepeat;
        public UInt16 repeatnum;
        public UInt16 repeatspacing;
    }
    public emSysMsgType msgType;
    public RepeatSetting repeatSetting;
    public emSysMsgShowPos showPosition;
    //优先级 0-9 0为最高优先级
    public UInt16 priority;
    //冲突处理 0-2 0正常播放 1插播 2强制切断当前消息插播
    public UInt16 conflict;
    //之后修改为Id
    public string content;
    public UInt16 speed;
}

//public struct TroopData
//{
//    public byte IdInSquad;
//    public ushort SoldierId;
//    public UInt16 GeneralId
//    {
//        get
//        {
//            if (PlayerDataManager.Instance.GeneralDict.ContainsKey(GeneralUUId))
//                return PlayerDataManager.Instance.GeneralDict[GeneralUUId].tabid;
//            else if (PlayerDataManager.Instance.CreateGeneralDict.ContainsKey(GeneralUUId))
//            {
//                //PlayerDataManager.Instance.CreateGeneralDict[GeneralUUId].
//                return 1;
//            }
//            else
//            {
//                return 0;
//            }
//        }
//    }
//    public bool IsEmpty
//    {
//        get
//        {
//            if (SoldierId == 0 && GeneralUUId == 0)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
//    }
//    public UInt64 GeneralUUId;
//    //1配表2自创3怪物
//    public byte generalType;
//    public uint SoldierNum;
//}

//TroopList[0] 主将 [1]军师
//public struct SquadData
//{
//    public int SquadId;
//    public int FlagId;
//    public emTroopUseType UseStatus;
//    public string SquadName;
//    public TroopData[] TroopList;
//    public bool isLock;
//    public bool isCanUnLock;
//    public SquadData Copy()
//    {
//        SquadData nData = this;
//        if (TroopList == null)
//        {
//            nData.TroopList = new TroopData[global_ConstDataDefine.TroopMemberNum];
//        }
//        else
//        {
//            nData.TroopList = new TroopData[TroopList.Length];
//        }

//        return nData;
//    }
//}

//public struct TaskInfo
//{
//    public DataDefine.STaskData taskData;
//    //是否已提交
//    public bool isDelivery;
//    //是否可以提交
//    public bool isCanDelivery;
    
//    public TaskInfo(DataDefine.STaskData data, bool IsDelivery = false)
//    {
//        taskData = data;
//        isCanDelivery = true;
//        foreach(var i in data.dictConditions)
//        {
//            if (!i.IsComplete)
//            {
//                isCanDelivery = false;
//            }
//        }
//        isDelivery = IsDelivery;
//    }
//}

public struct TroopEntityFightData
{
    //5分钟攻击次数  次/5分钟
    public float AttakSpd;
    //卡牌上的标准 米/每秒
    public float MoveSpd;
    public float Attak;
    public float Damage;
    public float Defence;
}

public struct TroopGeneralFightData
{
    public float Wit;
    public float Attack;
    public float Commander;
}

#endregion