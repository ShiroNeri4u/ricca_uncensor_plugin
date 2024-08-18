using APP;
using HarmonyLib;

namespace Ricca_Uncensor_Plugin;

[HarmonyPatch]
public class BasicCheatPatch
{

	[HarmonyPatch(typeof(CharacterActor), "CurrentHitPoint", MethodType.Setter)]

	[HarmonyPrefix]
	public static void CurrentHitPointPrefix(CharacterActor __instance, ref float value)
	{
		if (CheatMenu.PlayerHpNotDecrease.Status && __instance.IsPlayerActor)
		{
			value = __instance.hitPointMax;
		}
		else if (CheatMenu.EnemyBeInstantlyKilled.Status && !__instance.IsPlayerActor)
		{
		    value = 0;
		}
	}

	[HarmonyPatch(typeof(CharacterActor), "CurrentMagicPoint", MethodType.Setter)]

	[HarmonyPrefix]
	public static void CurrentMagicPointPrefix(CharacterActor __instance, ref float value)
	{
		if (CheatMenu.PlayerMpNotDecrease.Status && __instance.IsPlayerActor)
		{ 
			value = __instance.magicPointMax;
		}
	}
	
	[HarmonyPatch(typeof(CharacterActor), "TrySpecialAttackStart")]

	[HarmonyPrefix]
	public static void TrySpecialAttackStartPrefix(CharacterActor __instance, ref bool isCooling)
	{
		if (CheatMenu.SkillHasNotCoolTime.Status && __instance.IsPlayerActor)
		{
			isCooling = false;
		}
	}

	[HarmonyPatch(typeof(App.PlayerParameter), "SubMagicCrystalCount")]

	[HarmonyPrefix]
	public static void SubMagicCrystalCountPrefix(ref int sub)
	{
		if (CheatMenu.CrystalNotDecrease.Status)
		{
			sub = 0;
		}
	}

	[HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "RestoreArmor")]
	[HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "BreakArmor")]

	[HarmonyPrefix]
	public static bool ArmorBreakerLockPrefix(ACT.CharacterArmorBreaker __instance)
	{
		if (CheatMenu.PlayerArmorBreakLock.Status && __instance.parentActor.IsPlayerActor)
		{
			return false;
		}
		else return true;
	}
}