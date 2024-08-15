using ACT;

using HarmonyLib;

using Il2CppSystem.Runtime.Remoting.Channels;

using System.Collections.Generic;

using UnityEngine;

namespace Ricca_Uncensor_Plugin;

[HarmonyPatch]
public class ArmorBreakPatch
{
    [HarmonyPatch(typeof(CharacterArmorBreaker), nameof(CharacterArmorBreaker.Initialize))]
    [HarmonyPostfix]
    public static void InitializePostfix(CharacterArmorBreaker __instance)
    {
        ArmorBreakGUI.Instance.AddCharacterArmorBreaker(__instance);
    }
}

public class ArmorBreakGUI : MonoBehaviour
{
    private static bool isEnabled = false;
    private static ArmorBreakGUI instance;
    private readonly List<CharacterArmorBreaker> breakers = new();
    private readonly string[] translateText = ["崭新出厂", "久经沙场", "破损不堪", "无了"];

    public static ArmorBreakGUI Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void OnGUI()
    {
        if (Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.F12)
                isEnabled = !isEnabled;
        }
        if (!isEnabled)
            return;

        EnsureBreakerListNotNull();
        float buttonHeight = 20, buttonWidth = 80;

        var y = 100f;
        for (var i = 0; i < breakers.Count; i++)
        {
            var breaker = breakers[i];
            var x = 10f;

            if (!breaker.isActiveAndEnabled)
                continue;

            GUI.Label(new(x, y, 200f, buttonHeight), breaker.transform.parent.gameObject.name);
            x += 200f + 10f;

            if (GUI.Button(new(x, y, buttonWidth, buttonHeight), translateText[0]))
                breaker.SetArmorLevel(-1);
            x += buttonWidth + 10f;
            for (int id = 0; id < breaker.LevelList.Count; id++)
            {
                if (GUI.Button(new(x, y, buttonWidth, buttonHeight), translateText[id + 1]))
                    breaker.SetArmorLevel(id);
                x += buttonWidth + 10f;
            }

            y += buttonHeight + 10f;
        }
    }

    private void EnsureBreakerListNotNull()
    {
        for (int i = breakers.Count - 1; i >= 0; i--)
        {
            if (breakers[i] == null || !breakers[i].IsProfileValid)
                breakers.RemoveAt(i);
        }
    }

    public void RemoveCharacterArmorBreaker(CharacterArmorBreaker instance)
    {
        breakers.Remove(instance);
    }

    public void AddCharacterArmorBreaker(CharacterArmorBreaker instance)
    {
        breakers.Add(instance);
    }
}