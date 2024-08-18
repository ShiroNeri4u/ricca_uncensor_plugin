using ACT;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ricca_Uncensor_Plugin;

[HarmonyPatch]
public class ArmorBreakerPatch
{
    [HarmonyPatch(typeof(CharacterArmorBreaker), nameof(CharacterArmorBreaker.Initialize))]

    [HarmonyPostfix]
    public static void InitializePostfix(CharacterArmorBreaker __instance)
    {
        ArmorBreakerMonitor.instance.AddInstance(__instance);
    }
}

public class ArmorBreakerMonitor : MonoBehaviour
{
    private static ArmorBreakerMonitor _instance;
    public static ArmorBreakerMonitor instance => _instance;
    public readonly List<CharacterArmorBreaker> Instance = new();
    public void AddInstance(CharacterArmorBreaker __instance)
    {
        Instance.Add(__instance);
    }

    public void RemoveInstance(int id)
    {
        Instance.RemoveAt(id);
    }

    public string GetName(int id)
    {
        return Instance[id].parentActor.IsPlayerActor ? Instance[id].parentActor.NickName + CheatMenu.GetLanguageText("Player") : "Instance[id].parentActor.NickName";
    }

    public int GetCurrenctLevel(int id)
    {
        return Instance[id].CurrentArmorLevelIndex + 1;
    }

    public int GetMaxLevel(int id)
    {
        return Instance[id].LevelList.Count + 1;
    }

    public void SetCharacterArmorLevel(int id, int level)
    {
        Instance[id].SetArmorLevel(level - 1, CharacterArmorBreaker.CostumeUpdateMode.Normal);
    }

    private void EnsureBreakerListNotNull()
    {
        for (int i = Instance.Count - 1; i >= 0; i--)
        {
            if (Instance[i] == null || !Instance[i].IsProfileValid)
                RemoveInstance(i);
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        _instance = this;
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void Update()
    {
        EnsureBreakerListNotNull();
    }
}