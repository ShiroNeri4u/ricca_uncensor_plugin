using System;
using APP;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace Ricca_Uncensor_Plugin;

public class CheatPatch : MonoBehaviour
{

	[HarmonyPatch(typeof(CharacterActor), "set_CurrentHitPoint")]
	public class CharacterActor_HPCheat
	{
		[HarmonyPrefix]
		public static void Prefix( CharacterActor __instance, ref float value)
		{
			if (bPlayerCheat && __instance.IsPlayerActor)
			{
				value = __instance.hitPointMax;
			}
			else if (bKillCheat && !__instance.IsPlayerActor)
			{
			    value = 0;
			}
		}
	}

	[HarmonyPatch(typeof(CharacterActor), "set_CurrentMagicPoint")]
	public class CharacterActor_MPCheat
	{
		[HarmonyPrefix]
		public static void Prefix(CharacterActor __instance, ref float value)
		{
			if (bPlayerCheat && __instance.IsPlayerActor)
			{ 
				value = __instance.magicPointMax;
			}
		}
	}
	
	[HarmonyPatch(typeof(CharacterActor), "TrySpecialAttackStart")]
	public class CharacterActor_CoolTimeCheat
	{	
		[HarmonyPrefix]
		public static void Prefix(CharacterActor __instance, ref bool isCooling)
		{
			if (bSkillCheat && __instance.IsPlayerActor)
			{
				isCooling = false;
			}
		}
	}

    
	[HarmonyPatch(typeof(App.PlayerParameter), "SubMagicCrystalCount")]
	public class MagicCrystal_Cheat
	{
		[HarmonyPrefix]
		public static void Prefix(ref int sub)
		{
			if (bCrystalCheat)
			{
				sub = 0;
			}
		}
	}

