using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace Ricca_Uncensor_Plugin;

[BepInPlugin(GUID, MODNAME, VERSION)]

public class Plugin : BasePlugin
{
    public const string MODNAME = "Ricca_Uncensor_Plugin";

	public const string AUTHOR = "KazamataNeri";

	public const string GUID = "moe.KazamataNeri.Ricca_Uncensor_Plugin";

	public const string VERSION = "1.1.0";

	internal static ManualLogSource log;

	public static new void Log(LogLevel lv, object data)
	{
		ManualLogSource manualLogSource = Plugin.log;
		if (manualLogSource != null)
		{
			manualLogSource.Log(lv, data);
		}
	}

    public override void Load()
    {
        log = base.Log;
		Harmony harmony = new Harmony("moe.KazamataNeri.Ricca_Uncensor_Plugin");
		harmony.PatchAll();
		base.AddComponent<ArmorBreakerMonitor>();
		base.AddComponent<LanguageInit>();
		base.AddComponent<CheatMenu>();
		base.AddComponent<NoMosaic>();
    }
    
}