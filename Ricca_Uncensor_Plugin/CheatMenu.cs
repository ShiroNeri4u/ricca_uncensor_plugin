using System;
using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine;

namespace Ricca_Uncensor_Plugin;

public class Widget
{
    public string Text;
}


public sealed class CheatBar : Widget
{
    public Rect LabelRect;
    public Rect ToggleRect;
    public int Id;
    public CheatBar(String text, int id)
    {
        Text = text;
        Id = id;
    }
}

public sealed class BasicCheatToggle : Widget
{
    public bool Status;
    public Rect LabelRect;
    public Rect ToggleRect;
    public BasicCheatToggle(String text, bool status)
    {
        Text = text;
        Status = status;
    }
}

public sealed class CheatMenu : MonoBehaviour
{
    private static List<string> langtext;

    private static bool bMenu = false;

    public static CheatBar BasicPanel = new CheatBar(GetLanguageText(nameof(BasicPanel)), (int)Panel.BasicPanel);

    public static CheatBar AdvancedPanel = new CheatBar(GetLanguageText(nameof(AdvancedPanel)), (int)Panel.AdvancedPanel);

    public static CheatBar ArmorManagerPanel = new CheatBar(GetLanguageText(nameof(ArmorManagerPanel)), (int)Panel.ArmorManagerPanel);

    public static BasicCheatToggle PlayerHpNotDecrease = new BasicCheatToggle(GetLanguageText(nameof(PlayerHpNotDecrease)), false);

    public static BasicCheatToggle PlayerMpNotDecrease = new BasicCheatToggle(GetLanguageText(nameof(PlayerMpNotDecrease)), false);

    public static BasicCheatToggle SkillHasNotCoolTime = new BasicCheatToggle(GetLanguageText(nameof(SkillHasNotCoolTime)), false);

    public static BasicCheatToggle EnemyBeInstantlyKilled = new BasicCheatToggle(GetLanguageText(nameof(EnemyBeInstantlyKilled)), false);

    public static BasicCheatToggle CrystalNotDecrease = new BasicCheatToggle(GetLanguageText(nameof(CrystalNotDecrease)), false);

    public static BasicCheatToggle PlayerArmorBreakLock = new BasicCheatToggle(GetLanguageText(nameof(PlayerArmorBreakLock)), false);

    public static BasicCheatToggle NoMosaic = new BasicCheatToggle(GetLanguageText(nameof(NoMosaic)), true);

    public static float WidthScale;

    public static float HeightScale;

    public static float AspectRatio;

    public static float WindowWidth;

    public static float WindowHeight;

    public static float SingleWidth;

    public static float SingleHeight;

    private static Rect WindowRect;

    private static Rect ArmorPerfect;

    private static Rect ArmorFair;

    private static Rect ArmorDamaged;

    private static Rect ArmorRagged;

    public static int PanelId;

    public enum Panel
    {
        BasicPanel = 0,
        AdvancedPanel = 1,
        ArmorManagerPanel = 2
    }

    private static List<CheatBar> CheatBarList = new List<CheatBar>
    {
        BasicPanel,
        AdvancedPanel,
        ArmorManagerPanel
    };

