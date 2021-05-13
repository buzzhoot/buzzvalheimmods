using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
//using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using Jotunn.Configs;
using Jotunn.InGameConfig;
using Jotunn.Entities;
using Jotunn.Utils;

namespace AllTameable
{
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Major)]

	[BepInPlugin("buzz.valheim.AllTameable", "AllTameable", "2.0.2")]
	class Plugin : BaseUnityPlugin
	{
		#region Var
		#region Config
		public static ConfigEntry<int> nexusID;
		public static ConfigEntry<string> cfg;
		public static ConfigEntry<bool> HatchingEgg;
		public static ConfigEntry<int> HatchingTime;
		public static ManualLogSource logger;
		#endregion
		#region Data
		public class TameTable : ICloneable
		{
			//public string name;
			public bool commandable { get; set; } = true;
			public float tamingTime { get; set; } = 600;
			public float fedDuration { get; set; } = 300;
			public float consumeRange { get; set; } = 2;
			public float consumeSearchInterval { get; set; } = 5;
			public float consumeHeal { get; set; } = 10;
			public float consumeSearchRange { get; set; } = 30;
			public string consumeItems { get; set; } = "RawMeat";
			public bool changeFaction { get; set; } = true;
			public bool procretion { get; set; } = true;
			public int maxCreatures { get; set; } = 5;
			public float pregnancyChance { get; set; } = 0.33f;
			public float pregnancyDuration { get; set; } = 10f;
			public float growTime { get; set; } = 60;
			public object Clone()
			{
				return this.MemberwiseClone();
			}
		}
		public static Dictionary<string, TameTable> cfgList = new Dictionary<string, TameTable>();
		public static TameTable CfgTable;
		public static List<string> ThxList = new List<string> { "deftesthawk", "buzz", "lordbugx", "hawksword", "zarboz" };
		public static EffectList.EffectData firework = new EffectList.EffectData();
		#endregion Data
		#region plugin
		public static bool loaded = false;
		public static GameObject Root;
		public static PrefabManager prefabManager;
		public static PetManager petManager;
		public static ConfigManager configManager;
		public static ConfigEntry<string> Animal1;
		public static ConfigEntry<string> Animal2;
		#endregion plugin

		#endregion Var

		#region  Mono
		private void Awake()
		{
			logger = base.Logger;
			nexusID = base.Config.Bind<int>("Nexus", "NexusID", 478, "Nexus mod ID for updates");
			HatchingTime = base.Config.Bind<int>("2DragonEgg", "hatching time", 300, "how long will egg become a drake");
			HatchingEgg = base.Config.Bind<bool>("2DragonEgg", "enable egg hatching", true, "this alse enable tamed drake spawn eggs");
			cfg = base.Config.Bind<string>("1General", "Settings",
				"Hatchling,true,600,300,30,10,300,10,RawMeat,true,true,5,0.33,10,300",
				"name,commandable,tamingTime,fedDuration,consumeRange,consumeSearchInterval,consumeHeal,consumeSearchRange,consumeItem:consumeItem,changeFaction,procretion,maxCreatures,pregnancyChance,pregnancyDuration,growTime,;next one;...;last one");
			Animal1 = Config.Bind("1General", "Settings","Hatchling,true,600,300,30,10,300,10,RawMeat,true,true,5,0.33,10,300", new ConfigDescription("name,commandable,tamingTime,fedDuration,consumeRange,consumeSearchInterval,consumeHeal,consumeSearchRange,consumeItem:consumeItem,changeFaction,procretion,maxCreatures,pregnancyChance,pregnancyDuration,growTime", null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
			Animal2 = Config.Bind("1General", "Settings", "Hatchling,true,600,300,30,10,300,10,RawMeat,true,true,5,0.33,10,300", new ConfigDescription("Server side string", null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
			loaded = initCfg();
			CfgTable = new TameTable();
			string list = "Your list has: ";
			foreach (var item in cfgList.Keys)
			{
				list += item + "  ";
			}
			Root = new GameObject("AllTameable Root");

			prefabManager = Root.AddComponent<PrefabManager>();
			petManager = Root.AddComponent<PetManager>();

			var SM_Root = new GameObject("ConfigManager").transform;
			SM_Root.SetParent(Plugin.Root.transform);
			SM_Root.gameObject.SetActive(false);
			configManager = SM_Root.gameObject.AddComponent<ConfigManager>();
			configManager.debugInfo = list;
			configManager.obj = CfgTable;
			configManager.title = "All Tameable Setup " + (loaded ? "Loaded" : "!!Load Fail!!");
			//configManager.gameObject.SetActive(false);
			DontDestroyOnLoad(Root);

			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

			DBG.blogInfo("AllTameable Loadded");
		}
		#endregion  Mono

		#region patch
		#region odb

		#endregion odb
		#region ZNS
		[HarmonyPatch(typeof(ZNetScene), "Awake")]
		private static class ZNetScene_Awake_Patch
		{
			private static void Postfix(ZNetScene __instance)
			{
				prefabManager.PostZNS();
				GetFirework();
				if (Player.m_localPlayer != null) { SetPlayerSpwanEffect(); }
			}
		}
		[HarmonyPatch(typeof(ZNetScene), "Shutdown")]
		private static class Postfix_ZNetScene_ShutDown
		{
			private static void Postfix()
			{
				prefabManager.Clear();
			}
		}
		#endregion ZNS
		#region Tame
		[HarmonyPatch(typeof(Tameable), "Tame")]
		private static class Patch_Tameable_Tame
		{
			private static void Postfix(Tameable __instance)
			{
				string a = __instance.name.Replace("(Clone)", "");
				if (cfgList.ContainsKey(a) && cfgList[a].changeFaction)
					__instance.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
			}
		}


		#endregion Tame
		#region Misc
		[HarmonyPatch(typeof(Console), "InputText")]
		private static class Patch_Console_InputText
		{
			private static void Prefix()
			{
				InputCMD(global::Console.instance.m_input.text);
			}
		}
		[HarmonyPatch(typeof(Fireplace), "UseItem")]
		private static class Prefix_Fireplace_UseItem
		{
			private static bool Prefix(Fireplace __instance, Humanoid user, ItemDrop.ItemData item, ref bool __result)
			{
				if (!HatchingEgg.Value)
				{
					return true;
				}
				if (item.m_dropPrefab.name == "DragonEgg")
				{
					if (!__instance.IsBurning())
					{
						user.Message(MessageHud.MessageType.Center, Localization.instance.Localize("You need to add more fuel before you are hatch the egg"));
						__result = true;
						return false;
					}
					Inventory inventory = user.GetInventory();
					user.Message(MessageHud.MessageType.Center, Localization.instance.Localize("The egg is hatching"));
					inventory.RemoveItem(item, 1);
					var go = Instantiate(ZNetScene.instance.GetPrefab("HatchingDragonEgg"));
					go.transform.localPosition = user.transform.position + new Vector3(0, 2, 0);
					__result = true;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(Player), "OnSpawned")]
		private static class Prefix_Player_SetLocalPlayer
		{
			private static void Prefix()
			{
				var pname = Player.m_localPlayer.GetPlayerName();
				if (ThxList.Contains(pname.ToLower()))
				{
					SetPlayerSpwanEffect();
				}
			}
		}
		#endregion Misc
		#endregion

		#region Tool
		public static void InputCMD(string cmd)
		{
			if (cmd.Length > 0)
			{
				if (cmd.StartsWith(" "))
				{
					cmd = cmd.Remove(0, 1);
				}
				if (cmd == "/buzztame")
				{
					var go = Plugin.configManager.gameObject;
					go.SetActive(!go.activeSelf);
				}
			}
		}
		public bool initCfg()
		{
			if (cfg.Value == "")
			{
				DBG.blogWarning("CFG is empty");
				return false;
			}
			string[] list = cfg.Value.Split(new char[] { ';' });
			foreach (string tt in list)
			{
				string[] set = tt.Split(new char[] { ',' });

				if (set.Length == 8 || set.Length == 9)
				{
					DBG.blogWarning("Upadate your cfg : " + tt);
					return false;
				}
				if (set.Length != 15)
				{
					DBG.blogWarning("Not enought args : " + tt);
					return false;
				}
				TameTable table = new TameTable();
				string name = set[0];
				if (set[1] == "true")
				{
					table.commandable = true;
				}
				else { table.commandable = false; }

				try
				{
					table.tamingTime = float.Parse(set[2]);
					table.fedDuration = float.Parse(set[3]);
					table.consumeRange = float.Parse(set[4]);
					table.consumeSearchInterval = float.Parse(set[5]);
					table.consumeHeal = float.Parse(set[6]);
					table.consumeSearchRange = float.Parse(set[7]);
				}
				catch (Exception e)
				{
					DBG.blogWarning("wrong syntax : " + tt);
					logger.LogError(e);
					return false;
				}
				table.consumeItems = set[8];

				if (set[9] == "true")
				{
					table.changeFaction = true;
				}
				if (set[10] == "true")
				{
					table.procretion = true;
				}

				try
				{
					float a = 0.33f;
					table.maxCreatures = int.Parse(set[11]);
					if (Single.TryParse(set[12], out a))
					{
						table.pregnancyChance = a;
					}

					table.pregnancyDuration = Single.Parse(set[13]);
					table.growTime = Single.Parse(set[14]);
				}
				catch (Exception e)
				{
					DBG.blogWarning("wrong syntax : " + tt);
					logger.LogError(e);
					return false;
				}
				cfgList.Add(name, table);
			}
			DBG.blogInfo("TameTable Loaded :" + cfgList.Count);
			return true;
		}
		#endregion

		#region Feature
		private static void GetFirework()
		{
			firework.m_prefab = ZNetScene.instance.GetPrefab("fire_pit").GetComponent<Fireplace>().m_fireworks;
			firework.m_enabled = true;
		}
		private static void SetPlayerSpwanEffect()
		{

			if (Player.m_localPlayer.m_spawnEffects.m_effectPrefabs.Contains(firework))
			{
				return;
			}
			Array.Resize(ref Player.m_localPlayer.m_spawnEffects.m_effectPrefabs, 1);
			Player.m_localPlayer.m_spawnEffects.m_effectPrefabs[0] = firework;
		}
		#endregion

		#region Utilities


		#endregion
		#region cfgManger
		public static void CfgMangerAdd()
		{
			var name = configManager.CfgName;
			if (name == "")
			{
				Plugin.configManager.debugInfo = "name can't be empty!";
				return;
			}
			if (cfgList.ContainsKey(name))
			{
				Plugin.configManager.debugInfo = name + "is exsited, use Replace";
				return;
			}

			cfg.Value += UnpackTable(name);
			cfgList.Add(name, (TameTable)CfgTable.Clone());

			Plugin.configManager.debugInfo = configManager.CfgName + " added to the config";
		}
		public static void CfgMangerGet()
		{
			var name = configManager.CfgName;
			if (name == "")
			{
				Plugin.configManager.debugInfo = "name can't be empty!";
				return;
			}
			if (cfgList.ContainsKey(name))
			{
				configManager.obj = (TameTable)cfgList[name].Clone();
				CfgTable = (TameTable)configManager.obj;
				Plugin.configManager.debugInfo = name + " is Loaded";
				return;
			}
			Plugin.configManager.debugInfo = name + " is not exsit, use Add";
		}
		public static void CfgMangerRemove()
		{
			var name = configManager.CfgName;
			if (name == "")
			{
				Plugin.configManager.debugInfo = "name can't be empty!";
				return;
			}
			if (cfgList.ContainsKey(name))
			{
				cfgList.Remove(name);
				if (cfgList.Count == 0)
				{
					cfg.Value = "";
				}
				else { RemoveCfg(name); }

				Plugin.configManager.debugInfo = name + " Removed";
				return;
			}
			Plugin.configManager.debugInfo = name + " is not exsit, use Add";
		}
		public static void cfgMangerReplace()
		{
			var name = configManager.CfgName;
			if (name == "")
			{
				Plugin.configManager.debugInfo = "name can't be empty!";
				return;
			}
			if (cfgList.ContainsKey(name))
			{
				cfgList.Remove(name);
				RemoveCfg(name);
				cfgList.Add(name, (TameTable)CfgTable.Clone());
				cfg.Value += UnpackTable(name);
				Plugin.configManager.debugInfo = name + " Replaced";
				return;
			}
			Plugin.configManager.debugInfo = name + " is not exsit, use Add";

		}
		private static string UnpackTable(string name)
		{
			string l = "";
			var t = CfgTable;
			if (cfgList.Count != 0) { l += ";"; }
			l += name;
			l += "," + t.commandable.ToString().ToLower();
			l += "," + t.tamingTime;
			l += "," + t.fedDuration;
			l += "," + t.consumeRange;
			l += "," + t.consumeSearchInterval;
			l += "," + t.consumeSearchRange;
			l += "," + t.consumeHeal;
			l += "," + t.consumeItems;
			l += "," + t.changeFaction.ToString().ToLower();
			l += "," + t.procretion.ToString().ToLower();
			l += "," + t.maxCreatures;
			l += "," + t.pregnancyChance;
			l += "," + t.pregnancyDuration;
			l += "," + t.growTime;
			return l;
		}
		private static bool RemoveCfg(string name)
		{
			string s = "";
			bool result = false;
			string[] list = cfg.Value.Split(new char[] { ';' });
			foreach (string tt in list)
			{
				string[] set = tt.Split(new char[] { ',' });
				if (set[0] != name)
				{
					s += tt;
					s += ";";
				}
				else
				{
					result = true;
				}
			}
			if (s.Length <= 1)
			{
				cfg.Value = "";
				return result;
			}
			s = s.Substring(0, s.Length - 1);
			cfg.Value = s;
			return result;
		}
		#endregion cfgManger
	}
}