using System.Collections.Generic;
using HarmonyLib;

namespace Ricca_Uncensor_Plugin;

[HarmonyPatch]
public static class ArmorBreakerPatch
{
    [HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "Initialize")]

    [HarmonyPostfix]
    private static void InitializePostfix(ACT.CharacterArmorBreaker __instance)
    {
        ArmorManager.Breakers.Add(new ArmorBreakerMonitor(__instance, ArmorManager.Breakers.Count));
    }

    public static int GetMaxLevel(this ACT.CharacterArmorBreaker instance)
    {
        return instance.LevelList.Count + 1;
    }

    public static string GetName(this ACT.CharacterArmorBreaker instance)
    {
        return instance.parentActor.IsPlayerActor ? instance.parentActor.NickName + CheatMenu.GetLanguageText("Player") : instance.parentActor.NickName;
    }
}

public sealed class ArmorBreakerMonitor
{
    public ACT.CharacterArmorBreaker Instance;
    public bool IsValid => Instance != null && Instance.IsProfileValid;
    public string Name { get; private set; }
    public int MaxLevel => Instance.GetMaxLevel();
    public int CurrentLevel => Instance.currentArmorLevelIndex + 1;
    public ArmorBreakerMonitor(ACT.CharacterArmorBreaker _instance, int _index)
    {
        Instance = _instance;
        Name = Instance.GetName();
    }
}

public static class ArmorManager
{
    public static readonly List<ArmorBreakerMonitor> Breakers = [];
    public static void EnsureNotEmpty()
    {
        for (int index = Breakers.Count - 1; index >= 0; index--)
        {
            if (!Breakers[index].IsValid)
            {
                Breakers[index] = null;
                Breakers.RemoveAt(index);
            }
        }
    }
}