    private static List<BasicCheatToggle> BasicToggleList = new List<BasicCheatToggle>
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
        InitCheatMenu();
        ArmorPerfect = new(50f * WidthScale, 30f * HeightScale, 30f * WidthScale - 7f, SingleHeight);
        ArmorFair = new(80f * WidthScale, 30f * HeightScale, 30f * WidthScale - 7f, SingleHeight);
        ArmorDamaged = new(110f * WidthScale, 30f * HeightScale, 30f * WidthScale - 7f, SingleHeight);
        ArmorRagged = new(140f * WidthScale, 30f * HeightScale, 30f * WidthScale - 7f, SingleHeight);
    }

    public void OnGUI()
    {
        if (Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.F1)
            {
                bMenu = !bMenu;
            }
        }
        if (bMenu)
        {
            WindowRect = GUI.ModalWindow(0, WindowRect, (GUI.WindowFunction)WindowFunction, GetLanguageText("CheatMenu"));
        }
    }

    private static void AspectRatioCalc()
    {
        AspectRatio = Math.Abs((float)Screen.width / (float)Screen.height);
        foreach (var ratio in StandardRatios)
        {
            if (CheckAspectRatio(AspectRatio, ratio.Value[0]))
            {
                WidthScale = Math.Abs(Screen.width / ratio.Value[1]);
                HeightScale = Math.Abs(Screen.height / ratio.Value[2]);
                WindowWidth = 200f * WidthScale;
                WindowHeight = 140f * HeightScale;
                SingleWidth = 14f * WidthScale;
                SingleHeight = 14f * HeightScale;
                WindowRect = new Rect(Math.Abs((Screen.width - WindowWidth) / 2), (5f * HeightScale), WindowWidth, WindowHeight);
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

    private static void InitCheatMenu()
    {
        var CheatBarX = 10f * WidthScale;
        foreach (var toggle in CheatBarList)
        {
            toggle.LabelRect = new Rect(CheatBarX + SingleWidth, SingleHeight, 46f * WidthScale, SingleHeight);
            toggle.ToggleRect = new Rect(CheatBarX, SingleHeight, 60f * WidthScale, SingleHeight);
            CheatBarX += 60f * HeightScale;
        }
        var BasicPanelY = 2 * SingleHeight;
        foreach (var toggle in BasicToggleList)
        {
            toggle.LabelRect = new Rect(14f * WidthScale, BasicPanelY, WindowWidth - SingleWidth, SingleHeight);
            toggle.ToggleRect = new Rect(WindowWidth - 20f * WidthScale, BasicPanelY, 14f, 14f);
            BasicPanelY += 14f * HeightScale;
        }
    }

    private static void WindowFunction(int windowId)
    {
        foreach (var toggle in CheatBarList)
        {
            LabelToggleBar(toggle);
        }
        switch (PanelId)
        {
            case (int)Panel.BasicPanel:
                foreach (var toggle in BasicToggleList)
                {
                    LabelToggle(toggle);
                }
                break;
            case (int)Panel.AdvancedPanel:
                break;
            case (int)Panel.ArmorManagerPanel:
                ArmorManager.EnsureNotEmpty();
                ArmorToolBar();
                break;
        }
        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }

    private static void LabelToggleBar(CheatBar toggle)
    {
        GUI.Label(toggle.LabelRect, toggle.Text);
        if (GUI.Toggle(toggle.ToggleRect, PanelId == toggle.Id, ""))
        {
            PanelId = toggle.Id;
        }
    }

    private static void LabelToggle(BasicCheatToggle toggle)
    {
        GUI.Label(toggle.LabelRect, toggle.Text);
        toggle.Status = GUI.Toggle(toggle.ToggleRect, toggle.Status, "");
    }

    private static void ArmorToolBar()
    {
        GUI.Label(ArmorPerfect, GetLanguageText(nameof(ArmorPerfect)));
        GUI.Label(ArmorFair, GetLanguageText(nameof(ArmorFair)));
        GUI.Label(ArmorDamaged, GetLanguageText(nameof(ArmorDamaged)));
        GUI.Label(ArmorRagged, GetLanguageText(nameof(ArmorRagged)));
        int i = 0;
        foreach (ArmorBreakerMonitor monitor in ArmorManager.Breakers)
        {
            var CurrentRow = i * SingleHeight + 44f * HeightScale;
            GUI.Label(new Rect(10f * WidthScale, CurrentRow, 50 * WidthScale, SingleHeight), monitor.Name);
            for (int index = 0; index < monitor.MaxLevel; index++)
            {
                bool origin = index == monitor.CurrentLevel;
                bool temp;
                temp = GUI.Toggle(new Rect(60f * WidthScale + index * 30f * WidthScale - 7f, CurrentRow, 14f, 14f), origin, "");
                if(temp == true && origin == false)
                {
                    monitor.Instance.SetArmorLevel(index - 1);
                }
            } 
            i++;
        }
    }

    public static string GetLanguageText(string key)
    {
        langtext = Language.LanguageText[key];
        return langtext[Language.NowLanguage];
    }
}
