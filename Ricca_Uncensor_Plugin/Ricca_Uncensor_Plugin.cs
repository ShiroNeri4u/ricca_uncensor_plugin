using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using APP;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ricca_Uncensor_Plugin;

public class Ricca_Uncensor_Plugin : MonoBehaviour
{
	public static class Const
	{
		public const int ImageDiv = 16;

		public const int ImageCompareSize = 64;

		public const int MaxLoopTimeLimit = 10;
	}

	[HarmonyPatch(typeof(CharacterActor), "set_CurrentHitPoint")]
	public class CharacterActor_set_CurrentHitPoint
	{
		[HarmonyPrefix]
		public static void Prefix( CharacterActor __instance, ref float value)
		{
			if (bPlayerCheat && __instance.IsPlayerActor)
			{
				value = __instance.hitPointMax;
			}
			else if (bKillCheat && (!__instance.IsPlayerActor))
			{
			    value = 0;
			}
		}
	}

	[HarmonyPatch(typeof(CharacterActor), "set_CurrentMagicPoint")]
	public class CharacterActor_set_CurrentMagicPoint
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
	
	[HarmonyPatch(typeof(ACT.SpecialAttackData), "StartAttack")]
	public class ACT_SpecialAttackData_StartAttack
	{	
		[HarmonyPrefix]
		public static void Prefix(ref float coolTime)
		{
			if (bSkillCheat)
			{
				coolTime = 0;
			}
		}
	}
    
	[HarmonyPatch(typeof(App.PlayerParameter), "SubMagicCrystalCount")]
	public class App_PlayerParameter_SubMagicCrystalCount
	{
		[HarmonyPrefix]
		public static void Prefix(App.PlayerParameter __instance, ref int sub)
		{
			if (bCrystalCheat)
			{
				sub = 0;
			}
		}
	}

