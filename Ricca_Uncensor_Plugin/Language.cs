using System.Collections.Generic;
using UnityEngine;

namespace Ricca_Uncensor_Plugin;

public class Language
{
    public static int NowLanguage;
    public static Dictionary<string, List<string>> LanguageText = new Dictionary<string, List<string>>()
    {
        {"Player", new List<string>{"(玩家)", "(玩家)", "(プレイヤー)", "(플레이어)", "(Player)"}},
        {"PlayerHpNotDecrease", new List<string>{"※生命值锁定", "※生命值鎖定", "※ライフ値ロック", "※생명치 잠금", "※HP Lock"}},
        {"PlayerMpNotDecrease", new List<string>{"※魔法值锁定", "※魔法值鎖定", "※魔法値ロック", "※마법치 잠금", "※MP Lock"}},
        {"SkillHasNotCoolTime", new List<string>{"※技能无冷却", "※技能無冷卻", "※スキル冷却なし", "※스킬 쿨링 없음", "※No Skill CoolTime"}},
        {"EnemyBeInstantlyKilled", new List<string>{"※秒杀模式", "※秒殺模式", "※瞬殺モード", "※신속 격파 모드", "※Instant Kill Mode"}},
        {"CrystalNotDecrease", new List<string>{"※水晶不消耗", "※水晶不消耗", "※水晶は消耗しない", "※수정은 소모되지 않음", "※Crystal Not Decrease"}},
        {"PlayerArmorBreakLock", new List<string>{"※服装破损锁定", "※服裝破損度鎖定", "※衣類破損度ロック", "※의상 파손도 잠금", "※Armor Break Lock"}},
        {"NoMosaic", new List<string>{"※去马赛克", "※去馬賽克", "※モザイク除去", "※모자이크 제거", "※No Mosaic"}},
        {"CheatMenu", new List<string>{"作弊菜单", "作弊菜單", "「カンニング」メニュー", "커닝 메뉴", "Cheat Menu"}},
        {"BasicPanel", new List<string>{"基础面板", "基礎面板", "基礎パネル", "기본 패널", "BasicPanel"}},
        {"AdvancedPanel", new List<string>{"高级面板", "高級面板", "高級パネル", "고급 패널", "AdvPanel"}},
        {"ArmorManagerPanel", new List<string>{"服装面板", "服裝面板", "衣装パネル", "의상 패널", "ArmorPanel"}},
        {"ArmorPerfect", new List<string>{"完美", "完美", "完璧", "완벽", "Perfect"}},
        {"ArmorFair", new List<string>{"尚可", "尚可", "まだ", "尚可", "Fair"}},
        {"ArmorDamaged", new List<string>{"损坏", "損壞", "破損", "손상", "Damaged"}},
        {"ArmorRagged", new List<string>{"破损", "破損", "はそん", "파손", "Ragged"}},
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
    }
}