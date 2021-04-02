using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace DebugAwakeQuene
{
	[BepInPlugin("buzz.valhei.name", "name", "0.0.1")]
	public class Plugin : BaseUnityPlugin
	{
		#region Var
		//heloot
		//public static ConfigEntry<int> nexusID;
		private static ManualLogSource logger;
		Harmony _harmony;
		#endregion

		private void Awake()
		{
			Plugin.logger = base.Logger;

			//Plugin.nexusID = base.Config.Bind<int>("General", "NexusID", 354, "Nexus mod ID for updates");
			_harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
			Plugin.logger.LogInfo("name Loadded");
		}
		//?
		[HarmonyPatch(typeof(ZNet), "Awake")]
		private static class Postfix_ZNet_Awake
		{
			private static void Postfix()
			{
				Debug.LogWarning("ZNet_Awake post");
			}
		}
		[HarmonyPatch(typeof(ZNet), "Awake")]
		private static class Prefix_ZNet_Awake
		{
			private static void Prefix()
			{
				Debug.LogWarning("ZNet_Awake prefix");
			}
		}
		[HarmonyPatch(typeof(ObjectDB), "Awake")]
		private static class Postfix_ObjectDB_Awake
		{
			private static void Postfix()
			{
				Debug.LogWarning("ObjectDB_Awake post");
			}
		}
		[HarmonyPatch(typeof(ObjectDB), "Awake")]
		private static class Prefix_ObjectDB_Awake
		{
			private static void Prefix()
			{
				Debug.LogWarning("ObjectDB_Awake prefix");
			}
		}
		[HarmonyPatch(typeof(ZNetScene), "Awake")]
		private static class Postfix_ZNetScene_Awake
		{
			private static void Postfix()
			{
				Debug.LogWarning("ZNetScene_Awake post");
			}
		}
		[HarmonyPatch(typeof(ZNetScene), "Awake")]
		private static class Prefix_ZNetScene_Awake
		{
			private static void Prefix()
			{
				Debug.LogWarning("ZNetScene_Awake prefix");
			}
		}

		[HarmonyPatch(typeof(ZoneSystem), "Awake")]
		private static class Postfix_ZoneSystem_Awake
		{
			private static void Postfix()
			{
				Debug.LogWarning("ZoneSystem_Awake post");
			}
		}
		[HarmonyPatch(typeof(ZoneSystem), "Awake")]
		private static class Prefix_ZoneSystem_Awake
		{
			private static void Prefix()
			{
				Debug.LogWarning("ZoneSystem_Awake prefix");
			}
		}




	}
}