	[HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "Initialize")]
	[HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "BreakArmor")]
	[HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "RestoreArmor")]
	public class ACT_CharacterArmorBreaker
	{
		[HarmonyPostfix]
		public static void Postfix(ACT.CharacterArmorBreaker __instance)
		{
			if (bArmorCheat)
			{
				__instance.SetArmorLevel(ArmorLevel,ACT.CharacterArmorBreaker.CostumeUpdateMode.Force);
				__instance.InternalSetArmorLevel(ArmorLevel,ACT.CharacterArmorBreaker.CostumeUpdateMode.Force);
			}
		}
	}

	[HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "SetArmorLevel")]
	public class ACT_CharacterArmorBreaker_SetArmorLevel
	{
		[HarmonyPrefix]
		public static void Prefix(ACT.CharacterArmorBreaker __instance, ref int levelIndex)
		{
			if (bArmorCheat)
			{
				levelIndex = ArmorLevel;
			}
		}
	}

	[HarmonyPatch(typeof(ACT.CharacterArmorBreaker), "InternalSetArmorLevel")]
	public class ACT_CharacterArmorBreaker_InternalSetArmorLevel
	{
		[HarmonyPrefix]
		public static void Prefix(ACT.CharacterArmorBreaker __instance, ref int level)
		{
			if (bArmorCheat)
			{
				level = ArmorLevel;
			}
		}
	}
	private Renderer[] Renderers;

	private Texture2D[] Textures;

	private int RenderCount = -1;

	private int Renderindex = -1;

	private int TexCount = -1;

	private int Texindex = -1;

	private static DateTime? next_check = null;

	private static DateTime? next_check_tex = null;

	private static bool bCheck = false;

	private static bool bCheck_tex = false;

	private static string mod = "mod_";

	private TimeSpan loop_limit_span = new TimeSpan(0, 0, 0, 0, 1);

	private TimeSpan check_span = new TimeSpan(0, 0, 0, 1);

	private string OldSceneName = "";

	private const string TitleName = "logoScene";

	private static bool bHelpShow = true;

	public static string toast_txt = "";

	private static DateTime dtStart;

	public static DateTime? dtStartToast = null;

	private static bool bPlayerCheat = false;

	private static bool bSkillCheat = false;

	private static bool bKillCheat = false;
	
	private static bool bCrystalCheat = false;

	private static bool bArmorCheat = false;

	private static int ArmorLevel = -2;

	private static string Armortext;

	private static Dictionary<string, string> images = new Dictionary<string, string>();

	private static Dictionary<string, string> unique_keys = new Dictionary<string, string>();

	public static string imagesPath { get; set; }

	public void Awake()
	{
		Harmony harmony = new Harmony("moe.KazamataNeri.ricca_uncensor_plugin.patch");
		harmony.PatchAll();
		dtStart = DateTime.Now;
	}

	private void Start()
	{
		getImages();
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
			if (Event.current.keyCode == KeyCode.F1)
			{
				bPlayerCheat = !bPlayerCheat;
				toast_txt = "※HP/MP锁定 " + (bPlayerCheat ? "开启" : "关闭");
				dtStartToast = DateTime.Now;
			}
			else if (Event.current.keyCode == KeyCode.F2)
			{
				bSkillCheat = !bSkillCheat;
				toast_txt = "※技能无冷却 " + (bSkillCheat ? "开启" : "关闭");
				dtStartToast = DateTime.Now;
			}
			else if (Event.current.keyCode == KeyCode.F3)
			{
				bKillCheat = !bKillCheat;
				toast_txt = "※一击必杀" + (bKillCheat ? "开启" : "关闭");
				dtStartToast = DateTime.Now;
			}
			else if (Event.current.keyCode == KeyCode.F4)
			{
				bCrystalCheat = !bCrystalCheat;
				toast_txt = "※魔法石不消耗 " + (bCrystalCheat ? "开启" : "关闭");
				dtStartToast = DateTime.Now;
			}
			else if (Event.current.keyCode == KeyCode.F5)
			{
				if (ArmorLevel < 2){
					ArmorLevel++;
				}
				else
				{
					ArmorLevel = -2;
				}
				if (ArmorLevel == -2)
				{
					bArmorCheat = false;
				}
				else
				{
					bArmorCheat = true;
				}
				if ( ArmorLevel == -2)
				{
					Armortext = "关闭";
				}
				else if ( ArmorLevel == -1)
				{
					Armortext = "崭新如初";
				}
				else if ( ArmorLevel == 0)
				{
					Armortext = "轻微破损";
				}
				else if ( ArmorLevel == 1)
				{
					Armortext = "严重破损";
				}
				else if ( ArmorLevel == 2)
				{
					Armortext = "完全损坏";
				}

				bCrystalCheat = !bCrystalCheat;
				toast_txt = "※衣服破损值锁定 " + Armortext;
				dtStartToast = DateTime.Now;
			}
		}
		if (OldSceneName != SceneManager.GetActiveScene().name)
		{
			next_check = DateTime.Now;
			bCheck = false;
			OldSceneName = SceneManager.GetActiveScene().name;
		}
		if (bCheck)
		{
			try
			{
				DateTime dateTime2 = DateTime.Now + loop_limit_span;
				do
				{
					foreach (Material material in Renderers[Renderindex].materials)
					{
						if (material.shader.name.Contains("MosaicVCol"))
						{
							material.shader.maximumLOD = -2;
						}
					}
					Renderindex++;
				}
				while (Renderindex < RenderCount && DateTime.Now < dateTime2);
			}
			catch
			{
				next_check = DateTime.Now;
				bCheck = false;
			}
			if (Renderindex >= RenderCount)
			{
				next_check = DateTime.Now + check_span;
				bCheck = false;
			}
		}
		else
		{
			if (next_check.HasValue || DateTimeOffset.Now > next_check)
			{
				Renderer[] renderers = UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>();
				Renderers = renderers;
				RenderCount = Renderers.Count();
				if (RenderCount > 0)
					{
					Renderindex = 0;
					bCheck = true;
					}
					else
					{
					Renderindex = -1;
					}
			}
		}
		if (bCheck_tex)
		{
			try
			{
				DateTime dateTime3 = DateTime.Now + loop_limit_span;
				do
				{
					if (Textures[Texindex].name.Length > 0 && unique_keys.ContainsKey(Textures[Texindex].name))
					{
						ImageConversion.LoadImage(Textures[Texindex], (Il2CppStructArray<byte>)File.ReadAllBytes(unique_keys[Textures[Texindex].name]));
						Textures[Texindex].name = mod + Textures[Texindex].name;
					}
					Texindex++;
				}
				while (Texindex < TexCount && DateTime.Now < dateTime3);
			}
			catch
			{
				next_check_tex = DateTime.Now;
				bCheck_tex = false;
			}
			if (Texindex >= TexCount)
			{
				next_check_tex = DateTime.Now + check_span;
				bCheck_tex = false;
			}
		}
		if (next_check_tex.HasValue)
		{
			DateTime now = DateTime.Now;
			DateTime? dateTime = next_check_tex;
			if (!(now > dateTime))
			{
				return;
			}
		}
		Il2CppReferenceArray<UnityEngine.Object> source = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Texture2D>());
		Textures = source.Select((UnityEngine.Object x) => ((Il2CppObjectBase)(object)x).Cast<Texture2D>()).ToArray();
		TexCount = Textures.Count();
		if (TexCount > 0)
		{
			Texindex = 0;
			bCheck_tex = true;
		}
		else
		{
			Texindex = -1;
		}
	}

	private static void getImages()
	{
		imagesPath = Path.Combine(Paths.PluginPath, "ModImages");
		if (!Directory.Exists(imagesPath))
		{
			Directory.CreateDirectory(imagesPath);
		}
		try
		{
			string[] files = Directory.GetFiles(imagesPath, "*.png", SearchOption.AllDirectories);
			string[] array = files;
			foreach (string text in array)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				if (!images.ContainsKey(fileNameWithoutExtension))
				{
					images.Add(fileNameWithoutExtension, text);
					string text2 = fileNameWithoutExtension;
					int num = fileNameWithoutExtension.LastIndexOf('_');
					if (num > 0)
					{
						text2 = fileNameWithoutExtension.Substring(0, num);
						if (true)
						{
							if (unique_keys.ContainsKey(text2))
							{
								Plugin.Log(LogLevel.Warning, "Unique key " + text2 + " duplicated.");
							}
							else
							{
								unique_keys.Add(text2, text);
							}
						}
					}
					else if (unique_keys.ContainsKey(text2))
					{
						Plugin.Log(LogLevel.Warning, "Unique key " + text2 + " duplicated.");
					}
					else
					{
						unique_keys.Add(text2, text);
					}
				}
				else
				{
					Plugin.Log(LogLevel.Info, "key " + fileNameWithoutExtension + " is already exists.");
				}
			}
			Plugin.Log(LogLevel.Info, $"Unique   keys {unique_keys.Count} counts");
		}
		catch (Exception ex)
		{
			Plugin.Log(LogLevel.Error, ex.Message);
		}
	}

	public static Texture2D duplicateTexture(Texture2D source)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
		RenderTexture active = RenderTexture.active;
		Graphics.Blit(source, temporary);
		RenderTexture.active = temporary;
		Texture2D texture2D = new Texture2D(source.width, source.height, TextureFormat.ARGB32, mipChain: true);
		texture2D.ReadPixels(new Rect(0f, 0f, temporary.width, temporary.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
		return texture2D;
	}
}
