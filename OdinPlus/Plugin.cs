using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;
namespace OdinPlus
{
	[BepInPlugin("buzz.valheim.OdinPlus", "OdinPlus", "0.0.1")]
	public class Plugin : BaseUnityPlugin
	{
		#region Config Var
		//public static ConfigEntry<int> nexusID;
		public static ManualLogSource logger;
		public static ConfigEntry<KeyboardShortcut> KS_SecondInteractkey;
		public static ConfigEntry<KeyboardShortcut> KS_debug;
		public static ConfigEntry<string> CFG_ItemSellValue;
		public static ConfigEntry<string> CFG_Pets;
		#endregion
		public static GameObject OdinPlusRoot;
		private void Awake()
		{
			Plugin.logger = base.Logger;
			CFG_ItemSellValue = base.Config.Bind<string>("Config", "ItemSellValue", "Wood:1;Coins:1");
			CFG_Pets = base.Config.Bind<string>("Config", "PetList", "Troll,GoblinShaman");
			//Plugin.nexusID = base.Config.Bind<int>("General", "NexusID", 354, "Nexus mod ID for updates");
			KS_SecondInteractkey = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "Second Interact key", new KeyboardShortcut(KeyCode.F));
			KS_debug = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "debug key", new KeyboardShortcut(KeyCode.F3));
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

			//notice:: init here
			OdinPlusRoot = new GameObject("OdinPlus");
			OdinPlusRoot.AddComponent<OdinPlus>();
			DontDestroyOnLoad(OdinPlusRoot);
			OdinScore.init();
			DBG.blogInfo("OdinPlus Loadded");
		}

		#region patch		
		#region Player and Console and Fejd
		[HarmonyPatch(typeof(Player), "Update")]
		private static class Patch_Player_Update
		{
			private static void Postfix(Player __instance)
			{
				if (CheckPlayerNull())//|| OdinPlus.m_odinTrader == null)
				{
					return;
				}
				if (KS_SecondInteractkey.Value.IsDown() && __instance.GetHoverObject() != null)
				{
					if (__instance.GetHoverObject().transform.parent.GetComponent<OdinTrader>())
					{
						OdinPlus.m_odinTrader.SwitchSkill();
						return;
					}

				}
				#region debug
				if (KS_debug.Value.IsUp())
				{
				}
				if (Input.GetKeyDown(KeyCode.F4))
				{
					Pet.SummonHelper("Troll");

				}
				#endregion
				//end
			}
		}
		[HarmonyPatch(typeof(Console), "InputText")]
		private static class Patch_Console_InputText
		{
			private static void Prefix()
			{
				OdinPlus.ProcessCommands(global::Console.instance.m_input.text);
			}
		}
		[HarmonyPatch(typeof(FejdStartup), "Start")]
		private static class FejdStartup_Start_Patch
		{
			private static void Postfix()
			{
				if (Pet.Indicator != null)
				{
					return;
				}
				OdinPlus.initAssets();
				Pet.initIndicator();
				OdinSE.init();
				OdinMeads.init();
			}
		}
		#endregion
		#region Misc


		[HarmonyPatch(typeof(Localization), "SetupLanguage")]
		public static class MyLocalizationPatch
		{
			public static void Postfix(Localization __instance, string language)
			{
				BuzzLocal.init(language, __instance);
				BuzzLocal.UpdateDictinary();
			}
		}

		[HarmonyPatch(typeof(PlayerProfile), "SavePlayerData")]
		public static class PlayerProfile_SavePlayerData_Patch
		{
			public static void Postfix(PlayerProfile __instance, Player player)
			{
				if (CheckPlayerNull())
				{
					return;
				}
				OdinScore.saveOdinData(player.GetPlayerName());
			}
		}

		[HarmonyPatch(typeof(PlayerProfile), "LoadPlayerData")]//! Change Patch Point
		private static class Patch_PlayerProfile_LoadPlayerData
		{
			private static void Postfix(PlayerProfile __instance)
			{
				//DBG.blogWarning("loading");
				if (CheckPlayerNull()) { return; }
				OdinScore.loadOdinData(Player.m_localPlayer.GetPlayerName());
				if (OdinPlus.OdinNPCParent == null)
				{
					OdinPlus.OdinNPCParent = new GameObject("OdinPlus.OdinNPCParent");
					OdinPlus.OdinNPCParent.transform.SetParent(OdinPlusRoot.transform);
				}
				OdinPlus.initNPCs();
			}
		}

		[HarmonyPatch(typeof(Raven), "Awake")]//----------Indicator
		private static class Patch_Raven_Awake
		{
			private static void Postfix(Raven __instance)
			{
				Instantiate(__instance.m_exclamation, Vector3.zero, Quaternion.identity, Pet.Indicator.transform);
			}
		}

		[HarmonyPatch(typeof(Trader), "Start")]//add remove tthis
		private static class Patch_Trader_Start
		{
			private static void Prefix(Trader __instance)
			{
				var c = OdinMeads.MeadList[0].GetComponent<ItemDrop>();
				__instance.m_items.Add(new Trader.TradeItem
				{
					m_prefab = c,
					m_stack = 1,
					m_price = 1
				});
			}
		}
		#endregion
		#region ZnetScene
		[HarmonyPatch(typeof(ZNetScene), "Awake")]
		private static class ZNetScene_Awake_Prefix
		{
			private static void Prefix(ZNetScene __instance)
			{
				OdinMeads.Register(__instance);
			}
		}
		[HarmonyPatch(typeof(ZNetScene), "Awake")]
		private static class ZNetScene_Awake_Patch
		{
			private static void Postfix(ZNetScene __instance)
			{
				Pet.init(__instance);
				Pet.Register();
			}
		}
		[HarmonyPatch(typeof(ZNetScene), "Shutdown")]
		private static class ZNetScene_Shutdown_Patch
		{
			private static void Prefix()
			{
				Pet.Clear();
				if (!(OdinPlus.OdinNPCParent == null))
				{
					OdinPlus.m_odinTrader.RestTerrian();
					Destroy(OdinPlus.OdinNPCParent);
					return;
				}
				//Destroy(PrefabParent);
			}
		}
		#endregion
		#region ODB
		[HarmonyPatch(typeof(ObjectDB), "Awake")]
		private static class Patch_ObjectDB_Awake
		{
			private static void Postfix(ObjectDB __instance)
			{
				//OdinPlus.initAssets();
				OdinSE.Register();
				OdinMeads.Register(__instance);
			}
		}
		#endregion
		#endregion

		#region Tool
		public static bool CheckPlayerNull(bool log = false)
		{
			if (Player.m_localPlayer == null)
			{
				if (log) { DBG.blogWarning("Player is Null"); }

				return true;
			}
			return false;
		}

		#endregion

	}

}