using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ricca_Uncensor_Plugin;

public class NoMosaic : MonoBehaviour
{  
    private void Start()
	{
		getImages();
	}

    private void Update() 
    {
		if(CheatMenu.NoMosaic.Status)
		{
			MosaicRemove();
		}
	}

	private static bool bMosaicCheck = false;

	private static bool bMosaic_TexCheck = false;

	private int Mosaic_RenderCount = -1;

	private int Mosaic_Renderindex = -1;

	private int Mosaic_TexCount = -1;

	private int Mosaic_Texindex = -1;

	private static string mod = "mod_";

	private string OldSceneName;

	private Renderer[] Renderers;

	private Texture2D[] Textures;

	private static DateTime? Mosaic_NextCheck = null;

	private static DateTime? Mosaic_NextTexCheck = null;

	private TimeSpan Mosaic_LoopLimitSpan = new TimeSpan(0, 0, 0, 0, 1);

	private TimeSpan Mosaic_Checkspan = new TimeSpan(0, 0, 0, 1);

	public void MosaicRemove ()
	{
		if (OldSceneName != SceneManager.GetActiveScene().name)
		{
			Mosaic_NextCheck = DateTime.Now;
			bMosaicCheck = false;
			OldSceneName = SceneManager.GetActiveScene().name;
		}
        if (bMosaicCheck)
		{
			try
			{
				DateTime dateTime2 = DateTime.Now + Mosaic_LoopLimitSpan;
				do					
                {
					foreach (Material material in Renderers[Mosaic_Renderindex].materials)
					{
						if (material.shader.name.Contains("MosaicVCol"))
						{
							material.shader.maximumLOD = -2;
						}
					}
				Mosaic_Renderindex++;
				}
				while (Mosaic_Renderindex < Mosaic_RenderCount && DateTime.Now < dateTime2);
			}
			catch
			{
			    Mosaic_NextCheck = DateTime.Now;
		    	bMosaicCheck = false;
	    	}
	        if (Mosaic_Renderindex >= Mosaic_RenderCount)
	        {
		        Mosaic_NextCheck = DateTime.Now + Mosaic_Checkspan;
		        bMosaicCheck = false;
	        }
	    }
		else
		{
			if (Mosaic_NextCheck.HasValue || DateTimeOffset.Now > Mosaic_NextCheck)
			{
				Renderer[] renderers = FindObjectsOfType<SkinnedMeshRenderer>();
				Renderers = renderers;
				Mosaic_RenderCount = Renderers.Count();
				if (Mosaic_RenderCount > 0)
					{
					Mosaic_Renderindex = 0;
					bMosaicCheck = true;
					}
				else
					{
					Mosaic_Renderindex = -1;
					}
			}
		}
		if (bMosaic_TexCheck)
		{
			try
			{
				DateTime dateTime3 = DateTime.Now + Mosaic_LoopLimitSpan;
				do
				{
					if (Textures[Mosaic_Texindex].name.Length > 0 && unique_keys.ContainsKey(Textures[Mosaic_Texindex].name))
					{
						ImageConversion.LoadImage(Textures[Mosaic_Texindex], (Il2CppStructArray<byte>)File.ReadAllBytes(unique_keys[Textures[Mosaic_Texindex].name]));
						Textures[Mosaic_Texindex].name = mod + Textures[Mosaic_Texindex].name;
					}
					Mosaic_Texindex++;
				}
				while (Mosaic_Texindex < Mosaic_TexCount && DateTime.Now < dateTime3);
			}
			catch
			{
				Mosaic_NextTexCheck = DateTime.Now;
				bMosaic_TexCheck = false;
			}
			if (Mosaic_Texindex >= Mosaic_TexCount)
				{
				Mosaic_NextTexCheck = DateTime.Now + Mosaic_Checkspan;
				bMosaic_TexCheck = false;
			}
		}
		if (Mosaic_NextTexCheck.HasValue)
		{
			DateTime now = DateTime.Now;
			DateTime? dateTime = Mosaic_NextTexCheck;
			if (!(now > dateTime))
			{
				return;
			}
		}
		Il2CppReferenceArray<UnityEngine.Object> source = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Texture2D>());
		Textures = source.Select((UnityEngine.Object x) => ((Il2CppObjectBase)(object)x).Cast<Texture2D>()).ToArray();
		Mosaic_TexCount = Textures.Count();
		if (Mosaic_TexCount > 0)
		{
			Mosaic_Texindex = 0;
			bMosaic_TexCheck = true;
		}
		else
		{
			Mosaic_Texindex = -1;
		}
	}
    private static Dictionary<string, string> images = new Dictionary<string, string>();

	private static Dictionary<string, string> unique_keys = new Dictionary<string, string>();

	public static string imagesPath { get; set; }

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