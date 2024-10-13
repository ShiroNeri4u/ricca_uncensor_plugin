using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ricca_Uncensor_Plugin;

[HarmonyPatch]
public static class ArmorBreakerPatch
{
    [HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "Initialize")]

    [HarmonyPostfix]
    private static void InitializePostfix(ACT.CharacterArmorBreaker __instance)
    {
        ArmorManager.Breakers.Add(new ArmorBreakerMonitor(__instance));
    }

    public static void SetCharacterArmorLevel(this ACT.CharacterArmorBreaker instance, int level)
    {
        instance.SetArmorLevel(level - 1, ACT.CharacterArmorBreaker.CostumeUpdateMode.Normal);
    }

    public static int GetMaxLevel(this ACT.CharacterArmorBreaker instance)
    {
        return instance.LevelList.Count;
    }

    public static string GetName(this ACT.CharacterArmorBreaker instance)
    {
        return instance.parentActor.IsPlayerActor ? instance.parentActor.NickName + CheatMenu.GetLanguageText("Player") : instance.parentActor.NickName;
    }
}

public sealed class ArmorBreakerMonitor
{
    public readonly ACT.CharacterArmorBreaker instance;
    public bool IsValid => instance != null && instance.IsProfileValid;
    public string Name{get; private set;}
    public int MaxLevel{get; private set;}
    public int CurrentLevel => instance.CurrentArmorLevelIndex + 1;
    public Rect LabelRect;
    public Rect[] ToggleRect;
    public bool[] Toggle;
    public ArmorBreakerMonitor(ACT.CharacterArmorBreaker _instance)
    {
        instance = _instance;
        if(IsValid)
        {
            Name = instance.GetName();
            MaxLevel = instance.GetMaxLevel();
            float CurrentRow = ArmorManager.Breakers.Count * CheatMenu.SingleHeight + 30f;
            LabelRect = new Rect(0, CurrentRow, 60f * CheatMenu.WidthScale, CheatMenu.SingleHeight);
            ToggleRect = new Rect[MaxLevel];
            for(var index = -1; index < MaxLevel; index++)
            {
                if(index == CurrentLevel)
                {
                    Toggle[index] = true;
                }
                else
                {
                    Toggle[index] = false;
                }
                ToggleRect[index] = new Rect(75f * CheatMenu.WidthScale + index * 30f * CheatMenu.WidthScale - 7f, CurrentRow, 14f, 14f);
            }
        }
    }

    public void SetArmorLevel(int level)
    {
        instance.SetCharacterArmorLevel(level);
    }

    public void TweakHeghit()
    {
        LabelRect.y -= CheatMenu.SingleHeight;
        for(var index = -1; index < MaxLevel; index++)
        {
            ToggleRect[index].y -= CheatMenu.SingleHeight;
        }
    }
}

public sealed class ArmorManager : MonoBehaviour
{
    public static readonly List<ArmorBreakerMonitor> Breakers = new();
    private void OnGUI()
    {
        for(var index = Breakers.Count - 1; index >= 0; index--)
        {
            if(!Breakers[index].IsValid)
            {
                for(var current = index + 1; current + 1 < Breakers.Count; current++)
                {
                    Breakers[current].TweakHeghit();
                }
                Breakers[index] = null;
                Breakers.RemoveAt(index);
            }
            else
            {
                CheatMenu.ArmorLabelToolbar(Breakers[index]);
            }
        }
    }
}