	[HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "SetArmorLevel")]
	public class Armor_Cheat
	{
		[HarmonyPrefix]
		public static void Prefix(ACT.CharacterArmorBreaker __instance, ref int levelIndex)
		{
			if (bArmorCheat && __instance.parentActor.IsPlayerActor)
			{
				levelIndex = ArmorLevel;
			}
		}
	}

	[HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "Initialize")]
	public class Initialize
	{
		[HarmonyPostfix]
		public static void Postfix(ACT.CharacterArmorBreaker __instance)
		{
			if (__instance.parentActor.IsPlayerActor)
			{
				ArmorLevel = __instance.CurrentArmorLevelIndex;
			}
		}
	}

	private const string TitleName = "logoScene";

	private static bool bHelpShow = true;

	public static string toast_txt;

	private static DateTime dtStart;

	public static DateTime? dtStartToast = null;

	public static bool bNoMosaic = true;

	private static bool bPlayerCheat = false;

	private static bool bSkillCheat = false;

	private static bool bKillCheat = false;
	
	private static bool bCrystalCheat = false;

	private static bool bArmorCheat = false;

	private static int ArmorLevel;

	private enum ArmorStatus
	{
		Perfect = -1,
		Torn = 0,
		ripped = 1,
		Shredded  = 2
	}

	public GameObject CharacterActor;

	private static Action<ACT.CharacterArmorBreaker, int, ACT.CharacterArmorBreaker.CostumeUpdateMode> delegate_characterArmorBreaker_SetArmorLevel;

	public void Awake()
	{
		dtStart = DateTime.Now;
	}

	public void OnGUI()
	{
		GUIStyle buttonStyle = GUI.skin.button;

		buttonStyle.alignment = TextAnchor.MiddleLeft;

		buttonStyle.contentOffset = new Vector2(10f,0);

		if (SceneManager.GetActiveScene().name == TitleName && bHelpShow)
		{
			string text = "作弊补丁 By KazamataNeri";
			text += "\n※去码功能(自动生效)";
			text += "\n※F1 HP/MP锁定";
			text += "\n※F2 技能无冷却";
			text += "\n※F3 一击必杀";
			text += "\n※F4 魔法石不消耗";
			text += "\n※F5 衣服破损值锁定";
			int num = 0;
			int num2 = 20;
			string[] array = text.Split('\n');
			foreach (string text2 in array)
			{
				if (text2.Length > num2)
				{
					num2 = text2.Length;
				}
				num++;
			}
			if (GUI.Button(new Rect(10f, 30f, (float)num2 * 10f - 10f, (float)num * 16f + 15f), text))
			{
				bHelpShow = false;
			}
			if (DateTime.Now - dtStart > new TimeSpan(0, 0, 0, 15) || SceneManager.GetActiveScene().name != "logoScene")
			{
				bHelpShow = false;
			}
		}
		if (dtStartToast.HasValue)
		{
			GUI.Button(new Rect(10f, 10f, 190f, 20f), "\n" + toast_txt + "\n");
			DateTime now = DateTime.Now;
			DateTime? dateTime = dtStartToast;
			if (now - dateTime > new TimeSpan(0, 0, 0, 2))
			{
				dtStartToast = null;
			}
		}
		if (Event.current.type == EventType.KeyUp)
		{
			switch (Event.current.keyCode)
			{
				case KeyCode.F1:
					bPlayerCheat = !bPlayerCheat;
					toast_txt = "※HP/MP锁定 " + (bPlayerCheat ? "开启" : "关闭");
					dtStartToast = DateTime.Now;
					break;
				case KeyCode.F2:
					bSkillCheat = !bSkillCheat;
					toast_txt = "※技能无冷却 " + (bSkillCheat ? "开启" : "关闭");
					dtStartToast = DateTime.Now;
					break;
				case KeyCode.F3:
					bKillCheat = !bKillCheat;
					toast_txt = "※一击必杀" + (bKillCheat ? "开启" : "关闭");
					dtStartToast = DateTime.Now;
					break;
				case KeyCode.F4:
					bCrystalCheat = !bCrystalCheat;
					toast_txt = "※魔法石不消耗 " + (bCrystalCheat ? "开启" : "关闭");
					dtStartToast = DateTime.Now;
					break;
				case KeyCode.F5:
					bArmorCheat = !bArmorCheat;
					toast_txt = "※衣服破损程度锁定 " + (bArmorCheat ? "开启" : "关闭");
					dtStartToast = DateTime.Now;
					break;
				case KeyCode.F6:
					if(ArmorLevel > -1)
					{
						ArmorLevel--;
						SetCharacterArmorLevelImmediate();
					}
					toast_txt = "※衣服破损程度 " + Enum.GetName(typeof(ArmorStatus), ArmorLevel);
					break;
				case KeyCode.F7:
					if(ArmorLevel < 2)
					{
						ArmorLevel++;
						SetCharacterArmorLevelImmediate();
					}
					toast_txt = "※衣服破损程度 " + Enum.GetName(typeof(ArmorStatus), ArmorLevel);
					break;
			}
		}
	}

	private void SetCharacterArmorLevelImmediate()
	{
		delegate_characterArmorBreaker_SetArmorLevel ??=
			AccessTools.Method(typeof(ACT.CharacterArmorBreaker), "SetArmorLevel",new [] { typeof(int), typeof(ACT.CharacterArmorBreaker.CostumeUpdateMode) }, null)
			.CreateDelegate<Action<ACT.CharacterArmorBreaker, int, ACT.CharacterArmorBreaker.CostumeUpdateMode>>();
		foreach (var breaker in GameObject.FindObjectsOfType<ACT.CharacterArmorBreaker>())
		{
			if(breaker.parentActor.IsPlayerActor)
			{
				delegate_characterArmorBreaker_SetArmorLevel.Invoke(breaker, ArmorLevel, ACT.CharacterArmorBreaker.CostumeUpdateMode.Normal);
			}
		}
	}
}
