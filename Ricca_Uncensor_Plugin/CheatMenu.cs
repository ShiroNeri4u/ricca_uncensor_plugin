using System;
using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine;

namespace Ricca_Uncensor_Plugin;

public class CheatMenu : MonoBehaviour
{
    /* 作弊数值调整
    private static bool bPlayerDamageReduction;

    private static float PlayerDamageReduction;

    private static bool bEnemyDamageReduction;

    private static float EnemyDamageReduction;

    private static bool bPlayerMpConsumption;

    private static float PlayerMpConsumption;*/
    private static List<string> langtext;

    private static bool bMenu = false;
    private static int SelectLevel;
    public class Toggle
    {
        public string Name{get; set;}
        public bool Status{get; set;}
    }

    public static Toggle PlayerHpNotDecrease = new Toggle()
    {
        Name = "※" + GetLanguageText("PlayerHpNotDecrease"),
        Status = false
    };

    public static Toggle PlayerMpNotDecrease = new Toggle()
    {
        Name = "※" + GetLanguageText("PlayerMpNotDecrease"),
        Status = false
    };

    public static Toggle SkillHasNotCoolTime = new Toggle()
    {
        Name = "※" + GetLanguageText("SkillHasNotCoolTime"),
        Status = false
    };

    public static Toggle EnemyBeInstantlyKilled = new Toggle()
    {
        Name = "※" + GetLanguageText("EnemyBeInstantlyKilled"),
        Status = false
    };

    public static Toggle CrystalNotDecrease = new Toggle()
    {
        Name = "※" + GetLanguageText("CrystalNotDecrease"),
        Status = false
    };

    public static Toggle PlayerArmorBreakLock = new Toggle()
    {
        Name = "※" + GetLanguageText("PlayerArmorBreakLock"),
        Status = false
    };

    public static Toggle NoMosaic = new Toggle()
    {
        Name = "※" + GetLanguageText("NoMosaic"),
        Status = true
    };

    private static float WidthScale;

    private static float HeightScale;

    private static float AspectRatio;

    private static float WindowWidth;

    private static float WindowHeight;

    private static Rect WindowRect;

    private static float BarWidth;

    private static float BarHeight;

    private static Rect BarRect;

    public static int PanelId;

    public enum Panel
    {
        BasicsPanel = 0,
        AdvancedPanel = 1,
        ArmorManagerPanel = 2
    }

    //private static string[] PanelName = new string[] {GetLanguageText("BasicPanel"), GetLanguageText("AdvancePanel"), GetLanguageText("ArmorManager")};
    private static string[] PanelName = {"1","2","3"};

    private static List<Toggle> BasicsFuncList = new List<Toggle>
    {
        PlayerHpNotDecrease,
        PlayerMpNotDecrease,
        SkillHasNotCoolTime,
        EnemyBeInstantlyKilled,
        CrystalNotDecrease,
        PlayerArmorBreakLock,
        NoMosaic
    };

    private static Dictionary<string, List<float>> StandardRatios = new Dictionary<string, List<float>>()
    {
        {"16:9", new List<float>{16f/9f, 1280f, 720f}},
        {"16:10", new List<float>{16f/10f, 1280f, 800f}},
        {"4:3", new List<float>{4f/3f, 1280, 960f}}
    };

    public static Dictionary<int, string> AmrorStatus = new Dictionary<int, string>()
    {
        {0, "ArmorPerfect"},
        {1, "ArmorFair"},
        {2, "ArmorDamaged"},
        {3, "ArmorRagged"}
    };


    public void Start()
    {
        AspectRatioCalc();
    }

    public void OnGUI()
    {
        if (Event.current.type == EventType.KeyUp)
        {
            if(Event.current.keyCode == KeyCode.F1)
            {
                bMenu = !bMenu;
            }
        }
        if(bMenu)
        {
            WindowRect = GUI.Window(0, WindowRect, (GUI.WindowFunction)WindowFunction, GetLanguageText("CheatMenu"));
        }
    }

    private static void AspectRatioCalc()
    {
        AspectRatio = Math.Abs((float)Screen.width / (float)Screen.height);     
        foreach(var ratio in StandardRatios)
        {
            if(CheckAspectRatio(AspectRatio, ratio.Value[0]))
            {
                WidthScale = Math.Abs((float)Screen.width / ratio.Value[1]);
                HeightScale = Math.Abs((float)Screen.height / ratio.Value[2]);
                WindowWidth = 200f * WidthScale;
                WindowHeight = 150f * HeightScale;
                WindowRect = new Rect(Math.Abs(((float)Screen.width - WindowWidth)/ 2), (5f * HeightScale),WindowWidth , WindowHeight);
                break;
            }
        }
    }

    private static bool CheckAspectRatio(float aspectRatio, float targetRatio)
    {
        const float tolerance = 0.01f;
        if (Math.Abs(aspectRatio - targetRatio) <= tolerance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private static void WindowFunction(int windowId)
    {
        BarWidth = 180f * WidthScale;
        BarHeight = 15f * HeightScale;
        BarRect = new Rect((WindowWidth - BarWidth) / 2, 12f * HeightScale, BarWidth, BarHeight);
        PanelId = GUI.Toolbar(BarRect, PanelId, (Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray)PanelName);
        switch(PanelId)
        {
            case (int)Panel.BasicsPanel:
                var BasicsPanelY = 12f * HeightScale + BarHeight;
                foreach(var func in BasicsFuncList)
                {
                    LabelToggle(func, BasicsPanelY);
                    BasicsPanelY += 15f * HeightScale;
                }
                break;
            case (int)Panel.AdvancedPanel:
                break;
            case (int)Panel.ArmorManagerPanel:
                
                break;
        }
        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }

    private static void LabelToggle(Toggle toggle, float y)
    {
        GUI.Label(new Rect(15f * WidthScale, y, WindowWidth - 15f * WidthScale, 15f * HeightScale), toggle.Name);
        toggle.Status = GUI.Toggle(new Rect(WindowWidth - 20f * WidthScale, y, 15f * WidthScale, 15f * HeightScale), toggle.Status, "");
    }

    private static void ArmorLabelToobar(int id, float y)
    {
        var ListLength = ArmorBreakerMonitor.instance.GetMaxLevel(id);
        var List = new string[ListLength];
        var select = ArmorBreakerMonitor.instance.GetCurrenctLevel(id);
        for(int count = 0; count < ListLength; count ++)
        {
            List[count] = GetLanguageText(AmrorStatus[count]);
        }
        GUI.Label(new Rect(15f * WidthScale, y, WindowWidth - 15f * WidthScale, 15f * HeightScale), ArmorBreakerMonitor.instance.GetName(id));
        var ButtonWidth = 100f * HeightScale;
        SelectLevel = GUI.Toolbar(new Rect(WindowWidth - 20f * WidthScale, y, ButtonWidth * (float)ListLength, 15f * HeightScale), select, List);
        if(SelectLevel != select)
        {
            ArmorBreakerMonitor.instance.SetCharacterArmorLevel(id, SelectLevel);
        }
    }

    public static string GetLanguageText(string key)
    {
        langtext = Language.LanguageText[key];
        return langtext[Language.NowLanguage];
    }
}
