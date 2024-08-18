using System.Collections.Generic;
using UnityEngine;

namespace Ricca_Uncensor_Plugin;

public class Language
{
    public static int NowLanguage;
    public static Dictionary<string, List<string>> LanguageText = new Dictionary<string, List<string>>()
    {
        {"Player", new List<string>{"(玩家)", "", "", "", ""}},
        {"PlayerHpNotDecrease", new List<string>{"生命值锁定", "", "", "", ""}},
        {"PlayerMpNotDecrease", new List<string>{"魔力值锁定", "", "", "", ""}},
        {"SkillHasNotCoolTime", new List<string>{"技能无冷却", "", "", "", ""}},
        {"EnemyBeInstantlyKilled", new List<string>{"秒杀模式", "", "", "", ""}},
        {"CrystalNotDecrease", new List<string>{"水晶不消耗", "", "", "", ""}},
        {"PlayerArmorBreakLock", new List<string>{"服装破损锁定", "", "", "", ""}},
        {"NoMosaic", new List<string>{"去马赛克", "", "", "", ""}},
        {"CheatMenu", new List<string>{"作弊菜单", "", "", "", ""}},
        {"BasicPanel", new List<string>{"基础面板", "", "", "", ""}},
        {"AdvancePanel", new List<string>{"高级面板", "", "", "", ""}},
        {"ArmorManager", new List<string>{"服装管理器", "", "", "", ""}},
        {"ArmorPerfect", new List<string>{"完美", "", "", "", ""}},
        {"ArmorFair", new List<string>{"尚可", "", "", "", ""}},
        {"ArmorDamaged", new List<string>{"损坏", "", "", "", ""}},
        {"ArmorRagged", new List<string>{"破损", "", "", "", ""}},
    };

    public enum lang{
        chs = 0,
        cht = 1,
        jp = 2,
        kr = 3,
        en = 4,
    }
}

public class LanguageInit : MonoBehaviour
{
    public void Start()
    {
        switch (Application.systemLanguage)
		{
			case SystemLanguage.ChineseSimplified:
				Language.NowLanguage = (int)Language.lang.chs;
				break;
			case SystemLanguage.ChineseTraditional:
				Language.NowLanguage = (int)Language.lang.cht;
				break;
			case SystemLanguage.Japanese:
				Language.NowLanguage = (int)Language.lang.jp;
				break;
			case SystemLanguage.Korean:
				Language.NowLanguage = (int)Language.lang.kr;
				break;
			default:
				Language.NowLanguage = (int)Language.lang.en;
				break;
		}
        Plugin.Log(BepInEx.Logging.LogLevel.Info, "Load Language");
    }
}