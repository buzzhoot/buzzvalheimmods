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
		public static ConfigEntry<KeyboardShortcut> KS_SecondInteractkey;
		public static ConfigEntry<KeyboardShortcut> KS_debug;
		public static ConfigEntry<string> CFG_ItemSellValue;
		public static ConfigEntry<string> CFG_Pets;
		#endregion

		#region Plugin Var
		public static ManualLogSource logger;
		public static string LastCmd;
		//prefab
		public static GameObject OdinPlusRoot;
		public static GameObject OdinPrefab;
		private static GameObject OdinNPCParent;
		private static GameObject OdinGuiRoot;
		//trader
		private static OdinTrader m_odinTrader;
		//Pet
		public static GameObject PrefabParent;

		#endregion

		#region Mono
		private void Awake()
		{
			Plugin.logger = base.Logger;
			CFG_ItemSellValue = base.Config.Bind<string>("Config", "ItemSellValue", "Wood:1;Coins:1");
			CFG_Pets = base.Config.Bind<string>("Config", "PetList", "Troll,GoblinShaman");
			//Plugin.nexusID = base.Config.Bind<int>("General", "NexusID", 354, "Nexus mod ID for updates");
			KS_SecondInteractkey = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "Second Interact key", new KeyboardShortcut(KeyCode.F));
			KS_debug = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "debug key", new KeyboardShortcut(KeyCode.F3));
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
			init();
			DBG.blogInfo("OdinPlus Loadded");
		}
		public void Start()
		{
			OdinScore.init();
		}
		public void Update()
		{
			CheckPlayerNull();
			CheckConsole();
		}
		#endregion

		#region patch
		[HarmonyPatch(typeof(Player), "Update")]
		private static class Patch_Player_Update
		{
			private static void Postfix(Player __instance)
			{
				if (CheckPlayerNull())//|| m_odinTrader == null)
				{
					return;
				}
				if (KS_SecondInteractkey.Value.IsDown() && __instance.GetHoverObject()!= null&&__instance.GetHoverObject().transform.parent.name==m_odinTrader.name)
				{
					m_odinTrader.SwitchSkill();
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

		#region Misc
		[HarmonyPatch(typeof(FejdStartup), "Start")]
		private static class FejdStartup_Start_Patch
		{
			private static void Postfix()
			{
				//skillIcon = ObjectDB.instance.GetItemPrefab("HelmetOdin").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
				Pet.initIndicator();
			}
		}

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

		[HarmonyPatch(typeof(PlayerProfile), "LoadPlayerData")]//-----------init odin npc
		private static class Patch_PlayerProfile_LoadPlayerData
		{
			private static void Postfix(PlayerProfile __instance)
			{
				//DBG.blogWarning("loading");
				if (CheckPlayerNull()) { return; }
				OdinScore.loadOdinData(Player.m_localPlayer.GetPlayerName());
				if (OdinNPCParent == null)
				{
					OdinNPCParent = new GameObject("OdinNPCParent");
					OdinNPCParent.transform.SetParent(OdinPlusRoot.transform);
				}
				initOdinPrefab();
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
		#endregion
		#region ZnetScene
		[HarmonyPatch(typeof(ZNetScene), "Awake")]
		private static class ZNetScene_Awake_Patch
		{
			private static void Postfix(ZNetScene __instance)
			{
				Pet.init(__instance);
				Pet.Register();
			}
		}

		/* 		[HarmonyPatch(typeof(ZNetScene), "GetPrefab", new Type[] { typeof(string) })]
				public static class GetPrefab_Prefix_Patch
				{
					public static bool Prefix(string name, ref GameObject __result)
					{
						GameObject go;
						if (Pet.GetPrefab(name, out go))
						{
							DBG.b();
							__result = go;
							return false;
						}
						return true;
					}
				} */

		[HarmonyPatch(typeof(ZNetScene), "Shutdown")]
		private static class ZNetScene_Shutdown_Patch
		{
			private static void Prefix()
			{
				Pet.Clear();
				if (!(OdinNPCParent == null))
				{
					m_odinTrader.RestTerrian();
					Destroy(OdinNPCParent);
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
			private static void Prefix()
			{

			}
		}
		#endregion
		#endregion

		#region Tool
		public static void CheckConsole()
		{

			if (global::Console.instance != null && global::Console.instance.m_chatWindow.gameObject.activeInHierarchy)
			{
				string cmd = LastCmd;
				if (Input.GetKeyDown(KeyCode.Return))
				{
					if (cmd.Length > 0)
					{
						ProcessCommands(cmd);
						return;
					}
				}
				LastCmd = global::Console.instance.m_input.text;
			}
		}
		public static bool CheckPlayerNull(bool log = false)
		{
			if (Player.m_localPlayer == null)
			{
				if (log) { DBG.blogWarning("Player is Null"); }

				return true;
			}
			return false;
		}
		public static void ProcessCommands(string inCommand)
		{
			if (inCommand.Length > 0)
			{
				if (inCommand.StartsWith(" "))
				{
					inCommand = inCommand.Remove(0, 1);
				}
				if (inCommand == "bzd")
				{
					Destroy(OdinNPCParent);
				}
				if (inCommand == "test")
				{
					Destroy(OdinPlusRoot);
				}
			}
		}
		#endregion

		#region Feature
		public static void init()
		{
			OdinPlusRoot = new GameObject("OdinPlus");
			PrefabParent = new GameObject("OdinPlusPrefabs");
			PrefabParent.SetActive(false);
			PrefabParent.transform.SetParent(OdinPlusRoot.transform);
			OdinNPCParent = new GameObject("OdinNPCs");
			OdinNPCParent.SetActive(false);
			OdinNPCParent.transform.SetParent(OdinPlusRoot.transform);
			DontDestroyOnLoad(OdinPlusRoot);
		}
		public static void initOdinPrefab()
		{
			var podin = ZNetScene.instance.GetPrefab("odin");
			var pfire = ZNetScene.instance.GetPrefab("fire_pit");
			var pcaul = ZNetScene.instance.GetPrefab("piece_cauldron");
			var odin = CopyChildren(podin);
			odin.transform.SetParent(OdinNPCParent.transform);
			var c = new GameObject("coll");
			c.AddComponent<CapsuleCollider>();
			c.transform.SetParent(odin.transform);
			c.transform.localScale = new Vector3(1, 2, 1);
			c.transform.localPosition = Vector3.up;
			var fire = CopyChildren(pfire);
			var caul = CopyChildren(pcaul);
			fire.transform.SetParent(OdinNPCParent.transform);
			caul.transform.SetParent(OdinNPCParent.transform);


			odin.transform.localPosition = new Vector3(0f, 0, 0f);
			fire.transform.localPosition = new Vector3(1.5f, 0, -0.5f);
			caul.transform.localPosition = new Vector3(1.5f, 0, -0.5f);
			Destroy(fire.transform.Find("PlayerBase").gameObject);
			fire.transform.Find("_enabled_high").gameObject.SetActive(true);
			caul.transform.Find("HaveFire").gameObject.SetActive(true);
			m_odinTrader = odin.AddComponent<OdinTrader>();
			caul.AddComponent<OdinStore>();
			Plugin.OdinPrefab = odin;
			OdinNPCParent.SetActive(true);
			//SE.init();
		}
		private static void CreateOdinGui(Transform GuiBase)
		{
			if (OdinGuiRoot != null) { return; }
			var bak = GuiBase;//.Find("Store").Find("border (1)");
			DBG.cprt((bak == null).ToString());
			OdinGuiRoot = new GameObject("OdinGui");
			// OdinGuiRoot.transform.SetParent(OdinGuiRoot.transform.parent.parent);
			//RectTransform rt = OdinGuiRoot.GetComponent<RectTransform>();
			//remove
			//Destroy(OdinGuiRoot.GetComponent<StoreGui>());
			//add
			//OdinGuiRoot.AddComponent<OdinStoreGUI>();
			//OdinGuiRoot.transform.SetParent(GuiBase);
			//Instantiate(bak, OdinGuiRoot.transform);
		}

		#endregion

		#region Utilities
		public static GameObject CopyChildren(GameObject prefab)
		{
			int cc = prefab.transform.childCount;
			GameObject r = new GameObject(prefab.name);
			for (int i = 0; i < cc; i++)
			{
				var o = prefab.transform.GetChild(i).gameObject;
				var a = Instantiate(o, r.transform);
				a.name = o.name;
			}
			return r;
		}
		public static void CopyComponent(Component original, GameObject destination)
		{
			System.Type type = original.GetType();
			Component copy = destination.AddComponent(type);
			// Copied fields can be restricted with BindingFlags
			FieldInfo[] fields = type.GetFields();
			PropertyInfo[] props = type.GetProperties();
			foreach (FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(original));
			}
			//foreach(PropertyInfo p in props)
			//{
			//    props.SetValue(copy, props.GetValue(original));
			//}
			return;
		}

		#endregion
	}

}