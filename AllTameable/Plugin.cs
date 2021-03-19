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


namespace AllTameable
{
	[BepInPlugin("buzz.valheim.AllTameable", "AllTameable", "1.2.0")]
	class Plugin : BaseUnityPlugin
	{
		#region Var
		#region Config
		public static ConfigEntry<int> nexusID;
		public static ConfigEntry<string> cfg;
		public static ManualLogSource logger;
		#endregion
		#region Data
		public class TameTable : ICloneable
		{
			//public string name;
			public bool commandable = true;
			public float tamingTime = 600;
			public float fedDuration = 300;
			public float consumeRange = 2;
			public float consumeSearchInterval = 10;
			public float consumeHeal = 10;
			public float consumeSearchRange = 20;
			public string consumeItems = "RawMeat";
			public bool changeFaction = false;
			public bool procretion = false;
			public int maxCreatures = 5;
			public float pregnancyChance = 0.33f;
			public float pregnancyDuration = 10f;
			public float growTime = 60;
			public object Clone()
			{
				return this.MemberwiseClone();
			}
		}
		public static Dictionary<string, TameTable> cfgList = new Dictionary<string, TameTable>();
		public static TameTable CfgTable = new TameTable();
		#endregion Data
		#region plugin
		public static bool loaded = false;
		public static GameObject Root;
		public static PrefabManager prefabManager;
		public static PetManager petManager;
		public static ConfigManager configManager;
		#endregion plugin

		#endregion Var

		#region  Mono
		private void Awake()
		{
			Plugin.logger = base.Logger;
			Plugin.nexusID = base.Config.Bind<int>("Nexus", "NexusID", 478, "Nexus mod ID for updates");
			cfg = base.Config.Bind<string>("1General", "Settings",
				"Troll,true,1800,600,2,10,10,20,NeckTailGrilled:Honey:CookedMeat,true,true,5,0.33,10,60;GoblinBrute,true,1800,600,2,10,10,20,CookedLoxMeat:Bread,false,false,3,0.99,10,60",
				"name,commandable,tamingTime,fedDuration,consumeRange,consumeSearchInterval,consumeHeal,consumeSearchRange,consumeItem:consumeItem,changeFaction,procretion,maxCreatures,pregnancyChance,pregnancyDuration,growTime,;next one;...;last one");

			loaded = initCfg();

			Root = new GameObject("AllTameable Root");
			prefabManager = Root.AddComponent<PrefabManager>();
			petManager = Root.AddComponent<PetManager>();
			configManager = Root.AddComponent<ConfigManager>();
			configManager.obj = CfgTable;
			configManager.title = "All Tameable Setup " + (loaded ? "Loaded" : "!!Load Fail!!");
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
					table.maxCreatures = int.Parse(set[11]);
					table.pregnancyChance = float.Parse(set[12]);
					table.pregnancyDuration = float.Parse(set[13]);
					table.growTime = float.Parse(set[14]);
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

		#endregion

		#region Utilities


		#endregion
		#region cfgManger
		public static void CfgMangerAdd()
		{
			var name = configManager.CfgName;
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
			if (cfgList.ContainsKey(name))
			{
				CfgTable = cfgList[name];
				return;
			}
			Plugin.configManager.debugInfo = name + "is not exsit, use Add";
		}
		public static void CfgMangerRemove()
		{
			var name = configManager.CfgName;
			if (cfgList.ContainsKey(name))
			{
				cfgList.Remove(name);
				RemoveCfg(name);
				Plugin.configManager.debugInfo = name + " Removed";
				return;
			}
			Plugin.configManager.debugInfo = name + "is not exsit, use Add";
		}
		public static void cfgMangerReplace()
		{
			var name = configManager.CfgName;
			if (cfgList.ContainsKey(name))
			{
				cfgList.Remove(name);
				RemoveCfg(name);
				cfgList.Add(name,(TameTable)CfgTable.Clone());
				cfg.Value += UnpackTable(name);
				Plugin.configManager.debugInfo = name + " Replaced";
				return;
			}
			Plugin.configManager.debugInfo = name + "is not exsit, use Add";

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
			s.Remove(s.Length, 1);
			cfg.Value = s;
			return result;
		}
		#endregion cfgManger
	}
}